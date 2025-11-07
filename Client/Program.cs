public class Program
{
	private static ChatClient _client;

	public static async Task Main(string[] args)
	{
		Console.WriteLine("🐇 Добро пожаловать в RabbitMQ Chat!");
		Console.WriteLine("=====================================");

		// Регистрация пользователя
		string userName = GetUserName();
		_client = new ChatClient(userName);

		// Начинаем получать сообщения
		_client.StartReceiving();

		Console.WriteLine($"\n✅ Вы зарегистрированы как: {userName}");
		await Task.Delay(1000);

		// Основной цикл чата
		await ChatLoop();
	}

	private static string GetUserName()
	{
		Console.Write("Введите ваше имя: ");
		string name = Console.ReadLine()?.Trim();

		while (string.IsNullOrEmpty(name))
		{
			Console.Write("Имя не может быть пустым. Введите ваше имя: ");
			name = Console.ReadLine()?.Trim();
		}

		return name;
	}

	private static async Task ChatLoop()
	{
		while (true)
		{
			ShowMenu();
			var choice = Console.ReadLine()?.Trim();

			switch (choice)
			{
				case "1":
					await SetRecipient();
					break;
				case "2":
					await SendMessage();
					break;
				case "3":
					await _client.ShowCurrentChatAsync();
					break;
				case "4":
					await _client.ShowAllChatsAsync();  // ЗДЕСЬ ИСПОЛЬЗУЕМ НОВЫЙ МЕТОД
					break;
				case "5":
					Console.WriteLine("👋 До свидания!");
					return;
				default:
					Console.WriteLine("❌ Неверный выбор. Попробуйте снова.");
					break;
			}

			Console.WriteLine("\nНажмите любую клавишу для продолжения...");
			Console.ReadKey();
			Console.Clear();
		}
	}

	private static void ShowMenu()
	{
		Console.WriteLine("\n=== ЧАТ МЕНЮ ===");
		Console.WriteLine("1. 📝 Выбрать собеседника");
		Console.WriteLine("2. ✉️  Отправить сообщение");
		Console.WriteLine("3. 📱 Показать текущий чат");
		Console.WriteLine("4. 💬 Список всех чатов");
		Console.WriteLine("5. 🚪 Выйти");
		Console.Write("Выберите действие: ");
	}

	private static async Task SetRecipient()
	{
		Console.Write("Введите имя собеседника: ");
		string recipient = Console.ReadLine()?.Trim();

		if (string.IsNullOrEmpty(recipient))
		{
			Console.WriteLine("❌ Имя собеседника не может быть пустым.");
			return;
		}

		await _client.SetCurrentInterlocutorAsync(recipient);
		Console.WriteLine($"✅ Собеседник '{recipient}' установлен!");

		// Показываем историю чата
		await _client.ShowCurrentChatAsync();
	}

	private static async Task SendMessage()
	{
		if (string.IsNullOrEmpty(_client.GetCurrentInterlocutor()))
		{
			Console.WriteLine("❌ Сначала выберите собеседника (пункт 1 в меню)");
			return;
		}

		Console.Write($"Введите сообщение для {_client.GetCurrentInterlocutor()}: ");
		string message = Console.ReadLine()?.Trim();

		if (string.IsNullOrEmpty(message))
		{
			Console.WriteLine("❌ Сообщение не может быть пустым.");
			return;
		}

		try
		{
			await _client.SendMessageAsync(message);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"❌ Ошибка отправки: {ex.Message}");
		}
	}
}