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
	public class MessageLogicTests
	{
		private readonly Mock<IMessageStorage> _mockMessageStorage;
		private const string TestUsername = "testuser";

		public MessageLogicTests()
		{
			_mockMessageStorage = new Mock<IMessageStorage>();
		}

		[Fact]
		public async Task ReadListAsync_WithNullModel_ReturnsFullList()
		{
			// Arrange
			var expectedMessages = new List<MessageViewModel>
			{
				new MessageViewModel { Id = 1, Sender = "user1", Recipient = "user2", Content = "Hello" },
				new MessageViewModel { Id = 2, Sender = "user2", Recipient = "user1", Content = "Hi" }
			};

			_mockMessageStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync(expectedMessages);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.ReadListAsync(null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			_mockMessageStorage.Verify(x => x.GetFullListAsync(), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WithModel_ReturnsFilteredList()
		{
			// Arrange
			var searchModel = new MessageSearchModel { Sender = "user1" };
			var expectedMessages = new List<MessageViewModel>
			{
				new MessageViewModel { Id = 1, Sender = "user1", Recipient = "user2", Content = "Hello" }
			};

			_mockMessageStorage.Setup(x => x.GetFilteredListAsync(searchModel))
				.ReturnsAsync(expectedMessages);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.ReadListAsync(searchModel);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			_mockMessageStorage.Verify(x => x.GetFilteredListAsync(searchModel), Times.Once);
		}

		[Fact]
		public async Task ReadListAsync_WhenStorageReturnsNull_ReturnsNull()
		{
			// Arrange
			_mockMessageStorage.Setup(x => x.GetFullListAsync())
				.ReturnsAsync((List<MessageViewModel>?)null);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.ReadListAsync(null);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task ReadElementAsync_WithValidModel_ReturnsElement()
		{
			// Arrange
			var searchModel = new MessageSearchModel { Id = 1 };
			var expectedMessage = new MessageViewModel 
			{ 
				Id = 1, 
				Sender = "user1", 
				Recipient = "user2", 
				Content = "Hello",
				Timestamp = DateTime.Now
			};

			_mockMessageStorage.Setup(x => x.GetElementAsync(searchModel))
				.ReturnsAsync(expectedMessage);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.ReadElementAsync(searchModel);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			_mockMessageStorage.Verify(x => x.GetElementAsync(searchModel), Times.Once);
		}

		[Fact]
		public async Task ReadElementAsync_WithNullModel_ThrowsArgumentNullException()
		{
			// Arrange
			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				messageLogic.ReadElementAsync(null!));
		}

		[Fact]
		public async Task ReadElementAsync_WhenElementNotFound_ReturnsNull()
		{
			// Arrange
			var searchModel = new MessageSearchModel { Id = 999 };
			_mockMessageStorage.Setup(x => x.GetElementAsync(searchModel))
				.ReturnsAsync((MessageViewModel?)null);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.ReadElementAsync(searchModel);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task CreateAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Sender = "user1",
				Recipient = "user2",
				Content = "Hello",
				ChatId = 1
			};

			var createdMessage = new MessageViewModel 
			{ 
				Id = 1, 
				Sender = "user1", 
				Recipient = "user2", 
				Content = "Hello",
				ChatId = 1
			};

			_mockMessageStorage.Setup(x => x.InsertAsync(model))
				.ReturnsAsync(createdMessage);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.CreateAsync(model);

			// Assert
			Assert.True(result);
			_mockMessageStorage.Verify(x => x.InsertAsync(model), Times.Once);
		}

		[Fact]
		public async Task CreateAsync_WithNullModel_ThrowsArgumentNullException()
		{
			// Arrange
			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				messageLogic.CreateAsync(null!));
		}

		[Fact]
		public async Task CreateAsync_WithEmptySender_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Sender = string.Empty,
				Recipient = "user2",
				Content = "Hello"
			};

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				messageLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyRecipient_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Sender = "user1",
				Recipient = string.Empty,
				Content = "Hello"
			};

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				messageLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WithEmptyContent_ThrowsArgumentNullException()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Sender = "user1",
				Recipient = "user2",
				Content = string.Empty
			};

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => 
				messageLogic.CreateAsync(model));
		}

		[Fact]
		public async Task CreateAsync_WhenInsertFails_ReturnsFalse()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Sender = "user1",
				Recipient = "user2",
				Content = "Hello"
			};

			_mockMessageStorage.Setup(x => x.InsertAsync(model))
				.ReturnsAsync((MessageViewModel?)null);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.CreateAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task UpdateAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Id = 1,
				Sender = "user1",
				Recipient = "user2",
				Content = "Updated message"
			};

			var updatedMessage = new MessageViewModel 
			{ 
				Id = 1, 
				Sender = "user1", 
				Recipient = "user2", 
				Content = "Updated message"
			};

			_mockMessageStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync(updatedMessage);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.UpdateAsync(model);

			// Assert
			Assert.True(result);
			_mockMessageStorage.Verify(x => x.UpdateAsync(model), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_WhenUpdateFails_ReturnsFalse()
		{
			// Arrange
			var model = new MessageBindingModel
			{
				Id = 1,
				Sender = "user1",
				Recipient = "user2",
				Content = "Updated message"
			};

			_mockMessageStorage.Setup(x => x.UpdateAsync(model))
				.ReturnsAsync((MessageViewModel?)null);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.UpdateAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task DeleteAsync_WithValidModel_ReturnsTrue()
		{
			// Arrange
			var model = new MessageBindingModel { Id = 1 };
			var deletedMessage = new MessageViewModel { Id = 1 };

			_mockMessageStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync(deletedMessage);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.DeleteAsync(model);

			// Assert
			Assert.True(result);
			_mockMessageStorage.Verify(x => x.DeleteAsync(model), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_WhenDeleteFails_ReturnsFalse()
		{
			// Arrange
			var model = new MessageBindingModel { Id = 1 };

			_mockMessageStorage.Setup(x => x.DeleteAsync(model))
				.ReturnsAsync((MessageViewModel?)null);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.DeleteAsync(model);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task GetMessagesByChatIdAsync_WithValidChatId_ReturnsPaginatedResult()
		{
			// Arrange
			var expectedResult = new PaginatedResult<MessageViewModel>
			{
				Items = new List<MessageViewModel>
				{
					new MessageViewModel { Id = 1, Sender = "user1", Recipient = "user2", Content = "Hello", ChatId = 1 }
				},
				Page = 1,
				PageSize = 50,
				TotalCount = 1,
				TotalPages = 1
			};

			_mockMessageStorage.Setup(x => x.GetMessagesByChatIdAsync(1, 1, 50))
				.ReturnsAsync(expectedResult);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.GetMessagesByChatIdAsync(1, 1, 50);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Items.Count);
			_mockMessageStorage.Verify(x => x.GetMessagesByChatIdAsync(1, 1, 50), Times.Once);
		}

		[Fact]
		public async Task GetMessagesByChatIdAsync_WithInvalidChatId_ThrowsArgumentException()
		{
			// Arrange
			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(() => 
				messageLogic.GetMessagesByChatIdAsync(0, 1, 50));
		}

		[Fact]
		public async Task SearchMessagesAsync_WithValidModel_ReturnsPaginatedResult()
		{
			// Arrange
			var searchModel = new MessageSearchModel { ChatId = 1 };
			var expectedResult = new PaginatedResult<MessageViewModel>
			{
				Items = new List<MessageViewModel>
				{
					new MessageViewModel { Id = 1, Sender = "user1", Recipient = "user2", Content = "Hello", ChatId = 1 }
				},
				Page = 1,
				PageSize = 50,
				TotalCount = 1,
				TotalPages = 1
			};

			_mockMessageStorage.Setup(x => x.SearchMessagesAsync(searchModel, 1, 50))
				.ReturnsAsync(expectedResult);

			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act
			var result = await messageLogic.SearchMessagesAsync(searchModel, 1, 50);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Items.Count);
			_mockMessageStorage.Verify(x => x.SearchMessagesAsync(searchModel, 1, 50), Times.Once);
		}

		[Fact]
		public async Task SearchMessagesAsync_WithInvalidChatId_ThrowsArgumentException()
		{
			// Arrange
			var searchModel = new MessageSearchModel { ChatId = 0 };
			var messageLogic = new MessageLogic(_mockMessageStorage.Object);

			// Act & Assert
			await Assert.ThrowsAsync<ArgumentException>(() => 
				messageLogic.SearchMessagesAsync(searchModel, 1, 50));
		}
	}
}
