using Logic;
using Models.Binding;
using Models.LogicContracts;
using Models.Pagination;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Moq;
using Xunit;

namespace Logic.Tests
{
	public class ChatLogicTests
	{
		private readonly Mock<IChatStorage> _mockChatStorage;
		private readonly ChatLogic _chatLogic;
		private const string TestUsername = "testuser";

		public ChatLogicTests()
		{
			_mockChatStorage = new Mock<IChatStorage>();
		}

		[Fact]
		public async Task ReadListAsync_WithNullModel_ReturnsFullList()
		{
			// Arrange
			var expectedChats = new List<ChatViewModel>
			{
				new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" },
				new ChatViewModel { Id = 2, CurrentUser = "user1", Interlocutor = "user3" }
			};

			_mockChatStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync(expectedChats);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.ReadListAsync(null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			_mockChatStorage.Verify(x => x.GetFullListAsync(), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WithModel_ReturnsFilteredList()
		{
			// Arrange
			var searchModel = new ChatSearchModel { CurrentUser = "user1" };
			var expectedChats = new List<ChatViewModel>
			{
				new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" }
			};

			_mockChatStorage.Setup(x => x.GetFilteredListAsync(searchModel))
				.ReturnsAsync(expectedChats);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.ReadListAsync(searchModel);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			_mockChatStorage.Verify(x => x.GetFilteredListAsync(searchModel), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WhenStorageReturnsNull_ReturnsNull()
		{
			// Arrange
			_mockChatStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync((List<ChatViewModel>?)null);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.ReadListAsync(null);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task ReadElementAsync_WithValidModel_ReturnsElement()
		{
			// Arrange
			var searchModel = new ChatSearchModel { Id = 1, CurrentUser = "user1" };
			var expectedChat = new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" };

			_mockChatStorage.Setup(x => x.GetElementAsync(searchModel))
				.ReturnsAsync(expectedChat);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.ReadElementAsync(searchModel);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			_mockChatStorage.Verify(x => x.GetElementAsync(searchModel), Times.Once);
		}

		[Fact]
		public async Task ReadElementAsync_WithNullModel_ThrowsArgumentNullException()
		{
			// Arrange
			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				chatLogic.ReadElementAsync(null!));
		}

		[Fact]
		public async Task ReadElementAsync_WhenElementNotFound_ReturnsNull()
		{
			// Arrange
			var searchModel = new ChatSearchModel { Id = 999 };
			_mockChatStorage.Setup(x => x.GetElementAsync(searchModel))
				.ReturnsAsync((ChatViewModel?)null);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.ReadElementAsync(searchModel);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task CreateAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				CurrentUser = "user1",
				Interlocutor = "user2"
			};

			var createdChat = new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" };

			_mockChatStorage.Setup(x => x.GetElementAsync(It.IsAny<ChatSearchModel>()))
				.ReturnsAsync((ChatViewModel?)null);
			_mockChatStorage.Setup(x => x.InsertAsync(model))
				.ReturnsAsync(createdChat);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.CreateAsync(model);

			// Assert
			Assert.True(result);
			_mockChatStorage.Verify(x => x.InsertAsync(model), Times.Once);
		}

		[Fact]
		public async Task CreateAsync_WithNullModel_ThrowsArgumentNullException()
		{
			// Arrange
			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				chatLogic.CreateAsync(null!));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyCurrentUser_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				CurrentUser = string.Empty,
				Interlocutor = "user2"
			};

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				chatLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyInterlocutor_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				CurrentUser = "user1",
				Interlocutor = string.Empty
			};

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				chatLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenChatAlreadyExists_ThrowsInvalidOperationException()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				Id = 0,
				CurrentUser = "user1",
				Interlocutor = "user2"
			};

			var existingChat = new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" };

			_mockChatStorage.Setup(x => x.GetElementAsync(It.IsAny<ChatSearchModel>()))
				.ReturnsAsync(existingChat);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => 
				chatLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenInsertFails_ReturnsFalse()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				CurrentUser = "user1",
				Interlocutor = "user2"
			};

			_mockChatStorage.Setup(x => x.GetElementAsync(It.IsAny<ChatSearchModel>()))
				.ReturnsAsync((ChatViewModel?)null);
			_mockChatStorage.Setup(x => x.InsertAsync(model))
				.ReturnsAsync((ChatViewModel?)null);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.CreateAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UpdateAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				Id = 1,
				CurrentUser = "user1",
				Interlocutor = "user2"
			};

