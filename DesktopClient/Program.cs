using System.Windows.Forms;

namespace DesktopClient
{
	internal static class Program
	{
		public static APIClient ApiClient { get; private set; }
		public static AuthService AuthService { get; private set; }
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new WelcomeForm());

			//// —оздаем APIClient (автоматически загрузит сохраненный токен)
			//ApiClient = new APIClient();

			//// ѕровер€ем есть ли токен и он валидный
			//if (SecureStorage.HasSessionToken())
			//{
			//	// TODO: ћожно добавить проверку валидности токена на сервере
			//	Application.Run(new MainChatForm());
			//}
			//else
			//{
			//	Application.Run(new WelcomeForm());
			//}
		}
	}
}