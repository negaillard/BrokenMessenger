namespace DesktopClient
{
	partial class WelcomeForm
	{
		private System.ComponentModel.IContainer components = null;
		private Panel panelMain;
		private Label lblTitle;
		private Button btnLogin;
		private Button btnRegister;
		

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
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1200, 800);
			this.Text = "Secure Chat - Добро пожаловать";
			this.WindowState = FormWindowState.Maximized;
			this.FormBorderStyle = FormBorderStyle.None;

			CreateControls();
		}

		private void CreateControls()
		{
			// Основная панель
			panelMain = new Panel();
			panelMain.Dock = DockStyle.Fill;
			panelMain.BackColor = Color.FromArgb(235, 245, 251);
			this.Controls.Add(panelMain);

			// Заголовок
			lblTitle = new Label();
			lblTitle.Text = "Мессенджер Олег";
			lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
			lblTitle.ForeColor = Color.FromArgb(86, 130, 163);
			lblTitle.AutoSize = true;
			lblTitle.Location = new Point(0, 100);
			lblTitle.Anchor = AnchorStyles.Top;
			lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			panelMain.Controls.Add(lblTitle);

			// Панель для кнопок
			var buttonPanel = new Panel();
			buttonPanel.Size = new Size(400, 200);
			buttonPanel.Location = new Point(0, 0); // Временно, потом центрируем
			panelMain.Controls.Add(buttonPanel);



			// Кнопка Войти
			btnLogin = new Button();
			btnLogin.Text = "Войти";
			btnLogin.Font = new Font("Segoe UI", 14, FontStyle.Bold);
			btnLogin.ForeColor = Color.White;
			btnLogin.BackColor = Color.FromArgb(86, 130, 163);
			btnLogin.Size = new Size(300, 50);
			btnLogin.Location = new Point(50, 30);
			btnLogin.FlatStyle = FlatStyle.Flat;
			btnLogin.FlatAppearance.BorderSize = 0;
			btnLogin.Cursor = Cursors.Hand;
			btnLogin.Click += BtnLogin_Click;
			buttonPanel.Controls.Add(btnLogin);

			// Кнопка Зарегистрироваться
			btnRegister = new Button();
			btnRegister.Text = "Зарегистрироваться";
			btnRegister.Font = new Font("Segoe UI", 14, FontStyle.Bold);
			btnRegister.ForeColor = Color.FromArgb(86, 130, 163);
			btnRegister.BackColor = Color.White;
			btnRegister.Size = new Size(300, 50);
			btnRegister.Location = new Point(50, 100);
			btnRegister.FlatStyle = FlatStyle.Flat;
			btnRegister.FlatAppearance.BorderSize = 1;
			btnRegister.FlatAppearance.BorderColor = Color.FromArgb(86, 130, 163);
			btnRegister.Cursor = Cursors.Hand;
			btnRegister.Click += BtnRegister_Click;
			buttonPanel.Controls.Add(btnRegister);

			// Кнопка выхода
			var btnExit = new Button();
			btnExit.Text = "✕";
			btnExit.Font = new Font("Segoe UI", 12, FontStyle.Bold);
			btnExit.ForeColor = Color.Gray;
			btnExit.BackColor = Color.Transparent;
			btnExit.Size = new Size(40, 40);
			btnExit.Location = new Point(panelMain.Width - 50, 10);
			btnExit.FlatStyle = FlatStyle.Flat;
			btnExit.FlatAppearance.BorderSize = 0;
			btnExit.Cursor = Cursors.Hand;
			btnExit.Click += (s, e) => Application.Exit();
			panelMain.Controls.Add(btnExit);

			// Кнопка сворачивания
			var btnMinimize = new Button();
			btnMinimize.Text = "─";
			btnMinimize.Font = new Font("Segoe UI", 12, FontStyle.Bold);
			btnMinimize.ForeColor = Color.Gray;
			btnMinimize.BackColor = Color.Transparent;
			btnMinimize.Size = new Size(40, 40);
			btnMinimize.Location = new Point(panelMain.Width - 90, 10);
			btnMinimize.FlatStyle = FlatStyle.Flat;
			btnMinimize.FlatAppearance.BorderSize = 0;
			btnMinimize.Cursor = Cursors.Hand;
			btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
			panelMain.Controls.Add(btnMinimize);

			// Версия приложения
			var lblVersion = new Label();
			lblVersion.Text = "v1.0.0";
			lblVersion.Font = new Font("Segoe UI", 8, FontStyle.Regular);
			lblVersion.ForeColor = Color.LightGray;
			lblVersion.AutoSize = true;
			lblVersion.Location = new Point(20, panelMain.Height - 30);
			panelMain.Controls.Add(lblVersion);
		}
	}
}