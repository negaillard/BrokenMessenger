using AuthServerAPI.Logic;
using AuthServerAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AuthServerTests
{
	public class SessionServiceTests
	{
		private readonly Mock<IDistributedCache> _mockCache;
		private readonly Mock<IConfiguration> _mockConfig;
		private readonly SessionService _sessionService;

		public SessionServiceTests()
		{
			_mockCache = new Mock<IDistributedCache>();
			_mockConfig = new Mock<IConfiguration>();
			_sessionService = new SessionService(_mockCache.Object, _mockConfig.Object);
		}

		[Fact]
		public async Task CreateSessionAsync_CreatesSessionAndStoresInCache()
		{
			// Arrange
			int userId = 1;
			string username = "testuser";

			string? savedKey = null;
			byte[]? savedValue = null;

			_mockCache
				.Setup(c => c.SetAsync(
					It.IsAny<string>(),
					It.IsAny<byte[]>(),
					It.IsAny<DistributedCacheEntryOptions>(),
					default))
				.Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((key, val, opt, t) =>
				{
					savedKey = key;
					savedValue = val;
				})
				.Returns(Task.CompletedTask);

			// Act
			var sessionId = await _sessionService.CreateSessionAsync(userId, username);

			// Assert
			Assert.NotNull(sessionId);
			Assert.NotNull(savedKey);
			Assert.NotNull(savedValue);

			Assert.StartsWith("session:", savedKey);

			// Десериализуем из байтов
			var jsonString = Encoding.UTF8.GetString(savedValue);
			var session = JsonSerializer.Deserialize<UserSession>(jsonString);
			Assert.NotNull(session);
			Assert.Equal(sessionId, session.SessionId);
			Assert.Equal(userId, session.UserId);
			Assert.Equal(username, session.Username);

			_mockCache.Verify(c =>
				c.SetAsync(
					It.IsAny<string>(),
					It.IsAny<byte[]>(),
					It.IsAny<DistributedCacheEntryOptions>(),
					default),
				Times.Once);
		}

		[Fact]
		public async Task GetSessionAsync_ReturnsSession_WhenExists()
		{
			// Arrange
			var session = new UserSession
			{
				SessionId = "abc",
				UserId = 1,
				Username = "u1",
				CreatedAt = DateTime.UtcNow
			};

			var json = JsonSerializer.Serialize(session);
			var jsonBytes = Encoding.UTF8.GetBytes(json); // Преобразуем в байты

			_mockCache.Setup(c => c.GetAsync("session:abc", default))
				.ReturnsAsync(jsonBytes); // Возвращаем байты вместо строки

			// Act
			var result = await _sessionService.GetSessionAsync("abc");

			// Assert
			Assert.NotNull(result);
			Assert.Equal("u1", result.Username);
			Assert.Equal(1, result.UserId);
		}

		[Fact]
		public async Task GetSessionAsync_ReturnsNull_WhenNotExists()
		{
			// Используем GetAsync вместо GetStringAsync
			_mockCache.Setup(c => c.GetAsync("session:missing", default))
				.ReturnsAsync((byte[]?)null);

			var result = await _sessionService.GetSessionAsync("missing");

			Assert.Null(result);
		}

		[Fact]
		public async Task ValidateSessionAsync_ReturnsTrue_WhenSessionValid()
		{
			var session = new UserSession
			{
				SessionId = "sid",
				Username = "john",
				IsActive = true,
				ExpiresAt = DateTime.UtcNow.AddHours(1)
			};

			var json = JsonSerializer.Serialize(session);
			var jsonBytes = Encoding.UTF8.GetBytes(json);

			_mockCache.Setup(c => c.GetAsync("session:sid", default))
				.ReturnsAsync(jsonBytes);

			var (isValid, username) = await _sessionService.ValidateSessionAsync("sid");

			Assert.True(isValid);
			Assert.Equal("john", username);
		}

		[Fact]
		public async Task ValidateSessionAsync_ReturnsFalse_WhenExpired()
		{
			var session = new UserSession
			{
				SessionId = "sid",
				Username = "john",
				IsActive = true,
				ExpiresAt = DateTime.UtcNow.AddHours(-1) // expired
			};

			var json = JsonSerializer.Serialize(session);
			var jsonBytes = Encoding.UTF8.GetBytes(json);

			_mockCache.Setup(c => c.GetAsync("session:sid", default))
				.ReturnsAsync(jsonBytes);

			var (isValid, username) = await _sessionService.ValidateSessionAsync("sid");

			Assert.False(isValid);
			Assert.Equal(string.Empty, username);
		}

		[Fact]
		public async Task ValidateSessionAsync_ReturnsFalse_WhenInactive()
		{
			var session = new UserSession
			{
				SessionId = "sid",
				Username = "john",
				IsActive = false,
				ExpiresAt = DateTime.UtcNow.AddHours(1)
			};

			var json = JsonSerializer.Serialize(session);
			var jsonBytes = Encoding.UTF8.GetBytes(json);

			_mockCache.Setup(c => c.GetAsync("session:sid", default))
				.ReturnsAsync(jsonBytes);

			var (isValid, _) = await _sessionService.ValidateSessionAsync("sid");

			Assert.False(isValid);
		}

		[Fact]
		public async Task ValidateSessionAsync_ReturnsFalse_WhenSessionMissing()
		{
			_mockCache.Setup(c => c.GetAsync("session:sid", default))
				.ReturnsAsync((byte[]?)null);

			var (isValid, username) = await _sessionService.ValidateSessionAsync("sid");

			Assert.False(isValid);
			Assert.Equal(string.Empty, username);
		}

		[Fact]
		public async Task DeleteSessionAsync_RemovesSessionAndReturnsTrue()
		{
			string? removedKey = null;

			_mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), default))
				.Callback<string, CancellationToken>((key, _) => removedKey = key)
				.Returns(Task.CompletedTask);

			var result = await _sessionService.DeleteSessionAsync("abc");

			Assert.True(result);
			Assert.Equal("session:abc", removedKey);

			_mockCache.Verify(c => c.RemoveAsync("session:abc", default), Times.Once);
		}
	}
}

