using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Binding;
using Models.Pagination;
using Requests.Requests;
using Requests.Responses;

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
				var result = await _apiClient.PostAnonymousAsync<UsernameCheckResponse>(
					"/api/auth/check-username",
					new UsernameCheckRequest{ Username =username });

				return (true, result?.Message ?? "Логин свободен");
			}
			catch (Exception ex) {
				return (false, $"Ошибка: {ex.Message}");
			} 
		}

		public async Task<(bool success, string message)> CheckEmailAsync(string email)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<EmailCheckResponse>(
					"/api/auth/check-email",
					new EmailCheckRequest{ Email = email });

				return (true, result?.Message ?? "Email свободен");
			}
			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}");
			}
		}

		public async Task<(bool success, string message)> SendRegistrationCodeAsync(string username, string email)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<RegistrationResponse>(
					"/api/auth/send-registration-code",
					new RegistrationRequest { Username = username, Email = email }
					);

				return (true, result.Message);
			}

			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}");
			}
		}

		public async Task<(bool success, string message)> VerifyRegistrationAsync(string username, string email, string code)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<VerifyRegistrationResponse>(
					"/api/auth/verify-registration",
					new VerifyRegistrationRequest { Username = username, Email = email, Code = code });

				return (true, result.Message);
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
				var result = await _apiClient.PostAnonymousAsync<LoginResponse>(
					"/api/auth/send-login-code",
					new LoginRequest{ Username = username });

				return (true, result.Message);
			}

			catch (Exception ex)
			{
				return (false, $"Ошибка: {ex.Message}");
			}
		}

		public async Task<(bool success, string message, VerifyLoginResponse response)> VerifyLoginAsync(string username, string code)
		{
			try
			{
				var result = await _apiClient.PostAnonymousAsync<VerifyLoginResponse>(
					"/api/auth/verify-login",
					new VerifyLoginRequest{ Username = username, Code =code });

				if (result?.UserId != null &&
					!string.IsNullOrEmpty(result.Username) && 
					!string.IsNullOrEmpty(result.SessionToken))
				{
					_apiClient.SetSessionToken(result.SessionToken);
					return (true, result.Message, result);
				}
				
				return (false, result.Message, null);
			}
			catch (Exception ex) {
				return (false, $"Ошибка: {ex.Message}", null);
			}
		}

		public async Task LogoutAsync()
		{
			string token = _apiClient.GetSessionToken();
			try
			{
				await _apiClient.PostAsync<LogoutResponse>("/api/auth/logout", new LogoutRequest { SessionToken = token});
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

		public async Task<(bool, string)> ValidateSessionAsync()
		{
			try
			{
				var result = await _apiClient.GetAsync<ValidateSessionResponse>("/api/auth/validate-session");
				return (result.IsValid, result.Username);
			}
			catch(Exception ex) 
			{
				return (false, string.Empty);
			}
		}

		public async Task<(bool Success, PaginatedResult<UserBindingModel>? Result)> SearchUsersAsync(
			string? username = null,
			int page = 1,
			int pageSize = 30)
		{
			try
			{
				// Собираем параметры запроса
				var queryParams = new Dictionary<string, string>
				{
					["page"] = page.ToString(),
					["pageSize"] = pageSize.ToString()
				};

				// Добавляем параметр поиска, если он указан
				if (!string.IsNullOrWhiteSpace(username))
				{
					queryParams["query"] = username;
				}

				// Формируем URL с query параметрами
				var queryString = string.Join("&", queryParams.Select(kvp =>
					$"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

				var url = $"/api/auth/search?{queryString}";

				var result = await _apiClient.GetAsync<PaginatedResult<UserBindingModel>>(url);
				return (true, result);
			}
			catch (Exception ex)
			{
				// Логируем ошибку (по желанию)
				//_logger.LogError(ex, "Error searching users");
				return (false, null);
			}
		}

		public async Task<(bool Success, UserBindingModel? User)> GetUserByIdAsync(string userId)
		{
			try
			{
				var url = $"/api/users/{Uri.EscapeDataString(userId)}";
				var user = await _apiClient.GetAsync<UserBindingModel>(url);
				return (true, user);
			}
			catch (Exception ex)
			{
				// Логируем ошибку
				Console.WriteLine($"Error getting user by id: {ex.Message}");
				return (false, null);
			}
		}
	}
}
