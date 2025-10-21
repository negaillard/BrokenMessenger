using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Interfaces
{
	public interface IMessage
	{
		int Id { get; set; }
		string Sender { get; set; }
		string Recipient { get; set; }
		string Content { get; set; }
		DateTime Timestamp { get; set; }
		bool IsSent { get; set; }
		int ChatId { get; set; }
	}
}
