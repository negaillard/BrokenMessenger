using Client.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

public class ChatClient
{
	private readonly User _user;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _exchange = "chat.direct";

	public ChatClient(string userName, string host = "localhost")
	{
		_user = new User(userName);
		var factory = new ConnectionFactory() { HostName = host };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

		// создаём свою очередь и биндим к exchange
		var queueName = $"queue.user.{_user.Username}";
		_channel.QueueDeclare(queueName, false, false, false);
		_channel.QueueBind(queueName, _exchange, routingKey: $"user.{_user.Username}");

		Console.WriteLine($"[{_user.Username}] подключен к серверу, очередь: '{queueName}'");
	}

	public User GetUser() => _user;

	public void StartReceiving()
	{
		var consumer = new EventingBasicConsumer(_channel);
		consumer.Received += (ch, ea) =>
		{
			var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());

			// Парсим сообщение (формат: "sender: content")
			var parts = messageBody.Split(new[] { ": " }, 2, StringSplitOptions.None);
			if (parts.Length == 2)
			{
				var sender = parts[0];
				var content = parts[1];

				// Добавляем в историю чата
				_user.AddReceivedMessage(sender, content);

				// Выводим уведомление
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"\n💬 [{DateTime.Now:HH:mm:ss}] Новое сообщение от {sender}: {content}");
				Console.ResetColor();

				// Если это сообщение от текущего собеседника, показываем в истории
				if (sender == _user.CurrentInterlocutor)
				{
					ShowCurrentChat();
				}

				Console.Write("Выберите действие: "); 
			}
		};
		_channel.BasicConsume($"queue.user.{_user.Username}", true, consumer);
	}

	public void SendMessage(string toUser, string text)
	{
		var body = Encoding.UTF8.GetBytes($"{_user.Username}: {text}");
		_channel.BasicPublish(
			exchange: _exchange,
			routingKey: $"user.{toUser}",
			basicProperties: null,
			body: body
		);

		// Сохраняем в локальную историю
		_user.AddSentMessage(text);

		Console.WriteLine($"✅ Сообщение отправлено {toUser}: {text}");
	}

	public void ShowCurrentChat()
	{
		if (string.IsNullOrEmpty(_user.CurrentInterlocutor))
		{
			Console.WriteLine("❌ Сначала выберите собеседника");
			return;
		}

		var chat = _user.GetOrCreateChat(_user.CurrentInterlocutor);
		chat.DisplayChatHistory();
	}
}
