using Interfaces;

namespace Models.Binding
{
	public class ChatBindingModel : IChat
	{
		public int Id { get; set; }
		public string CurrentUser { get; set; } = string.Empty;
		public string Interlocutor { get; set; } = string.Empty;
	}
}
