namespace DesktopClient
{
	namespace DesktopClient
	{
		partial class MainChatForm
		{
			private System.ComponentModel.IContainer components = null;
			private Panel panelMain;
			private SplitContainer splitContainer;
			private Panel chatsListPanel;
			private Panel chatHeaderPanel;
			private Panel messagesPanel;
			private TextBox txtSearch;
			private TextBox txtMessage;
			private Button btnSend;

			protected override void Dispose(bool disposing)
			{
				if (disposing && (components != null))
				{
					components.Dispose();
				}
				base.Dispose(disposing);
			}

			private void InitializeComponent()
			{
				this.components = new System.ComponentModel.Container();
				this.SuspendLayout();

				// panelMain
				this.panelMain = new Panel();
				this.panelMain.Dock = DockStyle.Fill;
				this.panelMain.BackColor = Color.FromArgb(235, 245, 251);

				// splitContainer - ФИКСИРОВАННАЯ ширина
				this.splitContainer = new SplitContainer();
				this.splitContainer.Dock = DockStyle.Fill;
				this.splitContainer.SplitterDistance = 350;
				this.splitContainer.SplitterWidth = 1;
				this.splitContainer.BackColor = Color.LightGray;
				this.splitContainer.Panel1.BackColor = Color.White;
				this.splitContainer.Panel2.BackColor = Color.FromArgb(240, 242, 245);
				this.splitContainer.IsSplitterFixed = true; // ✅ Фиксируем разделитель
				this.splitContainer.FixedPanel = FixedPanel.Panel1; // ✅ Фиксируем левую панель

				// Панель списка чатов (Panel1)
				// Заголовок списка чатов
				var chatsHeader = new Panel();
				chatsHeader.Dock = DockStyle.Top;
				chatsHeader.Height = 60;
				chatsHeader.BackColor = Color.FromArgb(86, 130, 163);

				var lblChatsTitle = new Label();
				lblChatsTitle.Text = "Чаты";
				lblChatsTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
				lblChatsTitle.ForeColor = Color.White;
				lblChatsTitle.AutoSize = true;
				lblChatsTitle.Location = new Point(15, 18);
				chatsHeader.Controls.Add(lblChatsTitle);

				// Поле поиска
				var searchPanel = new Panel();
				searchPanel.Dock = DockStyle.Top;
				searchPanel.Height = 50;
				searchPanel.BackColor = Color.White;

				this.txtSearch = new TextBox();
				this.txtSearch.PlaceholderText = "Поиск...";
				this.txtSearch.Font = new Font("Segoe UI", 10, FontStyle.Regular);
				this.txtSearch.Size = new Size(320, 30);
				this.txtSearch.Location = new Point(15, 10);
				this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
				this.txtSearch.BackColor = Color.FromArgb(240, 242, 245);
				searchPanel.Controls.Add(this.txtSearch);

				// Список чатов - БЕЗ горизонтального скролла
				this.chatsListPanel = new Panel();
				this.chatsListPanel.Dock = DockStyle.Fill;
				this.chatsListPanel.BackColor = Color.White;
				this.chatsListPanel.AutoScroll = true;
				this.chatsListPanel.HorizontalScroll.Enabled = false; // ✅ Отключаем горизонтальный скролл
				this.chatsListPanel.HorizontalScroll.Visible = false;
				this.chatsListPanel.AutoScrollMargin = new Size(0, 0);

				this.splitContainer.Panel1.Controls.Add(this.chatsListPanel);
				this.splitContainer.Panel1.Controls.Add(searchPanel);
				this.splitContainer.Panel1.Controls.Add(chatsHeader);

				// Панель активного чата (Panel2)
				// Заголовок активного чата
				this.chatHeaderPanel = new Panel();
				this.chatHeaderPanel.Dock = DockStyle.Top;
				this.chatHeaderPanel.Height = 60;
				this.chatHeaderPanel.BackColor = Color.White;
				this.chatHeaderPanel.BorderStyle = BorderStyle.FixedSingle;

				// Область сообщений - БЕЗ горизонтального скролла
				this.messagesPanel = new Panel();
				this.messagesPanel.Dock = DockStyle.Fill;
				this.messagesPanel.BackColor = Color.FromArgb(240, 242, 245);
				this.messagesPanel.AutoScroll = true;
				this.messagesPanel.HorizontalScroll.Enabled = false; // ✅ Отключаем горизонтальный скролл
				this.messagesPanel.HorizontalScroll.Visible = false;
				this.messagesPanel.AutoScrollMargin = new Size(0, 0);

				// Панель ввода сообщения
				var inputPanel = new Panel();
				inputPanel.Dock = DockStyle.Bottom;
				inputPanel.Height = 70;
				inputPanel.BackColor = Color.White;
				inputPanel.BorderStyle = BorderStyle.FixedSingle;

				this.txtMessage = new TextBox();
				this.txtMessage.Multiline = true;
				this.txtMessage.Font = new Font("Segoe UI", 11, FontStyle.Regular);
				this.txtMessage.Size = new Size(650, 40);
				this.txtMessage.Location = new Point(15, 15);
				this.txtMessage.BorderStyle = BorderStyle.FixedSingle;
				this.txtMessage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
				this.txtMessage.KeyDown += TxtMessage_KeyDown;

				this.btnSend = new Button();
				this.btnSend.Text = "➤";
				this.btnSend.Font = new Font("Segoe UI", 12, FontStyle.Bold);
				this.btnSend.ForeColor = Color.White;
				this.btnSend.BackColor = Color.FromArgb(86, 130, 163);
				this.btnSend.Size = new Size(50, 40);
				this.btnSend.Location = new Point(675, 15);
				this.btnSend.FlatStyle = FlatStyle.Flat;
				this.btnSend.FlatAppearance.BorderSize = 0;
				this.btnSend.Cursor = Cursors.Hand;
				this.btnSend.Click += BtnSend_Click;

				inputPanel.Controls.Add(this.txtMessage);
				inputPanel.Controls.Add(this.btnSend);

				this.splitContainer.Panel2.Controls.Add(this.messagesPanel);
				this.splitContainer.Panel2.Controls.Add(this.chatHeaderPanel);
				this.splitContainer.Panel2.Controls.Add(inputPanel);

				this.panelMain.Controls.Add(this.splitContainer);

				// Кнопка выхода
				var btnExit = new Button();
				btnExit.Text = "✕";
				btnExit.Font = new Font("Segoe UI", 12, FontStyle.Bold);
				btnExit.ForeColor = Color.Gray;
				btnExit.BackColor = Color.Transparent;
				btnExit.Size = new Size(40, 40);
				btnExit.Location = new Point(1150, 10);
				btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				btnExit.FlatStyle = FlatStyle.Flat;
				btnExit.FlatAppearance.BorderSize = 0;
				btnExit.Cursor = Cursors.Hand;
				btnExit.Click += (s, e) => Application.Exit();
				this.panelMain.Controls.Add(btnExit);

				// Кнопка сворачивания
				var btnMinimize = new Button();
				btnMinimize.Text = "─";
				btnMinimize.Font = new Font("Segoe UI", 12, FontStyle.Bold);
				btnMinimize.ForeColor = Color.Gray;
				btnMinimize.BackColor = Color.Transparent;
				btnMinimize.Size = new Size(40, 40);
				btnMinimize.Location = new Point(1100, 10);
				btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				btnMinimize.FlatStyle = FlatStyle.Flat;
				btnMinimize.FlatAppearance.BorderSize = 0;
				btnMinimize.Cursor = Cursors.Hand;
				btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
				this.panelMain.Controls.Add(btnMinimize);

				this.Controls.Add(this.panelMain);

				// MainChatForm
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(1200, 800);
				this.Text = "Secure Chat - Мессенджер";
				this.WindowState = FormWindowState.Maximized;
				this.FormBorderStyle = FormBorderStyle.None;

				this.ResumeLayout(false);
			}
		}
	}
}