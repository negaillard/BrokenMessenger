using AuthServerAPI.Models;

namespace AuthServerAPI.Logic.Interfaces
{
	public interface IEmailService
	{
		Task<bool> SendVerificationCodeAsync(string email, string code, VerificationType type);
	}
}
