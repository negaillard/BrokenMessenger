using System.Windows.Forms;

namespace DesktopClient
{
	internal static class Program
	{
		private static WelcomeForm _welcomeForm;
		private static LoginForm _loginForm;
		private static RegisterForm _registerForm;
		public static APIClient ApiClient { get; private set; }
		public static AuthService AuthService { get; private set; }

		[STAThread]
		static async Task Main()
		{	
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ApiClient = new APIClient();
			AuthService = new AuthService(ApiClient);

			_welcomeForm = new WelcomeForm();
			_welcomeForm.FormClosed += (s, e) =>
			{
				// Когда закрывается WelcomeForm и это последняя форма - выходим
				if (Application.OpenForms.Count == 0)
					Application.Exit();
			};
			_welcomeForm.Show();
			//if (!AuthService.IsAuthenticated())
			//{
			//	ShowWelcomeForm();
			//}

			//bool isValid = await AuthService.ValidateSessionAsync();

			//if (isValid)
			//{
			//	ShowMainChatForm();
			//}
			//else
			//{
			//	ApiClient.ClearSession();
			//	ShowWelcomeForm();
			//}

			Application.Run();
		}

		public static void ShowWelcomeForm()
		{
			// WelcomeForm всегда создаем заново
			_welcomeForm?.Close();
			_welcomeForm = new WelcomeForm();
			_welcomeForm.FormClosed += (s, e) =>
			{
				if (Application.OpenForms.Count == 0)
					Application.Exit();
			};
			_welcomeForm.Show();
		}

		public static void ShowLoginForm()
		{
			if (_loginForm == null || _loginForm.IsDisposed)
			{
				_loginForm = new LoginForm();
			}
			_loginForm.Show();

			// Скрываем welcome (но не закрываем - можем вернуться)
			_welcomeForm?.Hide();
			_registerForm?.Hide();
		}

		public static void ShowRegisterForm()
		{
			if (_registerForm == null || _registerForm.IsDisposed)
			{
				_registerForm = new RegisterForm();
			}
			_registerForm.Show();

			_welcomeForm?.Hide();
			_loginForm?.Hide();
		}

		public static void ShowCodeVerificationForm(string username, string email, VerificationType type)
		{
			// CodeVerificationForm создаем новую каждый раз
			if (type == VerificationType.Login) {
				var verificationForm = new CodeVerificationForm(username,  type);
				verificationForm.Show();
			}
			else
			{
				var verificationForm = new CodeVerificationForm(username, email, type);
				verificationForm.Show();
			}
			// Скрываем логин/регистрацию (сохраняем данные)
			_loginForm?.Hide();
			_registerForm?.Hide();
		}

		public static void ShowMainChatForm()
		{
			// Главный чат - новая форма, все остальные закрываем
			var mainForm = new MainChatForm();
			mainForm.Show();

			// Закрываем ВСЕ предыдущие формы
			_welcomeForm?.Close();
			_loginForm?.Close();
			_registerForm?.Close();
		}

	}
}