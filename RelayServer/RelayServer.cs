using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
				_exchange, 
				ExchangeType.Direct, 
				durable: false);
			Console.WriteLine("Relay Server initialized with exchange 'chat.direct'");
		}
	}
}
