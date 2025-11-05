using AuthServerAPI.Models;

namespace AuthServerAPI.Logic.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResult> SendRegistrationCodeAsync(string username, string email);
		Task<AuthResult> VerifyRegistrationAsync(string email, string code, string username);
	}
}
