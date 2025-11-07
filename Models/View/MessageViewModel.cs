using Interfaces;
using System.ComponentModel;

namespace Models.View
{
	public class MessageViewModel : IMessage
	{
		public int Id { get; set; }
		[DisplayName("отправитель")]
		public string Sender { get; set; } = string.Empty;
		[DisplayName("получатель")]
		public string Recipient { get; set; } = string.Empty;
		[DisplayName("текст")]
		public string Content { get; set; } = string.Empty;
		[DisplayName("время")]
		public DateTime Timestamp { get; set; }
		public bool IsSent { get; set; }
		public int ChatId { get; set; }

		public string DisplayTime => Timestamp.ToString("HH:mm");
		public string DirectionIcon => IsSent ? "➡️" : "⬅️";
	}
}
