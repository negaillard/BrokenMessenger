using Microsoft.Extensions.Logging;
using Models.Binding;
using Models.LogicContracts;
using Models.Pagination;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Models;
using Storage.Repositories;

namespace Logic
{
	public class ChatLogic : IChatLogic
	{
		//private readonly ILogger _logger;
		private readonly IChatStorage _chatStorage;
		public ChatLogic(
			//ILogger<ChatLogic> logger, 
			string username)
		{
			//_logger = logger;
			_chatStorage = new ChatStorage(username);
		}

		public async Task<List<ChatViewModel>?> ReadListAsync(ChatSearchModel? model)
		{
			//_logger.LogInformation("ReadList. Name:{Name}.Id:{Id}", model?.CurrentUser, model?.Id);
			var list = model == null
				? await _chatStorage.GetFullListAsync()
				: await _chatStorage.GetFilteredListAsync(model);

			if (list == null)
			{
				//_logger.LogWarning("ReadList return null list");
				return null;
			}
			//_logger.LogInformation("ReadList. Count:{Count}", list.Count);
			return list;
		}

		public async Task<ChatViewModel?> ReadElementAsync(ChatSearchModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			//_logger.LogInformation("ReadElement. Name:{Name}.Id:{Id}", model.CurrentUser, model.Id);
			var element = await _chatStorage.GetElementAsync(model);
			if (element == null)
			{
				//_logger.LogWarning("ReadElement element not found");
				return null;
			}
			//_logger.LogInformation("ReadElement find. Id:{Id}", element.Id);
			return element;
		}

		public async Task<bool> CreateAsync(ChatBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _chatStorage.InsertAsync(model) == null)
			{
				//_logger.LogWarning("Insert operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> UpdateAsync(ChatBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _chatStorage.UpdateAsync(model) == null)
			{
				//_logger.LogWarning("Update operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> DeleteAsync(ChatBindingModel model)
		{
			await CheckModelAsync(model, false);
			//_logger.LogInformation("Delete. Id:{Id}", model.Id);
			if (await _chatStorage.DeleteAsync(model) == null)
			{
				//_logger.LogWarning("Delete operation failed");
				return false;
			}
			return true;
		}

		private async Task CheckModelAsync(ChatBindingModel model, bool withParams = true)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			if (!withParams)
			{
				return;
			}
			if (string.IsNullOrEmpty(model.CurrentUser))
			{
				throw new ArgumentNullException("Нет имени пользователя", nameof(model.CurrentUser));
			}
			if (string.IsNullOrEmpty(model.Interlocutor))
			{
				throw new ArgumentNullException("Нет имени собеседника", nameof(model.Interlocutor));
			}
			//_logger.LogInformation("Chat. Name:{Name}. Id: {Id}", model.CurrentUser, model.Id);
			var element = await _chatStorage.GetElementAsync(new ChatSearchModel
			{
				CurrentUser = model.CurrentUser,
				Interlocutor = model.Interlocutor,
			});

			if (element != null && element.Id != model.Id)
			{
				throw new InvalidOperationException("Такая чат уже есть");
			}
		}

		public async Task<PaginatedResult<ChatViewModel>> GetRecentChatsAsync(int page = 1, int pageSize = 30)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 30;

			return await _chatStorage.GetRecentChatsAsync(page, pageSize);
		}

		public async Task<PaginatedResult<ChatViewModel>> SearchChatsAsync(
			string interlocutorName,
			int page = 1,
			int pageSize = 30)
		{
			if (page < 1) page = 1;
			if (pageSize < 1) pageSize = 30;

			var searchModel = new ChatSearchModel
			{
				Interlocutor = interlocutorName
				// Можно добавить и другие фильтры при необходимости
			};

			return await _chatStorage.SearchChatsAsync(searchModel, page, pageSize);
		}
	}
}
