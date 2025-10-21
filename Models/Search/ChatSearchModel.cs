using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Search
{
	public class ChatSearchModel
	{
		public int? Id { get; set; }
		public string? CurrentUser { get; set; }
		public string? Interlocutor { get; set; }
	}
}
