using Client.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
	public class Message : IMessage
	{
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Recipient { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsSent { get; set; }
		public int ChatId { get; set; }

		public Message()
		{
			Timestamp = DateTime.UtcNow;
		}

		public Message(string sender, string recipient, string content, bool isSent) : this()
		{
			Sender = sender;
			Recipient = recipient;
			Content = content;
			IsSent = isSent;
		}

		public override string ToString()
		{
			string direction = IsSent ? "➡️" : "⬅️";
			return $"{direction} [{Timestamp:HH:mm}] {Sender}: {Content}";
		}

	}
}
