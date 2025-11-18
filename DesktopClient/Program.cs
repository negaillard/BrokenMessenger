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
		static async Task Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new WelcomeForm());

			// –¿¡Œ◊¿ﬂ “≈Ã¿
			//ApiClient = new APIClient();
			//AuthService = new AuthService(ApiClient);

			//if (!AuthService.IsAuthenticated())
			//{
			//	Application.Run(new WelcomeForm());
			//}

			//bool isValid = await AuthService.ValidateSessionAsync();

			//if (isValid)
			//{
			//	Application.Run(new MainChatForm());
			//}
			//else
			//{
			//	ApiClient.ClearSession();
			//	Application.Run(new WelcomeForm());
			//}
		}
	}
}