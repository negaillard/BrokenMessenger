using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

public class ChatClient
{
	private readonly string _userName;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly string _exchange = "chat.direct";

	public ChatClient(string userName, string host = "localhost")
	{
		_userName = userName;
		var factory = new ConnectionFactory() { HostName = host };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

		// создаём свою очередь и биндим к exchange
		var queueName = $"queue.user.{_userName}";
		_channel.QueueDeclare(queueName, false, false, false);
		_channel.QueueBind(queueName, _exchange, routingKey: $"user.{_userName}");

		Console.WriteLine($"[{_userName}] connected and listening on '{queueName}'");
	}

	public void StartReceiving()
	{
		var consumer = new EventingBasicConsumer(_channel);
		consumer.Received += (ch, ea) =>
		{
			var message = Encoding.UTF8.GetString(ea.Body.ToArray());
			Console.WriteLine($"\n💬 [{_userName}] получил: {message}");
		};
		_channel.BasicConsume($"queue.user.{_userName}", true, consumer);
	}

	public void SendMessage(string toUser, string text)
	{
		var body = Encoding.UTF8.GetBytes($"{_userName}: {text}");
		_channel.BasicPublish(
			exchange: _exchange,
			routingKey: $"user.{toUser}",
			basicProperties: null,
			body: body
		);
		Console.WriteLine($"➡️ [{_userName}] → {toUser}: {text}");
	}
}
