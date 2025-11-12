using AuthServerAPI.Logic;
using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Requests.Requests;

namespace AuthServerAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IUserLogic _userLogic;
		private readonly ICodeVerificationLogic _codeVerificationLogic;
		private readonly ISessionService _sessionService;
		private readonly ILogger<AuthController> _logger;

		public AuthController(
		  IUserLogic userLogic,
		  ICodeVerificationLogic codeVerificationLogic,
		  ISessionService sessionService,
		  ILogger<AuthController> logger)
		{
			_userLogic = userLogic;
			_codeVerificationLogic = codeVerificationLogic;
			_sessionService = sessionService;
			_logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> SendRegistrationCode([FromBody] RegistrationRequest request)
		{
			try
			{
				_logger.LogInformation($"Запрос кода регистрации: {request.Username}, {request.Email}");

				var usernameCheck = await _userLogic.ReadElementAsync(new UserSearchModel { Username = request.Username });
				if (usernameCheck != null)
				{
					return BadRequest(new {error = "Username уже занят" });
				}

				var emailCheck = await _userLogic.ReadElementAsync(new UserSearchModel{ Email = request.Email });
				if (emailCheck != null)
				{
					return BadRequest(new { error = "Пользователь с этим Email уже зарегистрирован})" });
				}

				var result = await _codeVerificationLogic.SendCodeAsync(
					request.Email, 
					VerificationType.Registration
					);

				if (!result.success) {
					return BadRequest(new { error = result.message });
				}

				_logger.LogInformation($"Код регистрации отправлен на: {request.Email}");
				return Ok(new {message  = result.message});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при отправке кода регистрации");
				return BadRequest(new { error = "Ошибка сервера" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> VerifyRegistration([FromBody] VerifyRegistrationRequest request )
		{
			try
			{
				_logger.LogInformation($"Подтверждение регистрации: {request.Email}");

				var codeResult = await _codeVerificationLogic.VerifyCodeAsync(
					request.Email, 
					request.Code, 
					VerificationType.Registration
					);

				if (!codeResult.success)
				{
					return BadRequest(new {error = codeResult.message});
				}

				var userCreated = await _userLogic.CreateAsync(new UserBindingModel
				{
					Email = request.Email,
					Username = request.Username,
				});

				if (!userCreated)
				{
					return BadRequest(new {error = "Ошибка при создании пользовтеля"});
				}

				_logger.LogInformation($"Пользователь создан: {request.Username}");
				return Ok(new
				{
					message = "Регистрация успешно завершена",
					username = request.Username
				});
			}
			catch (Exception ex) 
			{
				_logger.LogError(ex, "Ошибка при подтверждении регистрации");
				return BadRequest(new { error = "Ошибка сервера" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> SendLoginCode([FromBody] LoginRequest request)
		{
			try
			{
				_logger.LogInformation($"Запрос кода для входа: {request.Username}");
				var usernameCheck = await _userLogic.ReadElementAsync(new UserSearchModel { 
					Username = request.Username 
				});

				if( usernameCheck == null)
				{
					return BadRequest(new { error = "Пользователь с таким логином не найден" });
				}

				var result = await _codeVerificationLogic.SendCodeAsync(
					usernameCheck.Email, 
					VerificationType.Login
					);

				if (!result.success)
				{
					return BadRequest(new {error = result.message});
				}

				_logger.LogInformation($"Код входа отправлен на: {usernameCheck.Email}");
				return Ok(new { message = result.message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при отправке кода входа");
				return BadRequest(new { error = "Ошибка сервера" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> VerifyLogin([FromBody] VerifyLoginRequest request)
		{
			try
			{
				_logger.LogInformation($"Подтверждение входа: {request.Username}");
				var user = await _userLogic.ReadElementAsync(new UserSearchModel
				{
					Username = request.Username
				});

				if (user == null)
				{
					return BadRequest(new { error = "Ошибка при получении пользователя" });
				}

				var codeResult = await _codeVerificationLogic.VerifyCodeAsync(user.Email, request.Code, VerificationType.Login);

				if (!codeResult.success) {
					return BadRequest(new { error = codeResult.message });
				}

				var sessionId = await _sessionService.CreateSessionAsync(user.Id, user.Username);

				_logger.LogInformation($"Успешный вход: {request.Username}, session: {sessionId}");

				return Ok(new
				{
					message = "Вход выполнен успешно",
					username = user.Username,
					userId = user.Id,
					sessionToken = sessionId, 
					expiresAt = DateTime.UtcNow.AddHours(24)
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при подтверждении входа");
				return BadRequest(new { error = "Ошибка сервера" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
		{
			await _sessionService.DeleteSessionAsync(request.SessionToken);
			return Ok(new { message = "Выход выполнен" });
		}

		[HttpGet]
		public async Task<IActionResult> ValidateSession([FromHeader] string authorization)
		{
			var sessionId = authorization?.Replace("Bearer ", "");
			if (string.IsNullOrEmpty(sessionId))
				return Unauthorized();

			var isValid = await _sessionService.ValidateSessionAsync(sessionId);
			if (!isValid)
				return Unauthorized();

			return Ok(new { valid = true });
		}

		#region Нахуй не нужны
		// Проверка доступности username
		[HttpPost]
		public async Task<IActionResult> CheckUsername([FromBody] UsernameCheckRequest request)
		{
			try
			{
				_logger.LogInformation($"Проверка username: {request.Username}");

				var existingUser = await _userLogic.ReadElementAsync(new UserSearchModel
				{
					Username = request.Username
				});

				if (existingUser != null)
				{
					return Ok(new
					{
						available = false,
						message = "Username уже занят"
					});
				}

				return Ok(new
				{
					available = true,
					message = "Username доступен"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при проверке username");
				return BadRequest(new { error = "Ошибка сервера" });
			}
		}

		// Проверка доступности email
		[HttpPost]
		public async Task<IActionResult> CheckEmail([FromBody] EmailCheckRequest request)
		{
			try
			{
				_logger.LogInformation($"Проверка username: {request.Email}");

				var existingUser = await _userLogic.ReadElementAsync(new UserSearchModel
				{
					Email = request.Email
				});

				if (existingUser != null)
				{
					return Ok(new
					{
						available = false,
						message = "Email уже зарегистрирован"
					});
				}

				return Ok(new
				{
					available = true,
					message = "Email доступен"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при проверке Email");
				return BadRequest(new { error = "Ошибка сервера" });
			}
		}
		#endregion
	}
}
