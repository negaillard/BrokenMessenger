using AuthServerAPI.Models;

namespace AuthServerAPI.Logic.Interfaces
{
	public interface ICodeVerificationLogic
	{
		string GenerateCode();
		Task<(bool success, string message)> SendCodeAsync(string email, VerificationType type);
		Task<(bool success, string message)> VerifyCodeAsync(string email, string code, VerificationType type);
		Task<bool> IsCodeSentAsync(string email, VerificationType type);
		Task<TimeSpan?> GetTimeRemainingAsync(string email, VerificationType type);
	}
}