			var updatedChat = new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" };

			_mockChatStorage.Setup(x => x.GetElementAsync(It.IsAny<ChatSearchModel>()))
				.ReturnsAsync((ChatViewModel?)null);
			_mockChatStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync(updatedChat);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.UpdateAsync(model);

			// Assert
			Assert.True(result);
			_mockChatStorage.Verify(x => x.UpdateAsync(model), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_WhenUpdateFails_ReturnsFalse()
		{
			// Arrange
			var model = new ChatBindingModel
			{
				Id = 1,
				CurrentUser = "user1",
				Interlocutor = "user2"
			};

			_mockChatStorage.Setup(x => x.GetElementAsync(It.IsAny<ChatSearchModel>()))
				.ReturnsAsync((ChatViewModel?)null);
			_mockChatStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync((ChatViewModel?)null);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.UpdateAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task DeleteAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new ChatBindingModel { Id = 1 };
			var deletedChat = new ChatViewModel { Id = 1 };

			_mockChatStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync(deletedChat);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.DeleteAsync(model);

			// Assert
			Assert.True(result);
			_mockChatStorage.Verify(x => x.DeleteAsync(model), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_WhenDeleteFails_ReturnsFalse()
		{
			// Arrange
			var model = new ChatBindingModel { Id = 1 };

			_mockChatStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync((ChatViewModel?)null);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.DeleteAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task GetRecentChatsAsync_WithValidParameters_ReturnsPaginatedResult()
		{
			// Arrange
			var expectedResult = new PaginatedResult<ChatViewModel>
			{
				Items = new List<ChatViewModel>
				{
					new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" }
				},
				Page = 1,
				PageSize = 30,
				TotalCount = 1,
				TotalPages = 1
			};

			_mockChatStorage.Setup(x => x.GetRecentChatsAsync(1, 30))
				.ReturnsAsync(expectedResult);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.GetRecentChatsAsync(1, 30);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Items.Count);
			_mockChatStorage.Verify(x => x.GetRecentChatsAsync(1, 30), Times.Once);
		}

		[Fact]
		public async Task GetRecentChatsAsync_WithInvalidPage_CorrectsToPage1()
		{
			// Arrange
			var expectedResult = new PaginatedResult<ChatViewModel>
			{
				Items = new List<ChatViewModel>(),
				Page = 1,
				PageSize = 30,
				TotalCount = 0,
				TotalPages = 0
			};

			_mockChatStorage.Setup(x => x.GetRecentChatsAsync(1, 30))
				.ReturnsAsync(expectedResult);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.GetRecentChatsAsync(0, 30);

			// Assert
			_mockChatStorage.Verify(x => x.GetRecentChatsAsync(1, 30), Times.Once);
		}

		[Fact]
		public async Task GetRecentChatsAsync_WithInvalidPageSize_CorrectsToDefault()
		{
			// Arrange
			var expectedResult = new PaginatedResult<ChatViewModel>
			{
				Items = new List<ChatViewModel>(),
				Page = 1,
				PageSize = 30,
				TotalCount = 0,
				TotalPages = 0
			};

			_mockChatStorage.Setup(x => x.GetRecentChatsAsync(1, 30))
				.ReturnsAsync(expectedResult);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.GetRecentChatsAsync(1, 0);

			// Assert
			_mockChatStorage.Verify(x => x.GetRecentChatsAsync(1, 30), Times.Once);
		}

		[Fact]
		public async Task SearchChatsAsync_WithValidParameters_ReturnsPaginatedResult()
		{
			// Arrange
			var expectedResult = new PaginatedResult<ChatViewModel>
			{
				Items = new List<ChatViewModel>
				{
					new ChatViewModel { Id = 1, CurrentUser = "user1", Interlocutor = "user2" }
				},
				Page = 1,
				PageSize = 30,
				TotalCount = 1,
				TotalPages = 1
			};

			_mockChatStorage.Setup(x => x.SearchChatsAsync(It.IsAny<ChatSearchModel>(), 1, 30))
				.ReturnsAsync(expectedResult);

			var chatLogic = new ChatLogic(_mockChatStorage.Object);

			// Act
			var result = await chatLogic.SearchChatsAsync("user2", 1, 30);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Items.Count);
			_mockChatStorage.Verify(x => x.SearchChatsAsync(
				It.Is<ChatSearchModel>(m => m.Interlocutor == "user2"), 1, 30), Times.Once);
		}
	}
}
