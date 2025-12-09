using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Binding
{
	public class MessageDto
	{
		public string Sender { get; set; }
		public string Recipient { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
