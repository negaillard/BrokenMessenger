using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Requests.Requests;

namespace DesktopClient
{
	public class AuthService
	{
		private readonly APIClient _apiClient;

		public AuthService(APIClient apiClient)
		{
			_apiClient = apiClient;
		}

		public async Task<(bool success, string message)> CheckUsernameAsync(string username)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<ApiResult<dynamic>>(
					"/api/auth/check-username",
					new UsernameCheckRequest{ Username =username });

				return (result?.Success == true, result?.Message ?? "Ошибка проверки username");
			}
			catch (Exception ex) {
				return (false, $"Ошибка: {ex.Message}");
			} 
		}

		public async Task<(bool success, string message)> CheckEmailAsync(string email)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<ApiResult<dynamic>>(
					"/api/auth/check-email",
					new EmailCheckRequest{ Email = email });

				return (result?.Success == true, result?.Message ?? "Ошибка проверки email");
			}
			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}");
			}
		}

		public async Task<(bool success,string message)> SendLoginCodeAsync(string username)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<ApiResult<string>>(
					"/api/auth/send-login-code",
					new LoginRequest{ Username = username });

				return (result?.Success == true, result?.Message ?? "Ошибка отправки кода");
			}

			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}");
			}
		}

		public async Task<(bool success, string message, LoginResponse data)> VerifyLoginAsync(string username, string code)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<ApiResult<LoginResponse>>(
					"/api/auth/verify-login",
					new VerifyLoginRequest{ Username = username, Code =code });

				if (result?.Success == true && result.Data != null)
				{
					_apiClient.SetSessionToken(result.Data.SessionToken);
					return (true, result?.Message, result.Data);
				}

				return (false, result?.Message ?? "Ошибка входа", null);
			}
			catch (Exception ex) {
				return (false, $"Ошибка: {ex.Message}", null);
			}
		}

		public async Task<(bool success, string message)> SendRegistrationCodeAsync(string username, string email)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<ApiResult<string>>(
					"/api/auth/send-registration-code",
					new RegistrationRequest{ Username =username, Email = email}
					);

				return (result?.Success == true, result?.Message ?? "Ошибка отправки кода");
			}

			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}");
			}
		}

		public async Task<(bool success, string message, LoginResponse data)> VerifyRegistartionAsync(string username, string email, string code)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<ApiResult<LoginResponse>>(
					"/api/auth/verify-registration",
					new VerifyRegistrationRequest { Username = username, Email = email, Code = code });

				if (result?.Success == true && result.Data != null)
				{
					_apiClient.SetSessionToken(result.Data.SessionToken);
					return (true, result?.Message, result.Data);
				}

				return (false, result?.Message ?? "Ошибка входа", null);
			}
			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}", null);
			}
		}

		public async Task LogoutAsync(string token)
		{
			try
			{
				await _apiClient.PostAsync<ApiResult<bool>>("/api/auth/logout", new LogoutRequest { SessionToken = token});
			}
			catch
			{
				// Игнорируем ошибки при логауте
			}
			finally
			{
				_apiClient.ClearSession();
			}
		}

		public bool IsAuthenticated()
		{
			return SecureStorage.HasSessionToken();
		}

		public async Task<bool> ValidateSessionAsync()
		{
			try
			{
				var result = await _apiClient.GetAsync<ApiResult<bool>>("/api/auth/validate-session");
				return result?.Success == true && result.Data;
			}
			catch
			{
				return false;
			}
		}
	}

	public class ApiResult<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }
	}

	public class LoginResponse
	{
		public string SessionToken { get; set; }
		public string Username { get; set; }
		public int UserId { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}
