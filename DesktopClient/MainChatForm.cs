using Microsoft.IdentityModel.Tokens;
using Models.Binding;
using Models.Pagination;
using Models.Search;
using Models.View;
using Storage;
using System.Data;

namespace DesktopClient
{
	public partial class MainChatForm : Form
	{
		private ChatClient _chatClient;
		private string _currentInterlocutor;

		private int _currentPage = 1;
		private bool _isLoading = false;
		private bool _hasMore = true;
		private string _searchQuery = null;
		private string _me;
		private readonly AuthService _authService;

		private int _currentChatId = 0;
		private int _currentMessagePage = 1;
		private const int MESSAGES_PER_PAGE = 30;
		private bool _isLoadingMessages = false;
		private bool _hasMoreMessages = true;
		private bool _isAtTop = false;
		private bool _isChatLoaded = false; // Добавляем это поле


		private List<MessageViewModel> _loadedMessages = new();

		public MainChatForm(string username)
		{
			_me = username;
			var apiClient = new APIClient();
			_authService = new AuthService(apiClient);
			InitializeComponent();

			InitializeDatabase(username);

			_chatClient = new ChatClient(username);
			_chatClient.StartReceiving();
			_chatClient.OnMessageReceived += ChatClient_OnMessageReceived;

			this.Load += MainChatForm_Load;
			this.SizeChanged += MainChatForm_SizeChanged;
		}



		public void InitializeDatabase(string username)
		{
			using var db = new ChatDatabase(username);
			db.Database.EnsureCreated();
		}

		#region События
		private void ChatClient_OnMessageReceived(MessageDto msg)
		{
			// UI поток нужен!
			if (InvokeRequired)
			{
				Invoke(new Action(() => ChatClient_OnMessageReceived(msg)));
				return;
			}

			// Если сообщение относится к открытому чату → показываем
			if (msg.Sender == _currentInterlocutor)
			{
				AddIncomingMessageToUI(msg);
			}

			// Обновляем список чатов
			_ = LoadChatsFromDatabase(reset: true);
		}
		private async void ChatsListPanel_MouseWheel(object sender, MouseEventArgs e)
		{
			if (_isLoading || !_hasMore)
				return;

			// Прокрутка вниз e.Delta < 0
			if (e.Delta < 0)
			{
				var panel = chatsListPanel;

				// Почти у нижней границы?
				if (panel.VerticalScroll.Value + panel.Height >= panel.VerticalScroll.Maximum - 100)
				{
					await LoadChatsFromDatabase();
				}
			}
		}
		private async void MessagesPanel_MouseWheel(object sender, MouseEventArgs e)
		{
			if (_currentInterlocutor.IsNullOrEmpty())
			{
				return;
			}
			// Если пользователь прокручивает вверх
			if (e.Delta > 0)
			{
				// и почти у верхней границы
				if (messagesPanel.VerticalScroll.Value <= messagesPanel.VerticalScroll.Minimum + 5)
				{
					if (!_isLoadingMessages && _hasMoreMessages)
					{
						await LoadMoreMessagesAsync(); // или LoadMoreMessagesPreserveScrollAsync()
					}
				}
			}
		}
		private async void MessagesPanel_Scroll(object sender, ScrollEventArgs e)
		{
			// Если нет собеседника — выходим
			if (string.IsNullOrEmpty(_currentInterlocutor))
				return;

			// Если уже идёт загрузка — выходим
			if (_isLoadingMessages || !_hasMoreMessages)
				return;

			// Проверяем, долистал ли пользователь до самого верха
			if (messagesPanel.VerticalScroll.Value == 0)
			{
				int oldScrollHeight = messagesPanel.DisplayRectangle.Height;

				await LoadMoreMessagesAsync();

				// После подгрузки восстановим положение скролла,
				// чтобы чат не "прыгал" вниз
				int newScrollHeight = messagesPanel.DisplayRectangle.Height;
				messagesPanel.VerticalScroll.Value = newScrollHeight - oldScrollHeight;
			}
		}

