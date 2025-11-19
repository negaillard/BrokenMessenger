using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DesktopClient
{
	public partial class LoginForm : Form
	{
		private readonly AuthService _authService;
		public LoginForm()
		{
			var apiClient = new APIClient();
			_authService = new AuthService(apiClient);

			InitializeComponent();

			this.Load += LoginForm_Load;
			this.SizeChanged += LoginForm_SizeChanged;
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			CenterControls();
		}

		#region Валидация
		// Валидация имени пользователя
		private void TxtUsername_TextChanged(object sender, EventArgs e)
		{
			ValidateUsername();
			UpdateGetCodeButton();
		}

		private void ValidateUsername()
		{
			var username = txtUsername.Text.Trim();

			if (string.IsNullOrEmpty(username))
			{
				ShowUsernameError("Введите имя пользователя");
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

		private void TxtUsername_Leave(object sender, EventArgs e)
		{
			txtUsername.BackColor = Color.White;
			ValidateUsername();
		}
		#endregion

		#region Для визуала
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				BtnBack_Click(null, null);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		private void LoginForm_SizeChanged(object sender, EventArgs e)
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
		private void ShowUsernameError(string message)
		{
			lblUsernameError.Text = message;
			lblUsernameError.Visible = true;
			txtUsername.BorderStyle = BorderStyle.FixedSingle;
			txtUsername.BackColor = Color.FromArgb(255, 240, 240);
		}
		private void HideUsernameError()
		{
			lblUsernameError.Visible = false;
			txtUsername.BorderStyle = BorderStyle.FixedSingle;
			txtUsername.BackColor = Color.White;
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

			bool isUsernameValid = !string.IsNullOrEmpty(username) &&
								 username.Length >= 3 &&
								 username.Length <= 20 &&
								 Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");

			btnGetCode.Enabled = isUsernameValid;

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
		private void TxtUsername_Enter(object sender, EventArgs e)
		{
			txtUsername.BackColor = Color.FromArgb(240, 248, 255);
		}
		private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && btnGetCode.Enabled)
			{
				BtnGetCode_Click(sender, e);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}
		private void BtnBack_Click(object sender, EventArgs e)
		{
			Program.ShowWelcomeForm();
		}
		#endregion

		private async void BtnGetCode_Click(object sender, EventArgs e)
		{
			try
			{
				// Показываем индикатор загрузки
				btnGetCode.Enabled = false;
				btnGetCode.Text = "Проверка...";
				btnGetCode.BackColor = Color.Gray;

				var username = txtUsername.Text.Trim();

				var result = await _authService.SendLoginCodeAsync(username);
				if (!result.success)
				{
					ShowError(result.message);
					return;
				}

				// Если успешно - переходим к форме ввода кода

				Program.ShowCodeVerificationForm(username, string.Empty, VerificationType.Login);

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
	}
}
