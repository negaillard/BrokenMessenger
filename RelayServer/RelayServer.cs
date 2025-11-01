using RabbitMQ.Client;

namespace RelayServer
{
	public class RelayServer
	{
		private readonly IConnection _connection;
		private readonly IModel _channel;
		private readonly string _exchange = "chat.direct";

		private readonly Dictionary<string, string> _userQueues = new();

		public RelayServer(string host = "localhost")
		{
			var factory = new ConnectionFactory() { HostName = host };
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			// создаём exchange для чата
			_channel.ExchangeDeclare(
				_exchange,				// имя обменника
				ExchangeType.Direct,	// тип обменника (direct - будет сравнивать только по routing key)
				durable: true			// durability устойчивость выключена 
				);
			Console.WriteLine("Relay Server initialized with exchange 'chat.direct'");
		}
	}
}
