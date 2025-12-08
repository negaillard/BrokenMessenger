using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models.Search;
using Models.View;
using Models.Binding;

namespace DesktopClient
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

		public partial class MainChatForm : Form
		{
			private ChatClient _chatClient;
			private string _currentInterlocutor;

			public MainChatForm(string username)
			{
				InitializeComponent();
				_chatClient = new ChatClient(username);
				_chatClient.StartReceiving();

				this.Load += MainChatForm_Load;
				this.SizeChanged += MainChatForm_SizeChanged;

				LoadChatsFromDatabase();	
			}

			private void MainChatForm_Load(object sender, EventArgs e)
			{
				CenterControls();
			}

		#region Отрисовка
		private async Task<Panel> CreateChatItemAsync(dynamic chat, int yPos)
		{
			var panel = new Panel();
			panel.Size = new Size(350, 80);
			panel.Location = new Point(0, yPos);
			panel.BackColor = Color.White;
			panel.Cursor = Cursors.Hand;
			panel.BorderStyle = BorderStyle.FixedSingle;

			// Аватар
			var avatar = new Panel();
			avatar.Size = new Size(50, 50);
			avatar.Location = new Point(10, 15);
			avatar.BackColor = Color.FromArgb(86, 130, 163);
			avatar.BorderStyle = BorderStyle.FixedSingle;
			panel.Controls.Add(avatar);

			// Имя
			var lblName = new Label();
			lblName.Text = chat.Interlocutor;
			lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
			lblName.ForeColor = Color.Black;
			lblName.AutoSize = true;
			lblName.Location = new Point(70, 15);
			panel.Controls.Add(lblName);

			var lastMessage = await GetLastMessagePreview(chat.Interlocutor);

			// Последнее сообщение
			var lblLastMessage = new Label();
			lblLastMessage.Text = lastMessage.Item1;
			lblLastMessage.Font = new Font("Segoe UI", 10, FontStyle.Regular);
			lblLastMessage.ForeColor = Color.Gray;
			lblLastMessage.AutoSize = true;
			lblLastMessage.Location = new Point(70, 40);
			panel.Controls.Add(lblLastMessage);

			// Время
			var lblTime = new Label();
			lblTime.Text =lastMessage.Item2.ToString();
			lblTime.Font = new Font("Segoe UI", 9, FontStyle.Regular);
			lblTime.ForeColor = Color.Gray;
			lblTime.AutoSize = true;
			lblTime.Location = new Point(300, 15);
			lblTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			panel.Controls.Add(lblTime);

			// Обработчик клика
			panel.Click += async (s, e) => await SelectChatAsync(chat.Interlocutor);

			return panel;
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

		private Panel CreateMessageControl(string text, bool isMyMessage, string time, int yPos)
		{
			var panel = new Panel();
			panel.AutoSize = true;
			panel.MaximumSize = new Size(400, 0);
			panel.BackColor = Color.Transparent;

			// Текст сообщения
			var lblText = new Label();
			lblText.Text = text;
			lblText.Font = new Font("Segoe UI", 11, FontStyle.Regular);
			lblText.ForeColor = Color.Black;
			lblText.BackColor = isMyMessage ? Color.FromArgb(220, 248, 198) : Color.White;
			lblText.AutoSize = true;
			lblText.MaximumSize = new Size(350, 0);
			lblText.Padding = new Padding(10);
			lblText.BorderStyle = BorderStyle.FixedSingle;
			lblText.Location = new Point(0, 0);

			// Время
			var lblTime = new Label();
			lblTime.Text = time;
			lblTime.Font = new Font("Segoe UI", 8, FontStyle.Regular);
			lblTime.ForeColor = Color.Gray;
			lblTime.AutoSize = true;
			lblTime.Location = new Point(lblText.Right - 40, lblText.Bottom + 2);

			panel.Controls.Add(lblText);
			panel.Controls.Add(lblTime);

			panel.Size = new Size(lblText.Width, lblText.Height + 20);

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
		// переделать, надо чтобы мы загружали сообщения из чата этим методом
		private async Task LoadChatsFromDatabase()
			{
				try
				{
					chatsListPanel.Controls.Clear();

					var chats = await _chatClient._chatLogic.ReadListAsync(null);

					if (chats == null || !chats.Any()) {
						var noChatsLabel = new Label();
						noChatsLabel.Text = "Пока нет чатов...";
						noChatsLabel.Font = new Font("Segoe UI", 11, FontStyle.Regular);
						noChatsLabel.ForeColor = Color.Gray;
						noChatsLabel.AutoSize = true;
						noChatsLabel.Location = new Point(50, 50);
						chatsListPanel.Controls.Add(noChatsLabel);
						return;
					}

					int yPos = 0;
					foreach(var chat in chats)
					{
						var chatItem = await CreateChatItemAsync(chat, yPos);
						chatsListPanel.Controls.Add(chatItem);
						yPos += 80;
					}
				}
				catch (Exception ex) {
					MessageBox.Show($"Ошибка загрузки чатов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			

			private async Task<(string, DateTime)> GetLastMessagePreview(string interlocutor)
			{
				try
				{
					var chat = await _chatClient._chatLogic.ReadElementAsync(new Models.Search.ChatSearchModel
					{
						CurrentUser = _chatClient.GetUsername(),
						Interlocutor = interlocutor
					});
					if (chat != null) {
						var messages = await _chatClient._messageLogic.ReadListAsync(new MessageSearchModel
						{
							ChatId = chat.Id,
						});

						var lastMessage = messages?.OrderByDescending(m => m.Timestamp).FirstOrDefault();
						if (lastMessage != null) {
							return (lastMessage.Content.Length > 20
								? lastMessage.Content.Substring(0, 20) + "..."
								: lastMessage.Content, lastMessage.Timestamp.ToLocalTime());
						}
					}
				}
				catch (Exception ex) 
				{
					Console.WriteLine($"Ошибка получения последнего сообщения: {ex.Message}");
				}

				return ("Нет сообщений", DateTime.Now);
			}

			private async Task LoadMessagesAsync(string interlocutor)
			{
				try
				{
					messagesPanel.Controls.Clear();
					_currentInterlocutor = interlocutor;

					await _chatClient.SetCurrentInterlocutorAsync(interlocutor);

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
					var messages = await _chatClient._messageLogic.ReadListAsync(new MessageSearchModel
					{
						ChatId = chat.Id,
					});

					if (messages != null && messages.Any())
					{
						int yPos = 10;
						foreach (var msg in messages.OrderBy(m => m.Timestamp))
						{
							var messageControl = CreateMessageControl(
								msg.Content,
								msg.Sender == _chatClient.GetUsername(),
								msg.Timestamp.ToString("HH:mm"),
								yPos
							);
							messagesPanel.Controls.Add(messageControl);
							yPos += messageControl.Height + 5;
						}
					}

					if (messagesPanel.Controls.Count > 0)
					{
						messagesPanel.ScrollControlIntoView(messagesPanel.Controls[messagesPanel.Controls.Count - 1]);
					}
				}
				catch (Exception ex) {
					MessageBox.Show($"Ошибка загрузки сообщений: {ex.Message}", "Ошибка",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			

			private async Task SelectChatAsync(string interlocutor)
			{
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
					}
					catch (Exception ex) {
						MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}", "Ошибка",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else if(string.IsNullOrEmpty(_currentInterlocutor)){
					MessageBox.Show("Сначала выберите собеседника", "Информация",MessageBoxButtons.OK, MessageBoxIcon.Information);
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


		// добавит слушатель сообщений
		#endregion

		#region Вспомогательные методы
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

		}
	}
