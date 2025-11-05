using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
using AuthServerAPI.Storage;

namespace AuthServerAPI.Logic
{
	public class AuthService : IAuthService
	{
		private readonly ICodeVerificationLogic _codeVerification;
		private readonly IUserStorage _userStorage;

		public AuthService(ICodeVerificationLogic codeVerification, IUserStorage userStorage)
		{
			_codeVerification = codeVerification;
			_userStorage = userStorage;
		}

		public async Task<AuthResult> SendRegistrationCodeAsync(string username, string email)
		{
			// Проверяем, не отправлялся ли уже код
			if (await _codeVerification.IsCodeSentAsync(email, VerificationType.Registration))
			{
				var timeRemaining = await _codeVerification.GetTimeRemainingAsync(email, VerificationType.Registration);
				return new AuthResult
				{
					Success = false,
					Message = $"Код уже отправлен. Попробуйте через {timeRemaining?.Minutes} минут."
				};
			}

			var (success, message) = await _codeVerification.SendCodeAsync(email, VerificationType.Registration);
			return new AuthResult { Success = success, Message = message };
		}

		public async Task<AuthResult> VerifyRegistrationAsync(string email, string code, string username)
		{
			var (success, message) = await _codeVerification.VerifyCodeAsync(email, code, VerificationType.Registration);

			if (!success)
				return new AuthResult { Success = false, Message = message };

			// Код верный - создаем пользователя
			var user = new UserBindingModel
			{
				Username = username,
				Email = email,
			};

			// Сохраняем в БД
			var createdUser = await _userStorage.InsertElementAsync(user);

			return new AuthResult
			{
				Success = true,
				Message = "Регистрация успешна",
				UserId = createdUser.Id,
				Username = createdUser.Username
			};
		}
	}
}