		private void MainChatForm_Load(object sender, EventArgs e)
		{
			_chatClient.StartReceiving();

			CenterControls();
			LoadChatsFromDatabase();
		}
		private async void BtnSearch_Click(object sender, EventArgs e)
		{
			_searchQuery = txtSearch.Text.Trim();

			if (string.IsNullOrWhiteSpace(_searchQuery))
			{
				usersSearchPanel.Visible = false;
				await LoadChatsFromDatabase(reset: true);
				return;
			}

			await LoadChatsFromDatabase(reset: true);
			await LoadUsersSearchResultsAsync(_searchQuery);
		}
		private async void ChatsListPanel_Scroll(object sender, ScrollEventArgs e)
		{
			if (_hasMore == false) return;
			if (_isLoading) return;

			var panel = chatsListPanel;

			if (panel.VerticalScroll.Value + panel.Height >= panel.VerticalScroll.Maximum - 50)
			{
				// Пользователь долистал почти до конца
				await LoadChatsFromDatabase();
			}
		}

		private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && !e.Shift)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
				SendMessageAsync();
			}
		}

		private void BtnSend_Click(object sender, EventArgs e)
		{
			SendMessageAsync();
		}
		#endregion

		#region Отрисовка

		private void PrependMessages(List<MessageViewModel> newMessages)
		{
			if (newMessages == null || newMessages.Count == 0) return;

			// Гарантируем, что работаем на UI-потоке (если вызывается из async методов - уже так обычно)
			if (InvokeRequired)
			{
				Invoke(new Action(() => PrependMessages(newMessages)));
				return;
			}

			messagesPanel.SuspendLayout();

			try
			{
				// --- 1) Найдём «первый видимый контрол» до вставки, чтобы потом восстановить скролл ---
				Control firstVisible = messagesPanel.Controls
					.Cast<Control>()
					.OrderBy(c => c.Top)
					.FirstOrDefault(c => c.Bottom > messagesPanel.VerticalScroll.Value);

				object firstVisibleTag = firstVisible?.Tag;
				int offsetFromTop = firstVisible != null ? firstVisible.Top - messagesPanel.VerticalScroll.Value : 0;

				// --- 2) Создаём контролы для новых сообщений в правильном хронологическом порядке ---
				// Предполагаем, что newMessages приходят от старых к новым; если нет — сортируем
				var sorted = newMessages.OrderBy(m => m.Timestamp).ToList();

				var newControls = new List<Control>();
				int totalOffset = 0;
				foreach (var msg in sorted)
				{
					var ctrl = CreateMessageControl(
						msg.Content,
						msg.Sender == _chatClient.GetUsername(),
						ConvertToLocalTime(msg.Timestamp).ToString("HH:mm"),
						0 // y — мы установим потом
					);

					// Присваиваем уникальный тег, чтобы потом можно было найти контрол (важно для сохранения позиции)
					// Используем id если есть, иначе комбинируем sender+timestamp
					ctrl.Tag = msg.Id != 0 ? (object)msg.Id : $"{msg.Sender}_{msg.Timestamp.Ticks}";

					// Возможно CreateMessageControl установил неправильную Top (0) — запомним размеры
					newControls.Add(ctrl);
					totalOffset += ctrl.Height + 5;
				}

				// --- 3) Сдвигаем вниз все существующие контролы на полученный offset ---
				// (это освободит место сверху для новых контролов)
				foreach (Control exist in messagesPanel.Controls)
				{
					exist.Top += totalOffset;
				}

				// --- 4) Вставляем новые контролы сверху (в том же порядке: от старых к новым) ---
				// Определяем стартовую позицию для первой новой панели:
				int startTop;
				if (messagesPanel.Controls.Count > 0)
				{
					// найдем минимальный Top после сдвига — это будет место для вставки
					startTop = messagesPanel.Controls.Cast<Control>().Min(c => c.Top) - totalOffset;
				}
				else
				{
					// пустая панель — используем небольшой отступ сверху
					startTop = 10;
				}

				// Однако более корректный startTop — если первый существующий контрол до сдвига имел TopFirst,
				// то новое место = TopFirst - totalOffset. Мы уже сдвинули, поэтому:
				if (firstVisible != null)
				{
					// firstVisible.Top уже увеличен на totalOffset, поэтому старт = firstVisible.Top - totalOffset - (смещение от начала панели)
					// но проще — найдем минимальный Top сейчас и используем его для стартовой позиции
					startTop = messagesPanel.Controls.Cast<Control>().Min(c => c.Top) - totalOffset;
					// если получилось очень маленькое значение — ограничим
					if (startTop < 5) startTop = 5;
				}

				// Но чтобы не полагаться на хитрые кейсы, мы просто ставим первую новую панель на
				// previousMinTop (до сдвига) или на 10, если не было элементов.
				int previousMinTop = messagesPanel.Controls.Count > 0
					? messagesPanel.Controls.Cast<Control>().Min(c => c.Top)
					: 10;
				// корректируем предыдущий min по смыслу: до сдвига он был previousMinTop - totalOffset
				int insertTop = previousMinTop - totalOffset;
				if (messagesPanel.Controls.Count == 0) insertTop = 10;

				// Теперь добавляем
				int currentTop = insertTop;
				foreach (var ctrl in newControls)
				{
					// Сохраняем X — CreateMessageControl уже выставил X (лево/право), поэтому оставляем его
					int x = ctrl.Location.X;

					ctrl.Location = new Point(x, currentTop);
					messagesPanel.Controls.Add(ctrl);
					ctrl.BringToFront();

					currentTop += ctrl.Height + 5;
				}

				// --- 5) Восстановим видимую позицию: найдём тот же firstVisible контрол (по Tag) и проскроллим к нему ---
				if (firstVisibleTag != null)
				{
					var sameControl = messagesPanel.Controls
						.Cast<Control>()
						.FirstOrDefault(c => c.Tag != null && c.Tag.Equals(firstVisibleTag));

					if (sameControl != null)
					{
						int newScroll = Math.Max(0, sameControl.Top - offsetFromTop);
						// В WinForms AutoScrollPosition ведёт себя странно: установка в (0, y) требует отрицательных координат.
						// Удобнее - установить Value напрямую, но Value может быть вне допустимого диапазона, поэтому защищаемся:
						var v = messagesPanel.VerticalScroll;
						int clamped = Math.Min(Math.Max(newScroll, v.Minimum), v.Maximum);
						try
						{
							messagesPanel.AutoScrollPosition = new Point(0, clamped);
						}
						catch
						{
							// fallback
							messagesPanel.VerticalScroll.Value = clamped;
						}
					}
					else
					{
						// fallback: если контрол не найден — ставим скролл на значение, увеличенное на totalOffset
						var v = messagesPanel.VerticalScroll;
						int clamped = Math.Min(Math.Max(messagesPanel.VerticalScroll.Value + totalOffset, v.Minimum), v.Maximum);
						try
						{
							messagesPanel.AutoScrollPosition = new Point(0, clamped);
						}
						catch
						{
							messagesPanel.VerticalScroll.Value = clamped;
						}
					}
				}
				else
				{
					// Если before не существовал — ничего не делаем (панель пустая ранее)
				}
			}
			finally
			{
				messagesPanel.ResumeLayout();
				messagesPanel.Invalidate();
			}
		}
		private void RenderAllMessages()
		{
			messagesPanel.Controls.Clear();

			if (!_loadedMessages.Any()) return;

			int yPos = 10;

			// Сортируем по времени (старые сверху, новые снизу)
			var sortedMessages = _loadedMessages.OrderBy(m => m.Timestamp).ToList();

			foreach (var msg in sortedMessages)
			{
				var messageControl = CreateMessageControl(
					msg.Content,
					msg.Sender == _chatClient.GetUsername(),
					ConvertToLocalTime(msg.Timestamp).ToString("HH:mm"),
					yPos
				);

				messagesPanel.Controls.Add(messageControl);
				yPos += messageControl.Height + 5;
			}

			// Прокручиваем к последнему сообщению только при первой загрузке
			if (_currentMessagePage == 1 && messagesPanel.Controls.Count > 0)
			{
				messagesPanel.ScrollControlIntoView(messagesPanel.Controls[messagesPanel.Controls.Count - 1]);
			}
		}
		private void AddMessageToChat(string message, bool isMyMessage)
		{
			var yPos = messagesPanel.Controls.Count > 0 ?
				messagesPanel.Controls[messagesPanel.Controls.Count - 1].Bottom + 5 : 10;

			var messageControl = CreateMessageControl(message, isMyMessage, DateTime.Now.ToString("HH:mm"), yPos);
			messagesPanel.Controls.Add(messageControl);

			// Прокрутка вниз
			messagesPanel.ScrollControlIntoView(messageControl);
		}
		private Panel CreateUserSearchItem(UserBindingModel user, int y)
		{
			var panel = new Panel
			{
				Size = new Size(290, 40),
				Location = new Point(10, y),
				BackColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				Cursor = Cursors.Hand
			};

			var lbl = new Label
			{
				Text = user.Username,
				Font = new Font("Segoe UI", 11),
				AutoSize = true,
				Location = new Point(10, 10)
			};

			panel.Controls.Add(lbl);

			panel.Click += async (s, e) =>
			{
				if (_chatClient._chatLogic.ReadElementAsync(new ChatSearchModel
				{
					Interlocutor = user.Username
				}) != null)
				{
					await SelectChatAsync(user.Username);
					return;
				}
				// создаём новый чат
				await _chatClient._chatLogic.CreateAsync(new ChatBindingModel
				{
					CurrentUser = _chatClient.GetUsername(),
					Interlocutor = user.Username
				});

				usersSearchPanel.Visible = false;
				txtSearch.Clear();

				await LoadChatsFromDatabase(reset: true);
				await SelectChatAsync(user.Username);
			};

			return panel;
		}
		private Panel CreateChatItem(ChatViewModel chat, int yPos)
		{
			var panel = new Panel()
			{
				Size = new Size(350, 80),
				Location = new Point(0, yPos),
				BackColor = Color.White,
				Cursor = Cursors.Hand,
				BorderStyle = BorderStyle.FixedSingle
			};

			var avatar = new Panel()
			{
				Size = new Size(50, 50),
				Location = new Point(10, 15),
				BackColor = Color.FromArgb(86, 130, 163)
			};
			panel.Controls.Add(avatar);

			var lblName = new Label()
			{
				Text = chat.Interlocutor,
				Font = new Font("Segoe UI", 12, FontStyle.Bold),
				AutoSize = true,
				Location = new Point(70, 15)
			};
			panel.Controls.Add(lblName);

			var lblLastMessage = new Label()
			{
				Text = chat.LastMessageText,
				Font = new Font("Segoe UI", 10),
				ForeColor = Color.Gray,
				AutoSize = true,
				Location = new Point(70, 40)
			};
			panel.Controls.Add(lblLastMessage);

			// Форматируем время с конвертацией часового пояса
			string formattedTime = "";
			if (chat.LastMessageTime.HasValue)
			{
				formattedTime = ConvertToLocalTime(chat.LastMessageTime.Value).ToString("dd.MM HH:mm");
			}

			var lblTime = new Label()
			{
				Text = formattedTime,
				Font = new Font("Segoe UI", 9),
				ForeColor = Color.Gray,
				AutoSize = true,
				Location = new Point(280, 15)
			};
			panel.Controls.Add(lblTime);

			panel.Click += async (s, e) => await SelectChatAsync(chat.Interlocutor);

			return panel;
		}
		private Panel CreateMessageControl(string text, bool isMyMessage, string time, int yPos)
		{
			var panel = new Panel();
			panel.AutoSize = false;
			panel.BackColor = Color.Transparent;

			// Используем TableLayoutPanel для точного позиционирования
			var tableLayout = new TableLayoutPanel();
			tableLayout.ColumnCount = 1;
			tableLayout.RowCount = 2;
			tableLayout.AutoSize = true;
			tableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			tableLayout.BackColor = isMyMessage ? Color.FromArgb(220, 248, 198) : Color.White;
			tableLayout.BorderStyle = BorderStyle.FixedSingle;
			tableLayout.Padding = new Padding(10, 10, 10, 5);

			// Настройка строк
			tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Фиксированная высота для времени

			// Текст сообщения
			var lblText = new Label();
			lblText.Text = text;
			lblText.Font = new Font("Segoe UI", 11, FontStyle.Regular);
			lblText.ForeColor = Color.Black;
			lblText.BackColor = Color.Transparent;
			lblText.AutoSize = true;
			lblText.MaximumSize = new Size(350, 0);
			lblText.Dock = DockStyle.Fill;
			lblText.TextAlign = ContentAlignment.TopLeft;

			// Время
			var lblTime = new Label();
			lblTime.Text = time;
			lblTime.Font = new Font("Segoe UI", 8, FontStyle.Regular);
			lblTime.ForeColor = Color.Gray;
			lblTime.AutoSize = false;
			lblTime.Height = 15;
			lblTime.Dock = DockStyle.Right; // Выравнивание по правому краю
			lblTime.TextAlign = ContentAlignment.MiddleRight;
			lblTime.BackColor = Color.Transparent;

			// Добавляем контролы в TableLayoutPanel
			tableLayout.Controls.Add(lblText, 0, 0);
			tableLayout.Controls.Add(lblTime, 0, 1);

			// Добавляем TableLayoutPanel в панель
			panel.Controls.Add(tableLayout);
			tableLayout.Dock = DockStyle.Fill;

			// Устанавливаем размер панели
			panel.Size = new Size(tableLayout.PreferredSize.Width, tableLayout.PreferredSize.Height);

			// Позиционирование
			if (isMyMessage)
			{
				panel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				panel.Location = new Point(messagesPanel.Width - panel.Width - 20, yPos);
			}
			else
			{
				panel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				panel.Location = new Point(20, yPos);
			}

			return panel;
		}
		#endregion

		#region Функциональность чата
		private void AddIncomingMessageToUI(MessageDto msg)
		{
			var yPos = messagesPanel.Controls.Count > 0
				? messagesPanel.Controls[messagesPanel.Controls.Count - 1].Bottom + 5
				: 10;

			var panel = CreateMessageControl(
				msg.Content,
				isMyMessage: false,
				ConvertToLocalTime(msg.Timestamp).ToString("HH:mm"),
				yPos
			);

			messagesPanel.Controls.Add(panel);

			// скроллим вниз
			messagesPanel.ScrollControlIntoView(panel);
		}
		private async Task LoadMoreMessagesAsync()
		{
			if (!_hasMoreMessages || _isLoadingMessages) return;

			await LoadMessagesAsync(_currentInterlocutor, loadMore: true);
		}
		private async Task LoadUsersSearchResultsAsync(string query)
		{
			try
			{
				usersSearchPanel.Controls.Clear();
				usersSearchPanel.Visible = true;

				var (success, result) = await _authService.SearchUsersAsync(query, 1, 30);
				if (!success || result == null || result.Items.Count == 0)
				{
					usersSearchPanel.Controls.Add(new Label
					{
						Text = "Пользователи не найдены",
						Font = new Font("Segoe UI", 10),
						ForeColor = Color.Gray,
						AutoSize = true,
						Location = new Point(15, 10)
					});
					return;
				}


				int y = 10;

				foreach (var user in result.Items)
				{
					var panel = CreateUserSearchItem(user, y);
					usersSearchPanel.Controls.Add(panel);
					y += 50;
				}

				if (y == 10)
				{
					usersSearchPanel.Controls.Add(new Label
					{
						Text = "Все пользователи уже в ваших чатах",
						Font = new Font("Segoe UI", 10),
						ForeColor = Color.Gray,
						AutoSize = true,
						Location = new Point(15, 10)
					});
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка поиска пользователей: " + ex.Message);
			}
		}
		private async Task LoadChatsFromDatabase(bool reset = false)
		{
			if (_isLoading) return;
			_isLoading = true;

			try
			{
				if (reset)
				{
					_currentPage = 1;
					_hasMore = true;
					chatsListPanel.Controls.Clear();
				}

				PaginatedResult<ChatViewModel> result;

				if (string.IsNullOrWhiteSpace(_searchQuery))
					result = await _chatClient._chatLogic.GetRecentChatsAsync(_currentPage, 5);
				else
					result = await _chatClient._chatLogic.SearchChatsAsync(_searchQuery, _currentPage, 5);

				if (result == null || result.Items.Count == 0)
				{
					_hasMore = false;
					if (reset)
					{
						Label noLabel = new Label()
						{
							Text = "Пока нет чатов...",
							AutoSize = true,
							Font = new Font("Segoe UI", 11),
							ForeColor = Color.Gray,
							Location = new Point(50, 50)
						};
						chatsListPanel.Controls.Add(noLabel);
					}
					return;
				}

				int yPos = chatsListPanel.Controls.Count == 0
					? 0
					: chatsListPanel.Controls.Cast<Control>().Max(c => c.Bottom);

				foreach (var chat in result.Items)
				{
					var chatPanel = CreateChatItem(chat, yPos);
					chatsListPanel.Controls.Add(chatPanel);
					yPos += 80;
				}

				if (result.Items.Count < 5)
					_hasMore = false;
				else
					_currentPage++;
			}
			finally
			{
				_isLoading = false;
			}
		}
		private async Task LoadMessagesAsync(string interlocutor, bool loadMore = false)
		{
			if (_isLoadingMessages) return;
			_isLoadingMessages = true;

			try
			{
				// новый чат — сбрасываем состояние
				if (!loadMore)
				{
					_currentInterlocutor = interlocutor;
					_currentMessagePage = 1;
					_hasMoreMessages = true;
					_loadedMessages.Clear();
					messagesPanel.Controls.Clear();
					_isChatLoaded = false;
				}

				var chat = await _chatClient._chatLogic.ReadElementAsync(new ChatSearchModel
				{
					CurrentUser = _chatClient.GetUsername(),
					Interlocutor = interlocutor
				});

				if (chat == null)
				{
					await _chatClient._chatLogic.CreateAsync(new ChatBindingModel
					{
						CurrentUser = _chatClient.GetUsername(),
						Interlocutor = interlocutor
					});
					return;
				}

				_currentChatId = chat.Id;

				// загружаем страницу
				var pageResult = await _chatClient._messageLogic.GetMessagesByChatIdAsync(
					chatId: chat.Id,
					page: _currentMessagePage,
					pageSize: MESSAGES_PER_PAGE
				);

				if (pageResult?.Items != null && pageResult.Items.Any())
				{
					if (loadMore)
					{
						PrependMessages(pageResult.Items);

						_currentMessagePage++;
					}
					else
					{
						_loadedMessages = pageResult.Items.ToList();
						RenderAllMessages();
					}

					_hasMoreMessages = pageResult.HasNextPage;
					_isChatLoaded = true;
				}
				else if (!loadMore)
				{
					_isChatLoaded = false;
					messagesPanel.Controls.Clear();
					_loadedMessages.Clear();
				}
			}
			catch (Exception ex)
			{
				_isChatLoaded = false;
				MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				_isLoadingMessages = false;
			}
		}

		private async Task SelectChatAsync(string interlocutor)
		{
			_currentInterlocutor = interlocutor;
			_chatClient.SetCurrentInterlocutorAsync(interlocutor);
			// Обновляем заголовок чата
			chatHeaderPanel.Controls.Clear();

			var lblChatName = new Label();
			lblChatName.Text = interlocutor;
			lblChatName.Font = new Font("Segoe UI", 14, FontStyle.Bold);
			lblChatName.ForeColor = Color.Black;
			lblChatName.AutoSize = true;
			lblChatName.Location = new Point(15, 18);
			chatHeaderPanel.Controls.Add(lblChatName);

			// Загружаем сообщения для выбранного чата 
			await LoadMessagesAsync(interlocutor);
		}
		private async Task SendMessageAsync()
		{
			var message = txtMessage.Text.Trim();
			if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(_currentInterlocutor))
			{
				try
				{
					await _chatClient.SendMessageAsync(message);

					AddMessageToChat(message, true);
					txtMessage.Clear();
					LoadChatsFromDatabase(reset: true);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}", "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

			}
			else if (string.IsNullOrEmpty(_currentInterlocutor))
			{
				MessageBox.Show("Сначала выберите собеседника", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		// добавит слушатель сообщений
		#endregion

		#region Вспомогательные методы

		private DateTime ConvertToLocalTime(DateTime serverTime)
		{
			// Если время уже в локальном формате, возвращаем как есть
			if (serverTime.Kind == DateTimeKind.Local)
				return serverTime;

			// Если время в UTC, конвертируем в локальное
			if (serverTime.Kind == DateTimeKind.Utc)
				return serverTime.ToLocalTime();

			// Если Kind = Unspecified, предполагаем что это UTC или время сервера
			// Можно добавить логику, если знаете в каком формате сервер отправляет время
			return TimeZoneInfo.ConvertTimeFromUtc(
				DateTime.SpecifyKind(serverTime, DateTimeKind.Utc),
				TimeZoneInfo.Local
			);
		}
		private void MainChatForm_SizeChanged(object sender, EventArgs e)
		{
			if (this.IsHandleCreated && panelMain != null)
			{
				CenterControls();
			}
		}
		private void CenterControls()
		{
			try
			{
				if (panelMain == null) return;
				// Центрирование можно добавить при необходимости
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"CenterControls error: {ex.Message}");
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Application.Exit();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		#endregion

		private void btnLogout_Click(object sender, EventArgs e)
		{
			_authService.LogoutAsync();
			this.Close();
		}
	}
}
