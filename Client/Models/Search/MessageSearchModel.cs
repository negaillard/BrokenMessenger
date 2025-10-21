using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Search
{
	public class MessageSearchModel
	{
		public int? ChatId { get; set; }
		public string? Sender { get; set; }
		public string? Recipient { get; set; }
		public DateTime? FromDate { get; set; }
	}
}
