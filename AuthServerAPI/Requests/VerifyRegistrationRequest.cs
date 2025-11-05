namespace AuthServerAPI.Requests
{
	public class VerifyRegistrationRequest
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string Code { get; set; }
	}
}
