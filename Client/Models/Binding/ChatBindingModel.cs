using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models.Interfaces;


namespace Client.Models.Binding
{
	public class ChatBindingModel : IChat
	{
		public int Id { get; set; }
		public string CurrentUser { get; set; }
		public string Interlocutor { get; set; }
	}
}
