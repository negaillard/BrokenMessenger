using AuthServerAPI.Logic.Interfaces;
using AuthServerAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AuthServerAPI.Logic
{
	/// <summary>
	///  ЗДЕСЬ ИСПОЛЬЗУЕТСЯ REDIS
	/// </summary>
	public class CodeVerificationLogic : ICodeVerificationLogic
	{
		private readonly IDistributedCache _cache;
		private readonly IEmailService _emailService;
		private readonly RedisSettings _settings;

		public CodeVerificationLogic(
			IDistributedCache cache,
			IEmailService emailService,
			IOptions<RedisSettings> settings)
		{
			_cache = cache;
			_emailService = emailService;
			_settings = settings.Value;
		}

		public string GenerateCode()
		{
			var random = new Random();
			return random.Next(100000, 999999).ToString();
		}

		public async Task<(bool success, string message)> SendCodeAsync(string email, VerificationType type)
		{
			try
			{
				// Проверяем rate limiting (не чаще чем раз в 1 минуту)
				var rateLimitKey = $"ratelimit:{email}";
				var existingRateLimit = await _cache.GetStringAsync(rateLimitKey);
				if (existingRateLimit != null)
				{
					return (false, "Слишком частые запросы. Попробуйте через 1 минуту.");
				}

				// Устанавливаем rate limit на 1 минуту
				// Как будут выглядеть ключи в redis с использованием InstanceName = "AuthServer_"
				// Это нужно, что 
				// AuthServer_verification:EmailVerification:user@example.com
				// AuthServer_ratelimit:user@example.com

				await _cache.SetStringAsync(
					key: rateLimitKey, 
					value: "1", 
					options: new DistributedCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
					});

				// Генерируем код
				var code = GenerateCode();
				var codeInfo = new CodeInfo
				{
					Code = code,
					Email = email,
					Type = type,
					CreatedAt = DateTime.UtcNow,
					Attempts = 0 // счетчик попыток ввода
				};

				// Сохраняем в Redis
				var cacheKey = GetCacheKey(email, type);
				var serializedCodeInfo = JsonSerializer.Serialize(codeInfo);

				await _cache.SetStringAsync(
					key: cacheKey, 
					value: serializedCodeInfo, 
					options: new DistributedCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.VerificationCodeExpirationMinutes)
					});

				// Отправляем email
				await _emailService.SendVerificationCodeAsync(email, code, type);

				return (true, "Код отправлен на email");
			}
			catch (Exception ex)
			{
				return (false, $"Ошибка отправки: {ex.Message}");
			}
		}

		public async Task<(bool success, string message)> VerifyCodeAsync(string email, string code, VerificationType type)
		{
			try
			{
				var cacheKey = GetCacheKey(email, type);
				var serializedCodeInfo = await _cache.GetStringAsync(cacheKey);

				if (string.IsNullOrEmpty(serializedCodeInfo))
					return (false, "Код не найден или устарел. Запросите новый код.");

				var codeInfo = JsonSerializer.Deserialize<CodeInfo>(serializedCodeInfo);

				// Проверяем количество попыток
				if (codeInfo.Attempts >= 3)
				{
					await _cache.RemoveAsync(cacheKey); // Удаляем код после 3 неудачных попыток
					return (false, "Слишком много неверных попыток. Запросите новый код.");
				}

				// Проверяем код
				if (codeInfo.Code != code)
				{
					// Увеличиваем счетчик попыток
					codeInfo.Attempts++;
					var updatedSerializedCodeInfo = JsonSerializer.Serialize(codeInfo);
					await _cache.SetStringAsync(
						key: cacheKey, 
						value: updatedSerializedCodeInfo, 
						options:new DistributedCacheEntryOptions
						{
							AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.VerificationCodeExpirationMinutes)
						});

					var attemptsLeft = 3 - codeInfo.Attempts;
					return (false, $"Неверный код. Осталось попыток: {attemptsLeft}");
				}

				// Код верный - удаляем из Redis
				await _cache.RemoveAsync(cacheKey);
				return (true, "Код подтвержден");
			}
			catch (Exception ex)
			{
				return (false, $"Ошибка проверки кода: {ex.Message}");
			}
		}

		private static string GetCacheKey(string email, VerificationType type)
		{
			// verification:Login:anasirov@gmail.com
			return $"verification:{type}:{email.ToLowerInvariant()}";
		}


		#region нахуй не нужны
		public async Task<bool> IsCodeSentAsync(string email, VerificationType type)
		{
			var cacheKey = GetCacheKey(email, type);
			var codeInfo = await _cache.GetStringAsync(cacheKey);
			return !string.IsNullOrEmpty(codeInfo);
		}

		public async Task<TimeSpan?> GetTimeRemainingAsync(string email, VerificationType type)
		{
			// Этот метод может потребовать дополнительной логики в зависимости от версии Redis
			var cacheKey = GetCacheKey(email, type);
			var codeInfo = await _cache.GetStringAsync(cacheKey);
			return codeInfo != null ? TimeSpan.FromMinutes(5) : null; // Упрощенная версия
		}
		#endregion
	}

	public class RedisSettings
	{
		public int VerificationCodeExpirationMinutes { get; set; } = 15;
	}
}
