using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Search
{
	public class ChatSearchModel
	{
		public string? CurrentUser { get; set; }
		public string? Interlocutor { get; set; }
		public DateTime? FromDate { get; set; }
	}
}
