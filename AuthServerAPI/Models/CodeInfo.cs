namespace AuthServerAPI.Models
{
	public class CodeInfo {
		public string Code { get; set; }
		public string Email { get; set; }
		public VerificationType Type { get; set; }
		public DateTime CreatedAt { get; set; }
		public int Attempts { get; set; }
	}
}
