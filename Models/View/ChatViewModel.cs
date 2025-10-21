using Interfaces;
using System.ComponentModel;

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
