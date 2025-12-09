using Logic;
using Models.Binding;
using Models.LogicContracts;
using Models.Search;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class ChatClient
{
	private readonly string _username;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _exchange = "chat.direct";

	public readonly IChatLogic _chatLogic;
	public readonly IMessageLogic _messageLogic;
	public readonly IUserLogic _userLogic;

	private CancellationTokenSource _cts = new CancellationTokenSource();
	private string _currentInterlocutor;

	public ChatClient(string username, string host = "localhost")
	{
		_username = username;

		_chatLogic = new ChatLogic(username);
		_messageLogic = new MessageLogic(username);
		_userLogic = new UserLogic(username);

		var factory = new ConnectionFactory()
		{
			HostName = host,
			DispatchConsumersAsync = true
		};

		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

		// Объявляем обменник (если уже есть - не создаст заново)
		_channel.ExchangeDeclare(_exchange, ExchangeType.Direct, durable: true);

		// Очередь для пользователя
		var queue = $"queue.user.{_username}";
		_channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);

		// Привязка к exchange
		_channel.QueueBind(queue, _exchange, $"user.{_username}");
	}

	public void StartReceiving()
	{
		var queue = $"queue.user.{_username}";

		_ = Task.Run(async () =>
		{
			Console.WriteLine($"📥 Consumer loop started for {_username}");

			while (!_cts.Token.IsCancellationRequested)
			{
				try
				{
					var result = _channel.BasicGet(queue, autoAck: false);

					if (result == null)
					{
						await Task.Delay(200, _cts.Token);
						continue;
					}

					var json = Encoding.UTF8.GetString(result.Body.ToArray());
					var msg = JsonSerializer.Deserialize<MessageDto>(json);

					if (msg == null)
					{
						_channel.BasicAck(result.DeliveryTag, false);
						continue;
					}

					await HandleIncomingMessage(msg);

					_channel.BasicAck(result.DeliveryTag, false);
				}
				catch (Exception ex)
				{
					Console.WriteLine("❌ Consumer error: " + ex.Message);
					await Task.Delay(500);
				}
			}

			Console.WriteLine("❗ Consumer loop stopped.");
		});
	}

	private async Task HandleIncomingMessage(MessageDto msg)
	{
		var sender = msg.Sender;

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

		await _messageLogic.CreateAsync(new MessageBindingModel
		{
			Sender = sender,
			Recipient = _username,
			Content = msg.Content,
			Timestamp = msg.Timestamp,
			ChatId = chat.Id,
			IsSent = false
		});

		Console.WriteLine($"💬 {sender}: {msg.Content}");
	}

	public async Task SendMessageAsync(string text)
	{
		if (_currentInterlocutor == null) return;

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

		await _messageLogic.CreateAsync(new MessageBindingModel
		{
			Sender = dto.Sender,
			Recipient = dto.Recipient,
			Content = dto.Content,
			Timestamp = dto.Timestamp,
			ChatId = chat.Id,
			IsSent = true
		});

		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));
		_channel.BasicPublish(_exchange, $"user.{_currentInterlocutor}", null, body);
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

