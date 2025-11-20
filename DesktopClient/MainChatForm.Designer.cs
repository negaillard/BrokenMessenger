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
			splitContainer.SplitterWidth = 0;
			splitContainer.TabIndex = 0;
			// 
			// chatsListPanel
			// 
			chatsListPanel.AutoScroll = true;
			chatsListPanel.BackColor = Color.White;
			chatsListPanel.Dock = DockStyle.Fill;
			chatsListPanel.Location = new Point(0, 110);
			chatsListPanel.Name = "chatsListPanel";
			chatsListPanel.Size = new Size(350, 690);
			chatsListPanel.TabIndex = 0;
			// 
			// searchPanel
			// 
			searchPanel.BackColor = Color.White;
			searchPanel.Controls.Add(txtSearch);
			searchPanel.Dock = DockStyle.Top;
			searchPanel.Location = new Point(0, 60);
			searchPanel.Name = "searchPanel";
			searchPanel.Size = new Size(350, 50);
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
			txtSearch.Size = new Size(320, 30);
			txtSearch.TabIndex = 0;
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
			messagesPanel.Size = new Size(849, 670);
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
			chatHeaderPanel.Size = new Size(849, 60);
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
			inputPanel.Dock = DockStyle.Bottom;
			inputPanel.Height = 100;

			// 2 колонки: сообщение + кнопка
			inputPanel.ColumnCount = 2;
			inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85F)); // сообщение
			inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); // кнопка

			inputPanel.RowCount = 1;
			inputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

			// Добавляем контролы
			inputPanel.Controls.Add(txtMessage, 0, 0);
			inputPanel.Controls.Add(btnSend, 1, 0);
			// 
			// txtMessage
			// 
			//txtMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			txtMessage.BorderStyle = BorderStyle.FixedSingle;
			txtMessage.Font = new Font("Segoe UI", 11F);
			//txtMessage.Location = new Point(4, 15);
			txtMessage.Multiline = true;
			txtMessage.Name = "txtMessage";
			//txtMessage.Size = new Size(776, 40);
			txtMessage.TabIndex = 0;
			txtMessage.KeyDown += TxtMessage_KeyDown;
			txtMessage.Dock = DockStyle.Fill;
			txtMessage.Margin = new Padding(10, 15, 5, 15);
			
			txtMessage.ScrollBars = ScrollBars.Vertical;
			txtMessage.Height = 60; // Фиксированная высота с прокруткой
			txtMessage.Dock = DockStyle.Fill;
			// 
			// btnSend
			// 
			btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnSend.BackColor = Color.FromArgb(86, 130, 163);
			btnSend.Cursor = Cursors.Hand;
			btnSend.FlatAppearance.BorderSize = 0;
			btnSend.FlatStyle = FlatStyle.Flat;
			btnSend.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			btnSend.ForeColor = Color.White;
			//btnSend.Location = new Point(786, 15);
			btnSend.Name = "btnSend";
			//btnSend.Size = new Size(50, 40);
			btnSend.TabIndex = 1;
			btnSend.Text = "➤";
			btnSend.UseVisualStyleBackColor = false;
			btnSend.Click += BtnSend_Click;
			btnSend.Dock = DockStyle.Fill;
			btnSend.Margin = new Padding(5, 15, 10, 15);
			// 
			// MainChatForm
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1200, 800);
			Controls.Add(panelMain);
			Name = "MainChatForm";
			Text = "Secure Chat - Мессенджер";
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