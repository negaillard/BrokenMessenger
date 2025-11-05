namespace AuthServerAPI.Models
{
	public class AuthResult
	{
		public bool Success {  get; set; }
		public string Message { get; set; } = string.Empty;
		public int UserId { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
