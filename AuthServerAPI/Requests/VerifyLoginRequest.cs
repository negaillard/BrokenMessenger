namespace AuthServerAPI.Requests
{
	public class VerifyLoginRequest
	{
		public string Username { get; set; }
		public string Code { get; set; }
	}
}
