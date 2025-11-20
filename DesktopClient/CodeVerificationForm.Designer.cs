namespace DesktopClient
{
	partial class CodeVerificationForm
	{
		private System.ComponentModel.IContainer components = null;
		private Panel panelMain;
		private TextBox txtCode;
		private Button btnVerify;
		private Button btnBack;
		private Button btnResend;
		private Label lblTitle;
		private Label lblEmailInfo;
		private Label lblError;
		private Label lblTimer;
		private Panel formPanel;
		private Label lblCode;

		private System.Windows.Forms.Timer resendTimer;
		private int secondsRemaining = 120; // 2 минуты

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				resendTimer?.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			// Основная панель
			panelMain = new Panel();
			btnBack = new Button();
			lblTitle = new Label();
			formPanel = new Panel();
			lblEmailInfo = new Label();
			lblCode = new Label();
			lblTimer = new Label();
			lblError = new Label();
			btnResend = new Button();


			panelMain.Dock = DockStyle.Fill;
			panelMain.BackColor = Color.FromArgb(235, 245, 251);
			Controls.Add(panelMain);

			// Кнопка Назад

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

			lblTitle.Text = "Подтверждение кода";
			lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
			lblTitle.ForeColor = Color.FromArgb(86, 130, 163);
			lblTitle.AutoSize = true;
			lblTitle.Location = new Point(0, 100);
			lblTitle.Anchor = AnchorStyles.Top;
			lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			panelMain.Controls.Add(lblTitle);

			// Панель формы

			formPanel.Size = new Size(400, 350);
			formPanel.Location = new Point(0, 0);
			formPanel.BackColor = Color.White;
			formPanel.BorderStyle = BorderStyle.FixedSingle;
			formPanel.Padding = new Padding(20);
			panelMain.Controls.Add(formPanel);

			// Информация о email

			lblEmailInfo.Text = "Код отправлен на почту ";
			lblEmailInfo.Font = new Font("Segoe UI", 11, FontStyle.Regular);
			lblEmailInfo.ForeColor = Color.Gray;
			lblEmailInfo.AutoSize = true;
			lblEmailInfo.Location = new Point(20, 15);
			lblEmailInfo.TextAlign = ContentAlignment.MiddleCenter;
			formPanel.Controls.Add(lblEmailInfo);

			// Метка для кода

			lblCode.Text = "Код подтверждения";
			lblCode.Font = new Font("Segoe UI", 11, FontStyle.Bold);
			lblCode.ForeColor = Color.FromArgb(86, 130, 163);
			lblCode.AutoSize = true;
			lblCode.Location = new Point(20, 90);
			formPanel.Controls.Add(lblCode);

			// Поле для ввода кода
			txtCode = new TextBox();
			txtCode.Font = new Font("Segoe UI", 16, FontStyle.Bold); // Крупный шрифт для кода
			txtCode.Size = new Size(340, 40);
			txtCode.Location = new Point(20, 105);
			txtCode.BorderStyle = BorderStyle.FixedSingle;
			txtCode.MaxLength = 6; // Ограничение на 6 цифр
			txtCode.TextAlign = HorizontalAlignment.Center; // Центрируем текст
			txtCode.KeyPress += TxtCode_KeyPress; // Только цифры
			txtCode.TextChanged += TxtCode_TextChanged;
			txtCode.Enter += TxtCode_Enter;
			txtCode.Leave += TxtCode_Leave;
			txtCode.KeyDown += TxtCode_KeyDown; // Для обработки Enter
			formPanel.Controls.Add(txtCode);

			// Кнопка Подтвердить
			btnVerify = new Button();
			btnVerify.Text = "Подтвердить";
			btnVerify.Font = new Font("Segoe UI", 12, FontStyle.Bold);
			btnVerify.ForeColor = Color.White;
			btnVerify.BackColor = Color.FromArgb(86, 130, 163);
			btnVerify.Size = new Size(340, 45);
			btnVerify.Location = new Point(20, 160);
			btnVerify.FlatStyle = FlatStyle.Flat;
			btnVerify.FlatAppearance.BorderSize = 0;
			btnVerify.Cursor = Cursors.Hand;
			btnVerify.Click += BtnVerify_Click;
			btnVerify.Enabled = false;
			formPanel.Controls.Add(btnVerify);

			// Таймер повторной отправки

			lblTimer.Text = "Повторная отправка через: 2:00";
			lblTimer.Font = new Font("Segoe UI", 10, FontStyle.Regular);
			lblTimer.ForeColor = Color.Gray;
			lblTimer.AutoSize = true;
			lblTimer.Location = new Point(20, 220);
			lblTimer.TextAlign = ContentAlignment.MiddleCenter;
			formPanel.Controls.Add(lblTimer);

			// Кнопка Повторной отправки

			btnResend.Text = "Отправить код повторно";
			btnResend.Font = new Font("Segoe UI", 10, FontStyle.Regular);
			btnResend.ForeColor = Color.FromArgb(86, 130, 163);
			btnResend.BackColor = Color.Transparent;
			btnResend.Size = new Size(200, 30);
			btnResend.Location = new Point(100, 220);
			btnResend.FlatStyle = FlatStyle.Flat;
			btnResend.FlatAppearance.BorderSize = 0;
			btnResend.Cursor = Cursors.Hand;
			btnResend.Click += BtnResend_Click;
			btnResend.Visible = false; // Изначально скрыта
			formPanel.Controls.Add(btnResend);

			// Общая ошибка

			lblError.Text = "";
			lblError.Font = new Font("Segoe UI", 10, FontStyle.Regular);
			lblError.ForeColor = Color.Red;
			lblError.AutoSize = true;
			lblError.Location = new Point(20, 260);
			lblError.Visible = false;
			lblError.TextAlign = ContentAlignment.MiddleCenter;
			formPanel.Controls.Add(lblError);

			// Таймер для обратного отсчета
			resendTimer = new System.Windows.Forms.Timer();
			resendTimer.Interval = 1000; // 1 секунда
			resendTimer.Tick += ResendTimer_Tick;

			
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1200, 800);
			Text = "Мессенджер Олег - Подтверждение кода";
			WindowState = FormWindowState.Maximized;
			MaximizeBox = false;
			MinimizeBox = true; // Оставляем возможность сворачивания
			ControlBox = true;
			
		}
	}
}