using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopClient
{
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;

		public partial class MainChatForm : Form
		{
			public MainChatForm()
			{
				InitializeComponent();

				this.Load += MainChatForm_Load;
				this.SizeChanged += MainChatForm_SizeChanged;

				// Загрузка тестовых данных
				LoadSampleChats();
				LoadSampleMessages();
			}

			private void MainChatForm_Load(object sender, EventArgs e)
			{
				CenterControls();
			}

			private void MainChatForm_SizeChanged(object sender, EventArgs e)
			{
				if (this.IsHandleCreated && panelMain != null)
				{
					CenterControls();
				}
			}

			#region Функциональность чата

			private void LoadSampleChats()
			{
				// Очищаем список чатов
				chatsListPanel.Controls.Clear();

				// Тестовые чаты
				var sampleChats = new[]
				{
					new { Name = "Алексей Петров", LastMessage = "Привет! Как дела?", Time = "12:30", Unread = 2 },
					new { Name = "Мария Иванова", LastMessage = "Жду тебя в офисе", Time = "11:45", Unread = 0 },
					new { Name = "Команда разработки", LastMessage = "Собрание в 15:00", Time = "10:20", Unread = 5 },
					new { Name = "Иван Сидоров", LastMessage = "Отправил документы", Time = "09:15", Unread = 1 },
					new { Name = "Ольга Козлова", LastMessage = "Спасибо за помощь!", Time = "Вчера", Unread = 0 },
					new { Name = "Дмитрий Волков", LastMessage = "Когда сможешь созвониться?", Time = "Вчера", Unread = 3 }
				};

				int yPos = 0;
				foreach (var chat in sampleChats)
				{
					var chatItem = CreateChatItem(chat.Name, chat.LastMessage, chat.Time, chat.Unread, yPos);
					chatsListPanel.Controls.Add(chatItem);
					yPos += 80;
				}
			}

			private Panel CreateChatItem(string name, string lastMessage, string time, int unread, int yPos)
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
				lblName.Text = name;
				lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
				lblName.ForeColor = Color.Black;
				lblName.AutoSize = true;
				lblName.Location = new Point(70, 15);
				panel.Controls.Add(lblName);

				// Последнее сообщение
				var lblLastMessage = new Label();
				lblLastMessage.Text = lastMessage;
				lblLastMessage.Font = new Font("Segoe UI", 10, FontStyle.Regular);
				lblLastMessage.ForeColor = Color.Gray;
				lblLastMessage.AutoSize = true;
				lblLastMessage.Location = new Point(70, 40);
				panel.Controls.Add(lblLastMessage);

				// Время
				var lblTime = new Label();
				lblTime.Text = time;
				lblTime.Font = new Font("Segoe UI", 9, FontStyle.Regular);
				lblTime.ForeColor = Color.Gray;
				lblTime.AutoSize = true;
				lblTime.Location = new Point(300, 15);
				lblTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				panel.Controls.Add(lblTime);

				// Счетчик непрочитанных
				if (unread > 0)
				{
					var lblUnread = new Label();
					lblUnread.Text = unread.ToString();
					lblUnread.Font = new Font("Segoe UI", 9, FontStyle.Bold);
					lblUnread.ForeColor = Color.White;
					lblUnread.BackColor = Color.FromArgb(86, 130, 163);
					lblUnread.Size = new Size(20, 20);
					lblUnread.Location = new Point(310, 40);
					lblUnread.TextAlign = ContentAlignment.MiddleCenter;
					lblUnread.Anchor = AnchorStyles.Top | AnchorStyles.Right;
					panel.Controls.Add(lblUnread);
				}

				// Обработчик клика
				panel.Click += (s, e) => SelectChat(name);

				return panel;
			}

			private void LoadSampleMessages()
			{
				messagesPanel.Controls.Clear();

				// Тестовые сообщения
				var messages = new[]
				{
					new { Text = "Привет! Как твои дела?", IsMyMessage = false, Time = "12:25" },
					new { Text = "Привет! Все отлично, работаю над новым проектом. А у тебя как?", IsMyMessage = true, Time = "12:26" },
					new { Text = "Тоже все хорошо. Хотел обсудить новый дизайн интерфейса", IsMyMessage = false, Time = "12:27" },
					new { Text = "Конечно! Отправляй файлы, посмотрю", IsMyMessage = true, Time = "12:28" },
					new { Text = "Уже отправил на почту. Посмотри, когда будет время", IsMyMessage = false, Time = "12:30" }
				};

				int yPos = 10;
				foreach (var msg in messages)
				{
					var messageControl = CreateMessageControl(msg.Text, msg.IsMyMessage, msg.Time, yPos);
					messagesPanel.Controls.Add(messageControl);
					yPos += messageControl.Height + 5;
				}

				// Прокрутка вниз
				if (messagesPanel.Controls.Count > 0)
				{
					messagesPanel.ScrollControlIntoView(messagesPanel.Controls[messagesPanel.Controls.Count - 1]);
				}
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

			private void SelectChat(string chatName)
			{
				// Обновляем заголовок чата
				chatHeaderPanel.Controls.Clear();

				var lblChatName = new Label();
				lblChatName.Text = chatName;
				lblChatName.Font = new Font("Segoe UI", 14, FontStyle.Bold);
				lblChatName.ForeColor = Color.Black;
				lblChatName.AutoSize = true;
				lblChatName.Location = new Point(15, 18);
				chatHeaderPanel.Controls.Add(lblChatName);

				// Загружаем сообщения для выбранного чата
				LoadSampleMessages();
			}

			private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter && !e.Shift)
				{
					e.Handled = true;
					e.SuppressKeyPress = true;
					SendMessage();
				}
			}

			private void BtnSend_Click(object sender, EventArgs e)
			{
				SendMessage();
			}

			private void SendMessage()
			{
				var message = txtMessage.Text.Trim();
				if (!string.IsNullOrEmpty(message))
				{
					// TODO: Отправка сообщения через API
					AddMessageToChat(message, true);
					txtMessage.Clear();
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

			#endregion

			#region Вспомогательные методы

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
