using Models.Binding;
using Models.LogicContracts;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Repositories;
using Microsoft.Extensions.Logging;
using Models.Pagination;

namespace Logic
{
	public class MessageLogic : IMessageLogic
	{
		//private readonly ILogger _logger;
		private readonly IMessageStorage _messageStorage;
		
		public MessageLogic(
			//ILogger<MessageLogic> logger, 
			string username)
		{
			//_logger = logger;
			_messageStorage = new MessageStorage(username);
		}

		// Constructor for dependency injection (for testing)
		public MessageLogic(IMessageStorage messageStorage)
		{
			_messageStorage = messageStorage;
		}

		public async Task<List<MessageViewModel>?> ReadListAsync(MessageSearchModel? model)
		{
			//_logger.LogInformation("ReadList. Name:{Name}.Id:{Id}", model?.Sender, model?.Id);
			var list = model == null
				? await _messageStorage.GetFullListAsync()
				: await _messageStorage.GetFilteredListAsync(model);

			if (list == null)
			{
				//_logger.LogWarning("ReadList return null list");
				return null;
			}
			//_logger.LogInformation("ReadList. Count:{Count}", list.Count);
			return list;
		}

		public async Task<MessageViewModel?> ReadElementAsync(MessageSearchModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			//_logger.LogInformation("ReadElement. Name:{Name}.Id:{Id}", model?.Sender, model?.Id);
			var element = await _messageStorage.GetElementAsync(model);
			if (element == null)
			{
				//_logger.LogWarning("ReadElement element not found");
				return null;
			}
			//_logger.LogInformation("ReadElement find. Id:{Id}", element.Id);
			return element;
		}

		public async Task<bool> CreateAsync(MessageBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _messageStorage.InsertAsync(model) == null)
			{
				//_logger.LogWarning("Insert operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> UpdateAsync(MessageBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _messageStorage.UpdateAsync(model) == null)
			{
				//_logger.LogWarning("Update operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> DeleteAsync(MessageBindingModel model)
		{
			await CheckModelAsync(model, false);
			//_logger.LogInformation("Delete. Id:{Id}", model.Id);
			if (await _messageStorage.DeleteAsync(model) == null)
			{
				//_logger.LogWarning("Delete operation failed");
				return false;
			}
			return true;
		}

		private async Task CheckModelAsync(MessageBindingModel model, bool withParams = true)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			if (!withParams)
			{
				return;
			}
			if (string.IsNullOrEmpty(model.Sender))
			{
				throw new ArgumentNullException("Нет отправителя", nameof(model.Sender));
			}
			if (string.IsNullOrEmpty(model.Recipient))
			{
				throw new ArgumentNullException("Нет получателя", nameof(model.Recipient));
			}
			if (string.IsNullOrEmpty(model.Content))
			{
				throw new ArgumentNullException("Нет содержания", nameof(model.Content));
			}
			//_logger.LogInformation("Message. Name:{Name}. Id: {Id}", model.Sender, model.Id);
		}

		public async Task<PaginatedResult<MessageViewModel>> GetMessagesByChatIdAsync(
	   int chatId,
	   int page = 1,
	   int pageSize = 50)
		{
			if (chatId <= 0)
				throw new ArgumentException("Invalid chat ID");

			return await _messageStorage.GetMessagesByChatIdAsync(chatId, page, pageSize);
		}

		public async Task<PaginatedResult<MessageViewModel>> SearchMessagesAsync(
			MessageSearchModel searchModel,
			int page = 1,
			int pageSize = 50)
		{
			if (searchModel.ChatId.HasValue && searchModel.ChatId <= 0)
				throw new ArgumentException("Invalid chat ID");

			return await _messageStorage.SearchMessagesAsync(searchModel, page, pageSize);
		}
	}
}
