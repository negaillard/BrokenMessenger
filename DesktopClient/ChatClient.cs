using Logic;
using Models.Binding;
using Models.LogicContracts;
using Models.Search;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class ChatClient
{
	private readonly string _username;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _exchange = "chat.direct";
	private readonly string _queueName;
	public event Action<MessageDto>? OnMessageReceived;

	private readonly Dictionary<string, object> _queueArgs =
		new Dictionary<string, object>
		{
			{ "x-message-ttl", 2592000000 },   // 30 дней
            { "x-expires",      2592000000 }   // очередь умирает, если не используется 30 дней
        };

	public readonly IChatLogic _chatLogic;
	public readonly IMessageLogic _messageLogic;

	private string _currentInterlocutor;


	public ChatClient(string username, string host = "localhost")
	{
		_username = username;
		_queueName = $"queue.user.{_username}";

		_chatLogic = new ChatLogic(username);
		_messageLogic = new MessageLogic(username);

		var factory = new ConnectionFactory()
		{
			HostName = host,
			DispatchConsumersAsync = true
		};

		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

		// Проверяем, что exchange существует
		_channel.ExchangeDeclarePassive(_exchange);

		// Создаём очередь ВСЕГДА с одинаковыми аргументами
		_channel.QueueDeclare(
			queue: _queueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: _queueArgs // ← важно!
		);

		// Привязываем
		_channel.QueueBind(_queueName, _exchange, $"user.{_username}");

		Console.WriteLine($"[{_username}] Queue declared and bound: '{_queueName}'");
	}

	public void StartReceiving()
	{
		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.Received += async (ch, ea) =>
		{
			try
			{
				var json = Encoding.UTF8.GetString(ea.Body.ToArray());
				var msg = JsonSerializer.Deserialize<MessageDto>(json);

				if (msg == null)
				{
					_channel.BasicAck(ea.DeliveryTag, false);
					return;
				}

				await HandleIncomingMessage(msg);
				_channel.BasicAck(ea.DeliveryTag, false);
			}
			catch (Exception ex)
			{
				Console.WriteLine("❌ Consumer error: " + ex.Message);
			}
		};

		_channel.BasicConsume(
			queue: _queueName,
			autoAck: false,
			consumer: consumer
		);

		Console.WriteLine($"📥 [{_username}] Consumer is running.");
	}

	private async Task HandleIncomingMessage(MessageDto msg)
	{
		var sender = msg.Sender;

		// Находим или создаём чат
		var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
		{
			CurrentUser = _username,
			Interlocutor = sender
		});

		if (chat == null)
		{
			await _chatLogic.CreateAsync(new ChatBindingModel
			{
				CurrentUser = _username,
				Interlocutor = sender
			});

			chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
			{
				CurrentUser = _username,
				Interlocutor = sender
			});
		}

		// Сохраняем сообщение
		await _messageLogic.CreateAsync(new MessageBindingModel
		{
			Sender = sender,
			Recipient = _username,
			Content = msg.Content,
			Timestamp = msg.Timestamp,
			IsSent = false,
			ChatId = chat.Id
		});

		Console.WriteLine($"💬 {sender}: {msg.Content}");

		OnMessageReceived?.Invoke(msg);
	}

	public async Task SendMessageAsync(string text)
	{
		if (_currentInterlocutor == null)
			return;

		var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
		{
			CurrentUser = _username,
			Interlocutor = _currentInterlocutor
		});

		if (chat == null)
		{
			await _chatLogic.CreateAsync(new ChatBindingModel
			{
				CurrentUser = _username,
				Interlocutor = _currentInterlocutor
			});

			chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
			{
				CurrentUser = _username,
				Interlocutor = _currentInterlocutor
			});
		}

		var dto = new MessageDto
		{
			Sender = _username,
			Recipient = _currentInterlocutor,
			Content = text,
			Timestamp = DateTime.UtcNow
		};

		// Сохраняем у себя
		await _messageLogic.CreateAsync(new MessageBindingModel
		{
			Sender = _username,
			Recipient = _currentInterlocutor,
			Content = text,
			Timestamp = dto.Timestamp,
			ChatId = chat.Id,
			IsSent = true
		});

		// Отправляем в RabbitMQ
		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));

		_channel.BasicPublish(
			exchange: _exchange,
			routingKey: $"user.{_currentInterlocutor}",
			basicProperties: null,
			body: body
		);

		Console.WriteLine($"➡️ {text}");
	}

	public async Task SetCurrentInterlocutorAsync(string user)
	{
		_currentInterlocutor = user;

		var chat = await _chatLogic.ReadElementAsync(new ChatSearchModel
		{
			CurrentUser = _username,
			Interlocutor = user
		});

		if (chat == null)
		{
			await _chatLogic.CreateAsync(new ChatBindingModel
			{
				CurrentUser = _username,
				Interlocutor = user
			});
		}
	}

	public string GetUsername() => _username;
}
