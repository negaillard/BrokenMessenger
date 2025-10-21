using Interfaces;

namespace Models.Binding
{
	public class MessageBindingModel : IMessage
	{
		public int Id { get; set; }
		public string Sender { get; set; } = string.Empty;
		public string Recipient { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; }
		public bool IsSent { get; set; }
		public int ChatId { get; set; }
	}
}
