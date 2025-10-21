public class Program
{
	public static void Main(string[] args)
	{
		var server = new RelayServer.RelayServer();
		Console.WriteLine("Relay Server запущен!");
		Console.WriteLine("Exchange: 'chat.direct'");
		Console.WriteLine("Ожидание подключений клиентов...");
		Console.WriteLine("Нажмите Ctrl+C для остановки\n");

		// Сервер работает постоянно
		while (true)
		{
			Thread.Sleep(1000);
		}
	}
}
