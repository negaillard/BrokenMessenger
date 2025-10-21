using Client.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.View
{
	public class MessageViewModel : IMessage
	{
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Recipient { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsSent { get; set; }
		public int ChatId { get; set; }

		public string DisplayTime => Timestamp.ToString("HH:mm");
		public string DirectionIcon => IsSent ? "➡️" : "⬅️";
	}
}
