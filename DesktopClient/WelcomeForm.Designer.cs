namespace DesktopClient
{
	partial class WelcomeForm
	{
		private System.ComponentModel.IContainer components = null;
		private Panel panelMain;
		private Label lblTitle;
		private Button btnLogin;
		private Button btnRegister;
		private Panel buttonPanel;
		private Label lblVersion;
		

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
			lblTitle = new Label();
			buttonPanel = new Panel();
			btnLogin = new Button();
			btnRegister = new Button();
			lblVersion = new Label();
			panelMain.SuspendLayout();
			buttonPanel.SuspendLayout();
			SuspendLayout();
			// 
			// panelMain
			// 
			panelMain.BackColor = Color.FromArgb(235, 245, 251);
			panelMain.Controls.Add(lblTitle);
			panelMain.Controls.Add(buttonPanel);
			panelMain.Controls.Add(lblVersion);
			panelMain.Dock = DockStyle.Fill;
			panelMain.Location = new Point(0, 0);
			panelMain.Name = "panelMain";
			panelMain.Size = new Size(623, 395);
			panelMain.TabIndex = 0;
			// 
			// lblTitle
			// 
			lblTitle.Anchor = AnchorStyles.Top;
			lblTitle.AutoSize = true;
			lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
			lblTitle.ForeColor = Color.FromArgb(86, 130, 163);
			lblTitle.Location = new Point(0, 100);
			lblTitle.Name = "lblTitle";
			lblTitle.Size = new Size(375, 54);
			lblTitle.TabIndex = 0;
			lblTitle.Text = "Мессенджер Олег";
			lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// buttonPanel
			// 
			buttonPanel.Controls.Add(btnLogin);
			buttonPanel.Controls.Add(btnRegister);
			buttonPanel.Location = new Point(0, 0);
			buttonPanel.Name = "buttonPanel";
			buttonPanel.Size = new Size(400, 200);
			buttonPanel.TabIndex = 1;
			// 
			// btnLogin
			// 
			btnLogin.BackColor = Color.FromArgb(86, 130, 163);
			btnLogin.Cursor = Cursors.Hand;
			btnLogin.FlatAppearance.BorderSize = 0;
			btnLogin.FlatStyle = FlatStyle.Flat;
			btnLogin.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			btnLogin.ForeColor = Color.White;
			btnLogin.Location = new Point(50, 30);
			btnLogin.Name = "btnLogin";
			btnLogin.Size = new Size(300, 50);
			btnLogin.TabIndex = 0;
			btnLogin.Text = "Войти";
			btnLogin.UseVisualStyleBackColor = false;
			btnLogin.Click += BtnLogin_Click;
			// 
			// btnRegister
			// 
			btnRegister.BackColor = Color.White;
			btnRegister.Cursor = Cursors.Hand;
			btnRegister.FlatAppearance.BorderColor = Color.FromArgb(86, 130, 163);
			btnRegister.FlatStyle = FlatStyle.Flat;
			btnRegister.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			btnRegister.ForeColor = Color.FromArgb(86, 130, 163);
			btnRegister.Location = new Point(50, 100);
			btnRegister.Name = "btnRegister";
			btnRegister.Size = new Size(300, 50);
			btnRegister.TabIndex = 1;
			btnRegister.Text = "Зарегистрироваться";
			btnRegister.UseVisualStyleBackColor = false;
			btnRegister.Click += BtnRegister_Click;
			// 
			// lblVersion
			// 
			lblVersion.AutoSize = true;
			lblVersion.Font = new Font("Segoe UI", 8F);
			lblVersion.ForeColor = Color.LightGray;
			lblVersion.Location = new Point(20, 100);
			lblVersion.Name = "lblVersion";
			lblVersion.Size = new Size(46, 19);
			lblVersion.TabIndex = 4;
			lblVersion.Text = "v1.0.0";
			// 
			// WelcomeForm
			// 
			ClientSize = new Size(623, 395);
			Controls.Add(panelMain);
			Name = "WelcomeForm";
			Text = "Мессенджер Олег";
			WindowState = FormWindowState.Maximized;
			MaximizeBox = false;
			MinimizeBox = true; // Оставляем возможность сворачивания
			ControlBox = true;
			AutoScaleMode = AutoScaleMode.None;
			panelMain.ResumeLayout(false);
			panelMain.PerformLayout();
			buttonPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		private void BtnExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void BtnMinimize_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}
	}
}