using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Binding
{
	public class ChatBindingModel : IChat
	{
		public int Id { get; set; }
		public string CurrentUser { get; set; } = string.Empty;
		public string Interlocutor { get; set; } = string.Empty;
	}
}
