using Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.View
{
	public class ChatViewModel : IChat
	{
		public int Id { get; set; }
		[DisplayName("пользователь")]
		public string CurrentUser { get; set; } = string.Empty;
		[DisplayName("собеседник")]
		public string Interlocutor { get; set; } = string.Empty;
	}
}
