using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DesktopClient
{
	public partial class RegisterForm : Form
	{
		public RegisterForm()
		{
			InitializeComponent();

			this.Load += RegistrationForm_Load;
			this.SizeChanged += RegistrationForm_SizeChanged;
		}

		private void RegistrationForm_Load(object sender, EventArgs e)
		{
			CenterControls();
		}

		private void RegistrationForm_SizeChanged(object sender, EventArgs e)
		{
			if (this.IsHandleCreated && panelMain != null)
			{
				CenterControls();
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

				// Обновляем позиции кнопок выхода и сворачивания
				var exitButton = panelMain.Controls.OfType<Button>()
					.FirstOrDefault(b => b.Text == "✕");
				if (exitButton != null)
				{
					exitButton.Left = panelMain.Width - 50;
				}

				var minimizeButton = panelMain.Controls.OfType<Button>()
					.FirstOrDefault(b => b.Text == "─");
				if (minimizeButton != null)
				{
					minimizeButton.Left = panelMain.Width - 90;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"CenterControls error: {ex.Message}");
			}
		}

		// Валидация имени пользователя
		private void TxtUsername_TextChanged(object sender, EventArgs e)
		{
			ValidateUsername();
			UpdateGetCodeButton();
		}

		private void TxtUsername_Enter(object sender, EventArgs e)
		{
			txtUsername.BackColor = Color.FromArgb(240, 248, 255); // Light blue
		}

		private void TxtUsername_Leave(object sender, EventArgs e)
		{
			txtUsername.BackColor = Color.White;
			ValidateUsername();
		}

		// Валидация email
		private void TxtEmail_TextChanged(object sender, EventArgs e)
		{
			ValidateEmail();
			UpdateGetCodeButton();
		}

		private void TxtEmail_Enter(object sender, EventArgs e)
		{
			txtEmail.BackColor = Color.FromArgb(240, 248, 255);
		}

		private void TxtEmail_Leave(object sender, EventArgs e)
		{
			txtEmail.BackColor = Color.White;
			ValidateEmail();
		}

		private void ValidateUsername()
		{
			var username = txtUsername.Text.Trim();

			if (string.IsNullOrEmpty(username))
			{
				ShowUsernameError("Имя пользователя обязательно");
				return;
			}

			if (username.Length < 3)
			{
				ShowUsernameError("Минимум 3 символа");
				return;
			}

			if (username.Length > 20)
			{
				ShowUsernameError("Максимум 20 символов");
				return;
			}

			if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
			{
				ShowUsernameError("Только буквы, цифры и подчеркивание");
				return;
			}

			HideUsernameError();
		}

		private void ValidateEmail()
		{
			var email = txtEmail.Text.Trim();

			if (string.IsNullOrEmpty(email))
			{
				ShowEmailError("Email обязателен");
				return;
			}

			if (!IsValidEmail(email))
			{
				ShowEmailError("Некорректный формат email");
				return;
			}

			HideEmailError();
		}

		private bool IsValidEmail(string email)
		{
			try
			{
				var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
				return regex.IsMatch(email);
			}
			catch
			{
				return false;
			}
		}

		private void ShowUsernameError(string message)
		{
			lblUsernameError.Text = message;
			lblUsernameError.Visible = true;
			txtUsername.BorderStyle = BorderStyle.FixedSingle;
			txtUsername.BackColor = Color.FromArgb(255, 240, 240); // Light red
		}

		private void HideUsernameError()
		{
			lblUsernameError.Visible = false;
			txtUsername.BorderStyle = BorderStyle.FixedSingle;
			txtUsername.BackColor = Color.White;
		}

		private void ShowEmailError(string message)
		{
			lblEmailError.Text = message;
			lblEmailError.Visible = true;
			txtEmail.BorderStyle = BorderStyle.FixedSingle;
			txtEmail.BackColor = Color.FromArgb(255, 240, 240);
		}

		private void HideEmailError()
		{
			lblEmailError.Visible = false;
			txtEmail.BorderStyle = BorderStyle.FixedSingle;
			txtEmail.BackColor = Color.White;
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

		private void UpdateGetCodeButton()
		{
			var username = txtUsername.Text.Trim();
			var email = txtEmail.Text.Trim();

			bool isUsernameValid = !string.IsNullOrEmpty(username) &&
								 username.Length >= 3 &&
								 username.Length <= 20 &&
								 Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");

			bool isEmailValid = !string.IsNullOrEmpty(email) && IsValidEmail(email);

			btnGetCode.Enabled = isUsernameValid && isEmailValid;

			// Визуальная обратная связь
			if (btnGetCode.Enabled)
			{
				btnGetCode.BackColor = Color.FromArgb(86, 130, 163);
				btnGetCode.Cursor = Cursors.Hand;
			}
			else
			{
				btnGetCode.BackColor = Color.LightGray;
				btnGetCode.Cursor = Cursors.Default;
			}
		}

		private async void BtnGetCode_Click(object sender, EventArgs e)
		{
			try
			{
				// Показываем индикатор загрузки
				btnGetCode.Enabled = false;
				btnGetCode.Text = "Отправка...";
				btnGetCode.BackColor = Color.Gray;

				var username = txtUsername.Text.Trim();
				var email = txtEmail.Text.Trim();

				// TODO: Вызов API для отправки кода
				// var result = await _authService.SendRegistrationCodeAsync(username, email);

				// Имитация API вызова
				await System.Threading.Tasks.Task.Delay(1000);

				// Если успешно - переходим к форме ввода кода
				
				var codeForm = new CodeVerificationForm(username, email, VerificationType.Registration);
				codeForm.Show();
				this.Hide();

				HideError();
			}
			catch (Exception ex)
			{
				ShowError($"Ошибка: {ex.Message}");
			}
			finally
			{
				btnGetCode.Enabled = true;
				btnGetCode.Text = "Получить код";
				btnGetCode.BackColor = Color.FromArgb(86, 130, 163);
			}
		}

		private void BtnBack_Click(object sender, EventArgs e)
		{
			// Возврат к WelcomeForm
			var welcomeForm = new WelcomeForm();
			welcomeForm.Show();
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

	public enum VerificationType
	{
		Registration,
		Login
	}
}
