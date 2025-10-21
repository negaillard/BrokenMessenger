using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models.Interfaces;


namespace Client.Models.Binding
{
	public class MessageBindingModel : IMessage
	{
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Recipient { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsSent { get; set; }
		public int ChatId { get; set; }
	}
}
