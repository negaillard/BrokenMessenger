using Logic;
using Models.Binding;
using Models.LogicContracts;
using Models.Search;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class ChatClient
{
	private readonly string _username;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _exchange = "chat.direct";
	public readonly IChatLogic _chatLogic;
	public readonly IMessageLogic _messageLogic;
	public readonly IUserLogic _userLogic;
	private string _currentInterlocutor;

	public ChatClient(string userName, string host = "localhost")
	{
		_username = userName;

		_chatLogic = new ChatLogic(userName);
		_messageLogic = new MessageLogic(userName);
		_userLogic = new UserLogic(userName);

		var factory = new ConnectionFactory() { HostName = host };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

		// создаём свою очередь и биндим к exchange
		var queueName = $"queue.user.{_username}";

		// задаем аргументы для очереди с временем жизни
		var queueArgs = new Dictionary<string, object>
		{
			{ "x-message-ttl", 2592000000 },    // TTL сообщений: 30 дней (в миллисекундах)
			{ "x-expires", 2592000000 }         // TTL очереди: 30 дней без активности
		};

		// объявляем очередь внутри брокера
		_channel.QueueDeclare(
			queueName,					// имя очереди
			durable: true,				// устойчивость к падению durability
			false,						// exclusive
			false,						// autoDelete
			arguments: queueArgs	    // arguments дополнительно. они null по умолчанию
			);

		// привязываем очередь к exchange по routingKey
		// (то есть говорим, все элементы с таким ключом сюда переправляй епта)
		_channel.QueueBind(
			queueName, // привязываем 
			_exchange, 
			routingKey: $"user.{_username}"
			);

		Console.WriteLine($"[{_username}] подключен к серверу, очередь: '{queueName}'");
	}

	public async Task SetCurrentInterlocutorAsync(string interlocutor)
	{
		_currentInterlocutor = interlocutor;

		var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
		{
			CurrentUser = _username,
			Interlocutor = interlocutor
		});

		if (chat == null)
		{
			await _chatLogic.CreateAsync(new ChatBindingModel
			{
				CurrentUser = _username,
				Interlocutor = interlocutor
			});
		}
	}

	public void StartReceiving()
	{
		var consumer = new EventingBasicConsumer(_channel);
		consumer.Received += async (ch, ea) =>
		{
			var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
			var parts = messageBody.Split(new[] { ": " }, 2, StringSplitOptions.None);
			if (parts.Length == 2)
			{
				var sender = parts[0];
				var content = parts[1];

				try
				{
					// 1. СНАЧАЛА СОЗДАЕМ/ПОЛУЧАЕМ ЧАТ ДЛЯ ПОЛУЧАТЕЛЯ
					var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
					{
						CurrentUser = _username,  // текущий пользователь (получатель)
						Interlocutor = sender     // отправитель становится собеседником
					});

					if (chat == null)
					{
						var createResult = await _chatLogic.CreateAsync(new ChatBindingModel
						{
							CurrentUser = _username,
							Interlocutor = sender
						});

						if (createResult)
						{
							chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
							{
								CurrentUser = _username,
								Interlocutor = sender
							});
						}
					}

					// 2. СОХРАНЯЕМ СООБЩЕНИЕ С ПРАВИЛЬНЫМ ChatId
					if (chat != null)
					{
						await _messageLogic.CreateAsync(new MessageBindingModel
						{
							Sender = sender,
							Recipient = _username,
							Content = content,
							IsSent = false,
							ChatId = chat.Id
						});
					}

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"\n💬 [{DateTime.Now:HH:mm:ss}] Новое сообщение от {sender}: {content}");
					Console.ResetColor();

					if (sender == _currentInterlocutor)
					{
						await ShowCurrentChatAsync();
					}

					// Подтверждаем получение - сообщение УДАЛЯЕТСЯ из очереди
					_channel.BasicAck(ea.DeliveryTag, multiple: false);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"❌ Ошибка при сохранении полученного сообщения: {ex.Message}");
				}

				Console.Write("Выберите действие: ");
			}
		};
		_channel.BasicConsume(
			$"queue.user.{_username}", 
			false,   // ручное подтверждение
			consumer
			);
	}

	public async Task SendMessageAsync(string text)
	{
		if (string.IsNullOrEmpty(_currentInterlocutor))
		{
			Console.WriteLine("❌ Сначала выберите собеседника");
			return;
		}

		try
		{
			// 1. СНАЧАЛА СОЗДАЕМ/ПОЛУЧАЕМ ЧАТ
			var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
			{
				CurrentUser = _username,
				Interlocutor = _currentInterlocutor
			});

			if (chat == null)
			{
				var createResult = await _chatLogic.CreateAsync(new ChatBindingModel
				{
					CurrentUser = _username,
					Interlocutor = _currentInterlocutor
				});

				if (!createResult)
				{
					Console.WriteLine("❌ Не удалось создать чат");
					return;
				}

				// Получаем созданный чат с ID
				chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
				{
					CurrentUser = _username,
					Interlocutor = _currentInterlocutor
				});
			}

			// 2. ТЕПЕРЬ СОХРАНЯЕМ СООБЩЕНИЕ С ПРАВИЛЬНЫМ ChatId
			await _messageLogic.CreateAsync(new MessageBindingModel
			{
				Sender = _username,
				Recipient = _currentInterlocutor,
				Content = text,
				IsSent = true,
				ChatId = chat.Id  // ВАЖНО: передаем ID существующего чата
			});

			// 3. ОТПРАВЛЯЕМ ЧЕРЕЗ RabbitMQ
			var body = Encoding.UTF8.GetBytes($"{_username}: {text}");
			_channel.BasicPublish(_exchange, $"user.{_currentInterlocutor}", null, body);

			Console.WriteLine($"✅ Сообщение отправлено {_currentInterlocutor}: {text}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"❌ Ошибка отправки: {ex.Message}");
			if (ex.InnerException != null)
			{
				Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
			}
		}
	}

	public async Task ShowCurrentChatAsync()
	{
		if (string.IsNullOrEmpty(_currentInterlocutor))
		{
			Console.WriteLine("❌ Сначала выберите собеседника");
			return;
		}

		try
		{
			var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
			{
				CurrentUser = _username,
				Interlocutor = _currentInterlocutor
			});

			if (chat == null)
			{
				Console.WriteLine("❌ Чат не найден");
				return;
			}

			// ИЩЕМ СООБЩЕНИЯ ПО ID ЧАТА (если ChatId правильно сохраняется)
			var messages = await _messageLogic.ReadListAsync(new MessageSearchModel
			{
				ChatId = chat.Id
			});

			Console.WriteLine($"\n📱 Чат с {_currentInterlocutor} (ID: {chat.Id})");
			Console.WriteLine("══════════════════════════════════");

			if (messages == null || !messages.Any())
			{
				Console.WriteLine("   Пока нет сообщений...");
				return;
			}

			foreach (var msg in messages.OrderBy(m => m.Timestamp))
			{
				string direction = msg.Sender == _username ? "➡️" : "⬅️";
				Console.WriteLine($"   {direction} [{msg.Timestamp:HH:mm}] {msg.Sender}: {msg.Content}");
			}

			Console.WriteLine("══════════════════════════════════");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"❌ Ошибка загрузки чата: {ex.Message}");
		}
	}

	public async Task ShowAllChatsAsync()
	{
		var chats = await _chatLogic.ReadListAsync(null);

		Console.WriteLine("\n💬 Ваши чаты:");
		Console.WriteLine("══════════════════════════════════");

		if (chats == null || !chats.Any())
		{
			Console.WriteLine("   Пока нет чатов...");
			return;
		}

		foreach (var chat in chats)
		{
			// Получаем последнее сообщение в чате для превью
			var lastMessage = await _messageLogic.ReadListAsync(new MessageSearchModel
			{
				Sender = _username,
				Recipient = chat.Interlocutor
			});

			var lastMsgPreview = lastMessage?.OrderByDescending(m => m.Timestamp).FirstOrDefault();
			var previewText = lastMsgPreview != null ?
				$"{lastMsgPreview.Content.Substring(0, Math.Min(20, lastMsgPreview.Content.Length))}..." :
				"Нет сообщений";

			Console.WriteLine($"   👤 {chat.Interlocutor} | 📝 {previewText}");
		}

		Console.WriteLine("══════════════════════════════════");
	}

	public string GetUsername() => _username;
	public string GetCurrentInterlocutor() => _currentInterlocutor;
}
