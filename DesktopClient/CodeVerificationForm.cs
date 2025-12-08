using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DesktopClient
{
	public partial class CodeVerificationForm : Form
	{
		private string _username;
		private string _email = string.Empty;
		private VerificationType _verificationType;
		private readonly AuthService _authService;

		public CodeVerificationForm(string username, string email, VerificationType verificationType)
		{
			_username = username;
			_email = email;
			_verificationType = verificationType;

			var apiClient = new APIClient();
			_authService = new AuthService(apiClient);

			InitializeComponent();

			this.Load += CodeVerificationForm_Load;
			this.SizeChanged += CodeVerificationForm_SizeChanged;
		}

		public CodeVerificationForm(string username, VerificationType verificationType)
		{
			_username = username;
			_verificationType = verificationType;

			var apiClient = new APIClient();
			_authService = new AuthService(apiClient);

			InitializeComponent();

			this.Load += CodeVerificationForm_Load;
			this.SizeChanged += CodeVerificationForm_SizeChanged;
		}

		private void CodeVerificationForm_Load(object sender, EventArgs e)
		{
			SetupEmailDisplay();
			StartResendTimer();
			CenterControls();
		}
		#region Валидация

		// Валидация кода
		private void TxtCode_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Разрешаем только цифры и Backspace
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void TxtCode_TextChanged(object sender, EventArgs e)
		{
			ValidateCode();
		}

		private void TxtCode_Enter(object sender, EventArgs e)
		{
			txtCode.BackColor = Color.FromArgb(240, 248, 255);
		}

		private void TxtCode_Leave(object sender, EventArgs e)
		{
			txtCode.BackColor = Color.White;
		}

		private void TxtCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && btnVerify.Enabled)
			{
				BtnVerify_Click(sender, e);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void ValidateCode()
		{
			var code = txtCode.Text.Trim();

			if (string.IsNullOrEmpty(code))
			{
				btnVerify.Enabled = false;
				btnVerify.BackColor = Color.LightGray;
				return;
			}

			if (code.Length == 6 && code.All(char.IsDigit))
			{
				btnVerify.Enabled = true;
				btnVerify.BackColor = Color.FromArgb(86, 130, 163);
				btnVerify.Cursor = Cursors.Hand;
			}
			else
			{
				btnVerify.Enabled = false;
				btnVerify.BackColor = Color.LightGray;
				btnVerify.Cursor = Cursors.Default;
			}
		}
		#endregion

		#region Для визуала

		private void BtnBack_Click(object sender, EventArgs e)
		{
			// Возврат к логину/регистрации (они уже в памяти)
			if (_verificationType == VerificationType.Login)
			{
				Program.ShowLoginForm();
			}
			else
			{
				Program.ShowRegisterForm();
			}
			this.Close(); // Эту форму закрываем
		}
		private void CodeVerificationForm_SizeChanged(object sender, EventArgs e)
		{
			if (this.IsHandleCreated && panelMain != null)
			{
				CenterControls();
			}
		}

		private string MaskEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
				return "***@***.***";

			try
			{
				var parts = email.Split('@');
				if (parts.Length != 2) return "***@***.***";

				string localPart = parts[0];
				string domainPart = parts[1];

				// Маскируем local part (первые 3 символа + ***)
				string maskedLocal = localPart.Length <= 3 ?
					localPart.Substring(0, 1) + "***" :
					localPart.Substring(0, 3) + "***";

				// Маскируем domain part
				var domainParts = domainPart.Split('.');
				if (domainParts.Length >= 2)
				{
					string domainName = domainParts[0];
					string maskedDomain = domainName.Length <= 3 ?
						domainName.Substring(0, 1) + "***" :
						domainName.Substring(0, 3) + "***";

					string extension = string.Join(".", domainParts.Skip(1));
					return $"{maskedLocal}@{maskedDomain}.{extension}";
				}

				return $"{maskedLocal}@***.***";
			}
			catch
			{
				return "***@***.***";
			}
		}
		private void CenterControls()
			{
				try
				{
					if (panelMain == null || lblTitle == null)
						return;

					// Центрируем заголовок
					lblTitle.Left = (panelMain.Width - lblTitle.Width) / 2;

					// Центрируем панель формы
					var formPanel = panelMain.Controls.OfType<Panel>()
						.FirstOrDefault(p => p.BackColor == Color.White);
					if (formPanel != null)
					{
						formPanel.Left = (panelMain.Width - formPanel.Width) / 2;
						formPanel.Top = (panelMain.Height - formPanel.Height) / 2;
					}

					// Центрируем информацию о email
					if (lblEmailInfo != null)
					{
						lblEmailInfo.Left = (formPanel.Width - lblEmailInfo.Width) / 2;
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"CenterControls error: {ex.Message}");
				}
			}

		private void ShowError(string message)
		{
			lblError.Text = message;
			lblError.Visible = true;
		}

		private void HideError()
		{
			lblError.Visible = false;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				BtnBack_Click(null, null);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion

		#region Таймер
		// Таймер для обратного отсчета
		private void StartResendTimer()
		{
			secondsRemaining = 120; // 2 минуты
			UpdateTimerDisplay();
			resendTimer.Start();
		}

		private void ResendTimer_Tick(object sender, EventArgs e)
		{
			secondsRemaining--;
			UpdateTimerDisplay();

			if (secondsRemaining <= 0)
			{
				resendTimer.Stop();
				lblTimer.Visible = false;
				btnResend.Visible = true;
			}
		}

		private void UpdateTimerDisplay()
		{
			var minutes = secondsRemaining / 60;
			var seconds = secondsRemaining % 60;
			lblTimer.Text = $"Повторная отправка через: {minutes}:{seconds:D2}";
		}
		#endregion

		private void SetupEmailDisplay()
		{
			string maskedEmail = MaskEmail(_email);
			string actionText = _verificationType == VerificationType.Registration ?
				"Для завершения регистрации" : "Для входа в аккаунт";

			lblEmailInfo.Text = $"{actionText}\nКод отправлен на почту:\n{maskedEmail}";
		}

		private async void BtnVerify_Click(object sender, EventArgs e)
		{
			try
			{
				// Показываем индикатор загрузки
				btnVerify.Enabled = false;
				btnVerify.Text = "Проверка...";
				btnVerify.BackColor = Color.Gray;

				var code = txtCode.Text.Trim();
				if (_verificationType == VerificationType.Registration)
				{
					var result = await _authService.VerifyRegistrationAsync(_username, _email, code);
					if (!result.success)
					{
						ShowError(result.message);
						return;
					}
					Program.ShowLoginForm();
					this.Close();
				}
				if (_verificationType == VerificationType.Login)
				{
					var result = await _authService.VerifyLoginAsync(_username, code);
					if (!result.success)
					{
						ShowError(result.message);
						return;
					}
					// сюда надо передавать из результата имя и email, чтобы создать бд(локальную)
					// После успешной верификации
					Program.ShowMainChatForm(_username, string.Empty);
					this.Close();

					HideError();
				}
			}
			catch (Exception ex)
			{
				ShowError($"Неверный код. Попробуйте снова.");
			}
			finally
			{
				btnVerify.Enabled = true;
				btnVerify.Text = "Подтвердить";
				btnVerify.BackColor = Color.FromArgb(86, 130, 163);
			}
		}

		private async void BtnResend_Click(object sender, EventArgs e)
		{
			try
			{
				btnResend.Enabled = false;
				btnResend.Text = "Отправка...";
				if (_verificationType == VerificationType.Login)
				{
					var result = await _authService.SendLoginCodeAsync(_username);
					if (!result.success)
					{
						ShowError(result.message);
						return;
					}
				}
				else
				{
					var result = await _authService.SendRegistrationCodeAsync(_username, _email);
					if (!result.success)
					{
						ShowError(result.message);
						return;
					}
				}

				// Сбрасываем таймер
				StartResendTimer();
				lblTimer.Visible = true;
				btnResend.Visible = false;

				HideError();
			}
			catch (Exception ex)
			{
				ShowError($"Ошибка отправки: {ex.Message}");
			}
			finally
			{
				btnResend.Enabled = true;
				btnResend.Text = "Отправить код повторно";
			}
		}
	}
}
