namespace DesktopClient
{
	partial class RegisterForm
	{
		private System.ComponentModel.IContainer components = null;
		private Panel panelMain;
		private TextBox txtUsername;
		private TextBox txtEmail;
		private Button btnGetCode;
		private Button btnBack;
		private Label lblTitle;
		private Label lblError;
		private Label lblUsernameError;
		private Label lblEmailError;

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
			this.Text = "Secure Chat - Регистрация";
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

			// Кнопка Назад
			btnBack = new Button();
			btnBack.Text = "← Назад";
			btnBack.Font = new Font("Segoe UI", 11, FontStyle.Regular);
			btnBack.ForeColor = Color.FromArgb(86, 130, 163);
			btnBack.BackColor = Color.Transparent;
			btnBack.Size = new Size(100, 40);
			btnBack.Location = new Point(20, 20);
			btnBack.FlatStyle = FlatStyle.Flat;
			btnBack.FlatAppearance.BorderSize = 0;
			btnBack.Cursor = Cursors.Hand;
			btnBack.Click += BtnBack_Click;
			panelMain.Controls.Add(btnBack);

			// Заголовок
			lblTitle = new Label();
			lblTitle.Text = "Регистрация";
			lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
			lblTitle.ForeColor = Color.FromArgb(86, 130, 163);
			lblTitle.AutoSize = true;
			lblTitle.Location = new Point(0, 100);
			lblTitle.Anchor = AnchorStyles.Top;
			lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			panelMain.Controls.Add(lblTitle);

			// Панель формы
			var formPanel = new Panel();
			formPanel.Size = new Size(400, 350);
			formPanel.Location = new Point(0, 0); // Временно, потом центрируем
			formPanel.BackColor = Color.White;
			formPanel.BorderStyle = BorderStyle.FixedSingle;
			formPanel.Padding = new Padding(20);
			panelMain.Controls.Add(formPanel);

			// Метка для имени пользователя
			var lblUsername = new Label();
			lblUsername.Text = "Имя пользователя";
			lblUsername.Font = new Font("Segoe UI", 11, FontStyle.Bold);
			lblUsername.ForeColor = Color.FromArgb(86, 130, 163);
			lblUsername.AutoSize = true;
			lblUsername.Location = new Point(20, 30);
			formPanel.Controls.Add(lblUsername);

			// Поле имени пользователя
			txtUsername = new TextBox();
			txtUsername.Font = new Font("Segoe UI", 11);
			txtUsername.Size = new Size(340, 30);
			txtUsername.Location = new Point(20, 55);
			txtUsername.BorderStyle = BorderStyle.FixedSingle;
			txtUsername.TextChanged += TxtUsername_TextChanged;
			txtUsername.Enter += TxtUsername_Enter;
			txtUsername.Leave += TxtUsername_Leave;
			formPanel.Controls.Add(txtUsername);

			// Ошибка имени пользователя
			lblUsernameError = new Label();
			lblUsernameError.Text = "";
			lblUsernameError.Font = new Font("Segoe UI", 9, FontStyle.Regular);
			lblUsernameError.ForeColor = Color.Red;
			lblUsernameError.AutoSize = true;
			lblUsernameError.Location = new Point(20, 90);
			lblUsernameError.Visible = false;
			formPanel.Controls.Add(lblUsernameError);

			// Метка для email
			var lblEmail = new Label();
			lblEmail.Text = "Email";
			lblEmail.Font = new Font("Segoe UI", 11, FontStyle.Bold);
			lblEmail.ForeColor = Color.FromArgb(86, 130, 163);
			lblEmail.AutoSize = true;
			lblEmail.Location = new Point(20, 130);
			formPanel.Controls.Add(lblEmail);

			// Поле email
			txtEmail = new TextBox();
			txtEmail.Font = new Font("Segoe UI", 11);
			txtEmail.Size = new Size(340, 30);
			txtEmail.Location = new Point(20, 155);
			txtEmail.BorderStyle = BorderStyle.FixedSingle;
			txtEmail.TextChanged += TxtEmail_TextChanged;
			txtEmail.Enter += TxtEmail_Enter;
			txtEmail.Leave += TxtEmail_Leave;
			formPanel.Controls.Add(txtEmail);

			// Ошибка email
			lblEmailError = new Label();
			lblEmailError.Text = "";
			lblEmailError.Font = new Font("Segoe UI", 9, FontStyle.Regular);
			lblEmailError.ForeColor = Color.Red;
			lblEmailError.AutoSize = true;
			lblEmailError.Location = new Point(20, 190);
			lblEmailError.Visible = false;
			formPanel.Controls.Add(lblEmailError);

			// Кнопка Получить код
			btnGetCode = new Button();
			btnGetCode.Text = "Получить код";
			btnGetCode.Font = new Font("Segoe UI", 12, FontStyle.Bold);
			btnGetCode.ForeColor = Color.White;
			btnGetCode.BackColor = Color.FromArgb(86, 130, 163);
			btnGetCode.Size = new Size(340, 45);
			btnGetCode.Location = new Point(20, 240);
			btnGetCode.FlatStyle = FlatStyle.Flat;
			btnGetCode.FlatAppearance.BorderSize = 0;
			btnGetCode.Cursor = Cursors.Hand;
			btnGetCode.Click += BtnGetCode_Click;
			btnGetCode.Enabled = false; // Изначально неактивна
			formPanel.Controls.Add(btnGetCode);

			// Общая ошибка
			lblError = new Label();
			lblError.Text = "";
			lblError.Font = new Font("Segoe UI", 10, FontStyle.Regular);
			lblError.ForeColor = Color.Red;
			lblError.AutoSize = true;
			lblError.Location = new Point(20, 300);
			lblError.Visible = false;
			lblError.TextAlign = ContentAlignment.MiddleCenter;
			formPanel.Controls.Add(lblError);

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
		}
	}
}