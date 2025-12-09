namespace DesktopClient
{
		partial class MainChatForm
		{
			private System.ComponentModel.IContainer components = null;
			private Panel panelMain;
			private Panel chatsHeader;
			private Panel searchPanel;
			private TableLayoutPanel inputPanel;
			private SplitContainer splitContainer;
			private Panel chatsListPanel;
			private Panel chatHeaderPanel;
			private Panel messagesPanel;
			private TextBox txtSearch;
			private TextBox txtMessage;
			private Button btnSend;
			private Label lblChatsTitle;
			private Panel userMenuPanel;
			private Button btnUserMenu;
			private Panel dropdownMenu;
			private Button btnProfile;
			private Button btnLogout;
			private Label lblUserInfo;
			private Panel usersSearchPanel;
			private Button btnSearch;

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
			panelMain = new Panel();
			splitContainer = new SplitContainer();
			chatsListPanel = new Panel();
			searchPanel = new Panel();
			txtSearch = new TextBox();
			usersSearchPanel = new Panel();
			btnSearch = new Button();
			chatsHeader = new Panel();
			messagesPanel = new Panel();
			chatHeaderPanel = new Panel();
			lblChatsTitle = new Label();
			inputPanel = new TableLayoutPanel();
			txtMessage = new TextBox();
			btnSend = new Button();
			panelMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			searchPanel.SuspendLayout();
			chatHeaderPanel.SuspendLayout();
			inputPanel.SuspendLayout();
			SuspendLayout();
			// 
			// panelMain
			// 
			panelMain.BackColor = Color.FromArgb(235, 245, 251);
			panelMain.Controls.Add(splitContainer);
			panelMain.Dock = DockStyle.Fill;
			panelMain.Location = new Point(0, 0);
			panelMain.Name = "panelMain";
			panelMain.Size = new Size(1200, 800);
			panelMain.TabIndex = 0;
			// 
			// splitContainer
			// 
			splitContainer.BackColor = Color.LightGray;
			splitContainer.Dock = DockStyle.Fill;
			splitContainer.FixedPanel = FixedPanel.Panel1;
			splitContainer.IsSplitterFixed = true;
			splitContainer.Location = new Point(0, 0);
			splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			splitContainer.Panel1.BackColor = Color.White;
			splitContainer.Panel1.Controls.Add(chatsListPanel);
			splitContainer.Panel1.Controls.Add(searchPanel);
			splitContainer.Panel1.Controls.Add(chatsHeader);
			splitContainer.Panel1MinSize = 350;
			// 
			// splitContainer.Panel2
			// 
			splitContainer.Panel2.BackColor = Color.FromArgb(240, 242, 245);
			splitContainer.Panel2.Controls.Add(messagesPanel);
			splitContainer.Panel2.Controls.Add(chatHeaderPanel);
			splitContainer.Panel2.Controls.Add(inputPanel);
			splitContainer.Size = new Size(1200, 800);
			splitContainer.SplitterDistance = 350;
			splitContainer.TabIndex = 0;
			// 
			// chatsListPanel
			// 
			chatsListPanel.AutoScroll = true;
			chatsListPanel.BackColor = Color.White;
			chatsListPanel.Dock = DockStyle.Fill;
			chatsListPanel.Location = new Point(0, 310);
			chatsListPanel.Name = "chatsListPanel";
			chatsListPanel.Size = new Size(350, 490);
			chatsListPanel.TabIndex = 0;
			chatsListPanel.Scroll += ChatsListPanel_Scroll;
			// 
			// searchPanel
			// 
			searchPanel.BackColor = Color.White;
			searchPanel.Controls.Add(txtSearch);
			searchPanel.Controls.Add(usersSearchPanel);
			searchPanel.Controls.Add(btnSearch);
			searchPanel.Dock = DockStyle.Top;
			searchPanel.Location = new Point(0, 60);
			searchPanel.Name = "searchPanel";
			searchPanel.Size = new Size(350, 250);
			searchPanel.TabIndex = 1;
			// 
			// txtSearch
			// 
			txtSearch.BackColor = Color.FromArgb(240, 242, 245);
			txtSearch.BorderStyle = BorderStyle.FixedSingle;
			txtSearch.Font = new Font("Segoe UI", 10F);
			txtSearch.Location = new Point(15, 10);
			txtSearch.Name = "txtSearch";
			txtSearch.PlaceholderText = "Поиск...";
			txtSearch.Size = new Size(330, 30);
			txtSearch.TabIndex = 0;
			// 
			// usersSearchPanel
			// 
			usersSearchPanel.AutoScroll = true;
			usersSearchPanel.BackColor = Color.WhiteSmoke;
			usersSearchPanel.Location = new Point(0, 50);
			usersSearchPanel.Name = "usersSearchPanel";
			usersSearchPanel.Size = new Size(400, 200);
			usersSearchPanel.TabIndex = 1;
			usersSearchPanel.Visible = false;
			usersSearchPanel.Visible = false;
			// 
			// btnSearch
			// 
			btnSearch.BackColor = Color.FromArgb(86, 130, 163);
			btnSearch.Cursor = Cursors.Hand;
			btnSearch.FlatAppearance.BorderSize = 0;
			btnSearch.FlatStyle = FlatStyle.Flat;
			btnSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
			btnSearch.ForeColor = Color.White;
			btnSearch.Location = new Point(350, 10);
			btnSearch.Name = "btnSearch";
			btnSearch.Size = new Size(40, 35);
			btnSearch.TabIndex = 2;
			btnSearch.Text = "🔍";
			btnSearch.UseVisualStyleBackColor = false;
			btnSearch.Click += BtnSearch_Click;
			// 
			// chatsHeader
			// 
			chatsHeader.BackColor = Color.FromArgb(86, 130, 163);
			chatsHeader.Dock = DockStyle.Top;
			chatsHeader.Location = new Point(0, 0);
			chatsHeader.Name = "chatsHeader";
			chatsHeader.Size = new Size(350, 60);
			chatsHeader.TabIndex = 2;
			// 
			// messagesPanel
			// 
			messagesPanel.AutoScroll = true;
			messagesPanel.BackColor = Color.FromArgb(240, 242, 245);
			messagesPanel.Dock = DockStyle.Fill;
			messagesPanel.Location = new Point(0, 60);
			messagesPanel.Name = "messagesPanel";
			messagesPanel.Size = new Size(846, 640);
			messagesPanel.TabIndex = 0;
			// 
			// chatHeaderPanel
			// 
			chatHeaderPanel.BackColor = Color.White;
			chatHeaderPanel.BorderStyle = BorderStyle.FixedSingle;
			chatHeaderPanel.Controls.Add(lblChatsTitle);
			chatHeaderPanel.Dock = DockStyle.Top;
			chatHeaderPanel.Location = new Point(0, 0);
			chatHeaderPanel.Name = "chatHeaderPanel";
			chatHeaderPanel.Size = new Size(846, 60);
			chatHeaderPanel.TabIndex = 1;
			// 
			// lblChatsTitle
			// 
			lblChatsTitle.AutoSize = true;
			lblChatsTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
			lblChatsTitle.ForeColor = Color.White;
			lblChatsTitle.Location = new Point(-88, 21);
			lblChatsTitle.Name = "lblChatsTitle";
			lblChatsTitle.Size = new Size(86, 37);
			lblChatsTitle.TabIndex = 0;
			lblChatsTitle.Text = "Чаты";
			// 
			// inputPanel
			// 
			inputPanel.BackColor = Color.White;
			inputPanel.BorderStyle = BorderStyle.FixedSingle;
			inputPanel.ColumnCount = 2;
			inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85F));
			inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
			inputPanel.Controls.Add(txtMessage, 0, 0);
			inputPanel.Controls.Add(btnSend, 1, 0);
			inputPanel.Dock = DockStyle.Bottom;
			inputPanel.Location = new Point(0, 700);
			inputPanel.Name = "inputPanel";
			inputPanel.RowCount = 1;
			inputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			inputPanel.Size = new Size(846, 100);
			inputPanel.TabIndex = 2;
			// 
			// txtMessage
			// 
			txtMessage.BorderStyle = BorderStyle.FixedSingle;
			txtMessage.Dock = DockStyle.Fill;
			txtMessage.Font = new Font("Segoe UI", 11F);
			txtMessage.Location = new Point(10, 15);
			txtMessage.Margin = new Padding(10, 15, 5, 15);
			txtMessage.Multiline = true;
			txtMessage.Name = "txtMessage";
			txtMessage.ScrollBars = ScrollBars.Vertical;
			txtMessage.Size = new Size(702, 68);
			txtMessage.TabIndex = 0;
			txtMessage.KeyDown += TxtMessage_KeyDown;
			// 
			// btnSend
			// 
			btnSend.BackColor = Color.FromArgb(86, 130, 163);
			btnSend.Cursor = Cursors.Hand;
			btnSend.Dock = DockStyle.Fill;
			btnSend.FlatAppearance.BorderSize = 0;
			btnSend.FlatStyle = FlatStyle.Flat;
			btnSend.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			btnSend.ForeColor = Color.White;
			btnSend.Location = new Point(722, 15);
			btnSend.Margin = new Padding(5, 15, 10, 15);
			btnSend.Name = "btnSend";
			btnSend.Size = new Size(112, 68);
			btnSend.TabIndex = 1;
			btnSend.Text = "➤";
			btnSend.UseVisualStyleBackColor = false;
			btnSend.Click += BtnSend_Click;
			// 
			// MainChatForm
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1200, 800);
			Controls.Add(panelMain);
			Name = "MainChatForm";
			Text = "Мессенджер";
			WindowState = FormWindowState.Maximized;
			panelMain.ResumeLayout(false);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
			splitContainer.ResumeLayout(false);
			searchPanel.ResumeLayout(false);
			searchPanel.PerformLayout();
			chatHeaderPanel.ResumeLayout(false);
			chatHeaderPanel.PerformLayout();
			inputPanel.ResumeLayout(false);
			inputPanel.PerformLayout();
			ResumeLayout(false);
		}
	}
}