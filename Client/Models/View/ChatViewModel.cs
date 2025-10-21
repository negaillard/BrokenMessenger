using Client.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.View
{
	public class ChatViewModel : IChat
	{
		public int Id { get; set; }
		public string CurrentUser { get; set; }
		public string Interlocutor { get; set; }
	}
}
