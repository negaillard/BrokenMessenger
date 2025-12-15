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

		[DisplayName("Последнее сообщение")]
		public string LastMessageText { get; set; } = string.Empty;

		[DisplayName("Время")]
		public DateTime? LastMessageTime { get; set; }
	}
}
