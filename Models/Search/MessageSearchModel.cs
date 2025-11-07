namespace Models.Search
{
	public class MessageSearchModel
	{
		public int? Id { get; set; }
		public string? Sender { get; set; }
		public string? Recipient { get; set; }
		public string? Content { get; set; }
		public DateTime? Timestamp { get; set; }
		public bool? IsSent { get; set; }
		public int? ChatId { get; set; }
	}
}
