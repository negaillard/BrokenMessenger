using AuthServerAPI.Models;

namespace AuthServerAPI.Logic.Interfaces
{
	public interface ISessionService
	{
		Task<string> CreateSessionAsync(int userId, string username);
		Task<UserSession> GetSessionAsync(string sessionId);
		Task<(bool, string)> ValidateSessionAsync(string sessionId);
		Task<bool> DeleteSessionAsync(string sessionId);
		Task CleanExpiredSessionsAsync();
	}
}
