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

			//ShowWelcomeForm();

			if (!AuthService.IsAuthenticated())
			{
				_welcomeForm = new WelcomeForm();
				_welcomeForm.FormClosed += (s, e) =>
				{
					_loginForm?.Close();
					_registerForm?.Close();

					//  огда закрываетс€ WelcomeForm и это последн€€ форма - выходим
					if (Application.OpenForms.Count == 0)
						Application.Exit();
				};
				_welcomeForm.Show();
			}
			else
			{
				/// почему то в любом случае считаетс€ невалидной
				bool isValid = await AuthService.ValidateSessionAsync();

				if (isValid)
				{
					ShowMainChatForm();
				}
				else
				{
					ApiClient.ClearSession();
					_welcomeForm = new WelcomeForm();
					_welcomeForm.FormClosed += (s, e) =>
					{
						_loginForm?.Close();
						_registerForm?.Close();

						//  огда закрываетс€ WelcomeForm и это последн€€ форма - выходим
						if (Application.OpenForms.Count == 0)
							Application.Exit();
					};
					_welcomeForm.Show();
				}
			}

			Application.Run();
		}

		public static void ShowWelcomeForm()
		{
			// ѕоказываем WelcomeForm, скрываем остальные
			_welcomeForm?.Show();
			_loginForm?.Hide();
			_registerForm?.Hide();
		}

		public static void ShowLoginForm()
		{
			if (_loginForm == null || _loginForm.IsDisposed)
			{
				_loginForm = new LoginForm();
				_loginForm.FormClosed += (s, e) =>
				{
					// ѕри закрытии LoginForm провер€ем, не нужно ли завершить приложение
					if (Application.OpenForms.Count == 0)
						Application.Exit();
				};
			}
			_loginForm.Show();
			_welcomeForm?.Hide();
			_registerForm?.Hide();
		}

		public static void ShowRegisterForm()
		{
			if (_registerForm == null || _registerForm.IsDisposed)
			{
				_registerForm = new RegisterForm();
				_registerForm.FormClosed += (s, e) =>
				{
					// ѕри закрытии RegisterForm провер€ем, не нужно ли завершить приложение
					if (Application.OpenForms.Count == 0)
						Application.Exit();
				};
			}
			_registerForm.Show();
			_welcomeForm?.Hide();
			_loginForm?.Hide();
		}

		public static void ShowCodeVerificationForm(string username, string email, VerificationType type)
		{
			// CodeVerificationForm создаем новую каждый раз
			CodeVerificationForm verificationForm;
			if (type == VerificationType.Login)
			{
				verificationForm = new CodeVerificationForm(username, type);
			}
			else
			{
				verificationForm = new CodeVerificationForm(username, email, type);
			}

			verificationForm.FormClosed += (s, e) =>
			{
				// ѕри закрытии VerificationForm провер€ем, не нужно ли завершить приложение
				if (Application.OpenForms.Count == 0)
					Application.Exit();
			};

			verificationForm.Show();
			_loginForm?.Hide();
			_registerForm?.Hide();
		}

		public static void ShowMainChatForm()
		{
			// √лавный чат - нова€ форма, все остальные закрываем
			var mainForm = new MainChatForm();
			mainForm.FormClosed += (s, e) =>
			{
				// ѕри закрытии главной формы завершаем приложение
				Application.Exit();
			};
			mainForm.Show();

			// «акрываем ¬—≈ предыдущие формы
			_welcomeForm?.Close();
			_loginForm?.Close();
			_registerForm?.Close();
		}
	}
}