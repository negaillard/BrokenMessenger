using AuthServerAPI.Logic;
using AuthServerAPI.Models;
using AuthServerAPI.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthServerTests
{
	public class UserLogicTests
	{
		private readonly Mock<IUserStorage> _mockUserStorage;
		private readonly Mock<ILogger<UserLogic>> _mockLogger;
		private readonly UserLogic _userLogic;

		public UserLogicTests()
		{
			_mockUserStorage = new Mock<IUserStorage>();
			_mockLogger = new Mock<ILogger<UserLogic>>();
			_userLogic = new UserLogic(_mockLogger.Object, _mockUserStorage.Object);
		}

		[Fact]
		public async Task ReadListAsync_WithNullModel_ReturnsFullList()
		{
			// Arrange
			var expected = new List<UserBindingModel>
			{
				new UserBindingModel{ Id = 1, Username = "u1", Email = "e1" },
				new UserBindingModel{ Id = 2, Username = "u2", Email = "e2" }
			};

			_mockUserStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync(expected);

			// Act
			var result = await _userLogic.ReadListAsync(null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			_mockUserStorage.Verify(x => x.GetFullListAsync(), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WithModel_ReturnsFilteredList()
		{
			// Arrange
			var model = new UserSearchModel { Username = "test" };
			var expected = new List<UserBindingModel>
			{
				new UserBindingModel{ Id = 1, Username = "test", Email = "e1" }
			};

			_mockUserStorage.Setup(x => x.GetFilteredListAsync(model))
				.ReturnsAsync(expected);

			// Act
			var result = await _userLogic.ReadListAsync(model);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			_mockUserStorage.Verify(x => x.GetFilteredListAsync(model), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WhenStorageReturnsNull_ReturnsNull()
		{
			_mockUserStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync((List<UserBindingModel>?)null);

			var result = await _userLogic.ReadListAsync(null);

			Assert.Null(result);
		}


		[Fact]
		public async Task ReadElementAsync_WithValidModel_ReturnsElement()
		{
			var model = new UserSearchModel { Id = 1 };
			var expected = new UserBindingModel { Id = 1, Username = "u1", Email = "e1" };

			_mockUserStorage.Setup(x => x.GetElementAsync(model))
				.ReturnsAsync(expected);

			var result = await _userLogic.ReadElementAsync(model);

			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
		}

		[Fact]
		public async Task ReadElementAsync_WithNullModel_ThrowsArgumentNullException()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				_userLogic.ReadElementAsync(null!));
		}

		[Fact]
		public async Task ReadElementAsync_WhenNotFound_ReturnsNull()
		{
			var model = new UserSearchModel { Id = 999 };

			_mockUserStorage.Setup(x => x.GetElementAsync(model))
				.ReturnsAsync((UserBindingModel?)null);

			var result = await _userLogic.ReadElementAsync(model);

			Assert.Null(result);
		}


		[Fact]
		public async Task CreateAsync_WithValidModel_ReturnsTrue()
		{
			var model = new UserBindingModel { Username = "u1", Email = "e1" };

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserBindingModel?)null);

			_mockUserStorage.Setup(x => x.InsertElementAsync(model))
				.ReturnsAsync(new UserBindingModel());

			var result = await _userLogic.CreateAsync(model);

			Assert.True(result);
			_mockUserStorage.Verify(x => x.InsertElementAsync(model), Times.Once);
		}

		[Fact]
		public async Task CreateAsync_WithNullModel_ThrowsArgumentNullException()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				_userLogic.CreateAsync(null!));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyUsername_ThrowsArgumentNullException()
		{
			var model = new UserBindingModel { Username = "", Email = "email" };

			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				_userLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyEmail_ThrowsArgumentNullException()
		{
			var model = new UserBindingModel { Username = "u1", Email = "" };

			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				_userLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenUserAlreadyExists_ThrowsInvalidOperationException()
		{
			var model = new UserBindingModel { Username = "u1", Email = "e1" };
			var existing = new UserBindingModel { Id = 5, Username = "u1", Email = "e1" };

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync(existing);

			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				_userLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenInsertFails_ReturnsFalse()
		{
			var model = new UserBindingModel { Username = "u1", Email = "e1" };

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserBindingModel?)null);

			_mockUserStorage.Setup(x => x.InsertElementAsync(model))
				.ReturnsAsync((UserBindingModel?)null);

			var result = await _userLogic.CreateAsync(model);

			Assert.False(result);
		}


		[Fact]
		public async Task UpdateAsync_WithValidModel_ReturnsTrue()
		{
			var model = new UserBindingModel { Id = 1, Username = "u1", Email = "e1" };

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserBindingModel?)null);

			_mockUserStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync(new UserBindingModel());

			var result = await _userLogic.UpdateAsync(model);

			Assert.True(result);
		}

		[Fact]
		public async Task UpdateAsync_WhenUpdateFails_ReturnsFalse()
		{
			var model = new UserBindingModel { Id = 1, Username = "u1", Email = "e1" };

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserBindingModel?)null);

			_mockUserStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync((UserBindingModel?)null);

			var result = await _userLogic.UpdateAsync(model);

			Assert.False(result);
		}


		[Fact]
		public async Task DeleteAsync_WithValidModel_ReturnsTrue()
		{
			var model = new UserBindingModel { Id = 1 };

			_mockUserStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync(new UserBindingModel());

			var result = await _userLogic.DeleteAsync(model);

			Assert.True(result);
		}

		[Fact]
		public async Task DeleteAsync_WhenDeleteFails_ReturnsFalse()
		{
			var model = new UserBindingModel { Id = 1 };

			_mockUserStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync((UserBindingModel?)null);

			var result = await _userLogic.DeleteAsync(model);

			Assert.False(result);
		}

		[Fact]
		public async Task SearchUsersAsync_WithValidParameters_ReturnsPaginatedResult()
		{
			var expected = new PaginatedResult<UserBindingModel>
			{
				Items = new List<UserBindingModel>
				{
					new UserBindingModel { Id = 1, Username = "u1", Email = "e1" }
				},
				Page = 1,
				PageSize = 30,
				TotalCount = 1,
				TotalPages = 1
			};

			_mockUserStorage.Setup(x =>
				x.SearchUsersAsync(It.IsAny<UserSearchModel>(), 1, 30))
				.ReturnsAsync(expected);

			var result = await _userLogic.SearchUsersAsync("u1", 1, 30);

			Assert.NotNull(result);
			Assert.Single(result.Items);
			_mockUserStorage.Verify(x =>
				x.SearchUsersAsync(It.Is<UserSearchModel>(m => m.Username == "u1"), 1, 30),
				Times.Once);
		}
	}
}

