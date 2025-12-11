using Logic;
using Models.Binding;
using Models.LogicContracts;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Moq;
using Xunit;

namespace Logic.Tests
{
	public class UserLogicTests
	{
		private readonly Mock<IUserStorage> _mockUserStorage;
		private const string TestUsername = "testuser";

		public UserLogicTests()
		{
			_mockUserStorage = new Mock<IUserStorage>();
		}

		[Fact]
		public async Task ReadListAsync_WithNullModel_ReturnsFullList()
		{
			// Arrange
			var expectedUsers = new List<UserViewModel>
			{
				new UserViewModel { Id = 1, Username = "user1", Email = "user1@example.com" },
				new UserViewModel { Id = 2, Username = "user2", Email = "user2@example.com" }
			};

			_mockUserStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync(expectedUsers);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.ReadListAsync(null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			_mockUserStorage.Verify(x => x.GetFullListAsync(), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WithModel_ReturnsFilteredList()
		{
			// Arrange
			var searchModel = new UserSearchModel { Username = "user1" };
			var expectedUsers = new List<UserViewModel>
			{
				new UserViewModel { Id = 1, Username = "user1", Email = "user1@example.com" }
			};

			_mockUserStorage.Setup(x => x.GetFilteredListAsync(searchModel))
				.ReturnsAsync(expectedUsers);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.ReadListAsync(searchModel);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			_mockUserStorage.Verify(x => x.GetFilteredListAsync(searchModel), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WhenStorageReturnsNull_ReturnsNull()
		{
			// Arrange
			_mockUserStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync((List<UserViewModel>?)null);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.ReadListAsync(null);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task ReadElementAsync_WithValidModel_ReturnsElement()
		{
			// Arrange
			var searchModel = new UserSearchModel { Id = 1 };
			var expectedUser = new UserViewModel 
			{ 
				Id = 1, 
				Username = "user1", 
				Email = "user1@example.com" 
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(searchModel))
				.ReturnsAsync(expectedUser);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.ReadElementAsync(searchModel);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal("user1", result.Username);
			_mockUserStorage.Verify(x => x.GetElementAsync(searchModel), Times.Once);
		}

		[Fact]
		public async Task ReadElementAsync_WithNullModel_ThrowsArgumentNullException()
		{
			// Arrange
			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				userLogic.ReadElementAsync(null!));
		}

		[Fact]
		public async Task ReadElementAsync_WhenElementNotFound_ReturnsNull()
		{
			// Arrange
			var searchModel = new UserSearchModel { Id = 999 };
			_mockUserStorage.Setup(x => x.GetElementAsync(searchModel))
				.ReturnsAsync((UserViewModel?)null);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.ReadElementAsync(searchModel);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task CreateAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Username = "newuser",
				Email = "newuser@example.com"
			};

			var createdUser = new UserViewModel 
			{ 
				Id = 1, 
				Username = "newuser", 
				Email = "newuser@example.com" 
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserViewModel?)null);
			_mockUserStorage.Setup(x => x.InsertAsync(model))
				.ReturnsAsync(createdUser);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.CreateAsync(model);

			// Assert
			Assert.True(result);
			_mockUserStorage.Verify(x => x.InsertAsync(model), Times.Once);
		}

		[Fact]
		public async Task CreateAsync_WithNullModel_ThrowsArgumentNullException()
		{
			// Arrange
			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				userLogic.CreateAsync(null!));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyUsername_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Username = string.Empty,
				Email = "user@example.com"
			};

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				userLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyEmail_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Username = "user1",
				Email = string.Empty
			};

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				userLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenUserAlreadyExists_ThrowsInvalidOperationException()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Id = 0,
				Username = "existinguser",
				Email = "existinguser@example.com"
			};

			var existingUser = new UserViewModel 
			{ 
				Id = 1, 
				Username = "existinguser", 
				Email = "existinguser@example.com" 
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync(existingUser);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => 
				userLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenInsertFails_ReturnsFalse()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Username = "newuser",
				Email = "newuser@example.com"
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserViewModel?)null);
			_mockUserStorage.Setup(x => x.InsertAsync(model))
				.ReturnsAsync((UserViewModel?)null);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.CreateAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UpdateAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Id = 1,
				Username = "updateduser",
				Email = "updateduser@example.com"
			};

			var updatedUser = new UserViewModel 
			{ 
				Id = 1, 
				Username = "updateduser", 
				Email = "updateduser@example.com" 
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserViewModel?)null);
			_mockUserStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync(updatedUser);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.UpdateAsync(model);

			// Assert
			Assert.True(result);
			_mockUserStorage.Verify(x => x.UpdateAsync(model), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_WhenUpdateFails_ReturnsFalse()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Id = 1,
				Username = "updateduser",
				Email = "updateduser@example.com"
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync((UserViewModel?)null);
			_mockUserStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync((UserViewModel?)null);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.UpdateAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task DeleteAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new UserBindingModel { Id = 1 };
			var deletedUser = new UserViewModel { Id = 1 };

			_mockUserStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync(deletedUser);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.DeleteAsync(model);

			// Assert
			Assert.True(result);
			_mockUserStorage.Verify(x => x.DeleteAsync(model), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_WhenDeleteFails_ReturnsFalse()
		{
			// Arrange
			var model = new UserBindingModel { Id = 1 };

			_mockUserStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync((UserViewModel?)null);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act
			var result = await userLogic.DeleteAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UpdateAsync_WhenUserWithSameUsernameExists_ThrowsInvalidOperationException()
		{
			// Arrange
			var model = new UserBindingModel
			{
				Id = 1,
				Username = "existinguser",
				Email = "existinguser@example.com"
			};

			var existingUser = new UserViewModel 
			{ 
				Id = 2, // Different ID
				Username = "existinguser", 
				Email = "existinguser@example.com" 
			};

			_mockUserStorage.Setup(x => x.GetElementAsync(It.IsAny<UserSearchModel>()))
				.ReturnsAsync(existingUser);

			var userLogic = new UserLogic(_mockUserStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => 
				userLogic.UpdateAsync(model));
		}
	}
}
