namespace AuthServerAPI.Models
{
	public class UserSession
	{
		public string SessionId { get; set; }
		public int UserId { get; set; }
		public string Username { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime ExpiresAt { get; set; }
		public bool IsActive { get; set; }
	}
}
