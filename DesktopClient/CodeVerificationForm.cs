using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DesktopClient
{
	public partial class CodeVerificationForm : Form
	{
		private string _username;
		private string _email;
		private VerificationType _verificationType;

		public CodeVerificationForm(string username, string email, VerificationType verificationType)
		{
			_username = username;
			_email = email;
			_verificationType = verificationType;

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

		private void CodeVerificationForm_SizeChanged(object sender, EventArgs e)
		{
			if (this.IsHandleCreated && panelMain != null)
			{
				CenterControls();
			}
		}

		private void SetupEmailDisplay()
		{
			string maskedEmail = MaskEmail(_email);
			string actionText = _verificationType == VerificationType.Registration ?
				"Для завершения регистрации" : "Для входа в аккаунт";

			lblEmailInfo.Text = $"{actionText}\nКод отправлен на почту:\n{maskedEmail}";
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

		private void ShowError(string message)
		{
			lblError.Text = message;
			lblError.Visible = true;
		}

		private void HideError()
		{
			lblError.Visible = false;
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

				// TODO: Вызов API для проверки кода
				// var result = await _authService.VerifyCodeAsync(_username, _email, code, _verificationType);

				// Имитация API вызова
				await System.Threading.Tasks.Task.Delay(1000);

				// Если успешно - переходим к главной форме
				if (_verificationType == VerificationType.Registration)
				{
					// TODO: Создание пользователя и вход
				}

				var mainForm = new MainChatForm();
				mainForm.Show();
				this.Hide();

				HideError();
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

				// TODO: Вызов API для повторной отправки кода
				// var result = await _authService.ResendCodeAsync(_username, _email, _verificationType);

				await System.Threading.Tasks.Task.Delay(500);

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

		private void BtnBack_Click(object sender, EventArgs e)
		{
			// Возврат к предыдущей форме
			Form previousForm = _verificationType == VerificationType.Registration ?
				new RegisterForm() : new LoginForm();

			previousForm.Show();
			this.Hide();
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
	}
}
