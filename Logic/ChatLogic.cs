using Microsoft.Extensions.Logging;
using Models.Binding;
using Models.LogicContracts;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
	public class ChatLogic : IChatLogic
	{
		private readonly ILogger _logger;
		private readonly IChatStorage _chatStorage;
		public ChatLogic(ILogger<ChatLogic> logger, IChatStorage ChatStorage)
		{
			_logger = logger;
			_chatStorage = ChatStorage;
		}

		public async Task<List<ChatViewModel>?> ReadListAsync(ChatSearchModel? model)
		{
			_logger.LogInformation("ReadList. Name:{Name}.Id:{Id}", model?.Chatname, model?.Id);
			var list = model == null
				? await _chatStorage.GetFullListAsync()
				: await _chatStorage.GetFilteredListAsync(model);

			if (list == null)
			{
				_logger.LogWarning("ReadList return null list");
				return null;
			}
			_logger.LogInformation("ReadList. Count:{Count}", list.Count);
			return list;
		}

		public async Task<ChatViewModel?> ReadElementAsync(ChatSearchModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			_logger.LogInformation("ReadElement. Name:{Name}.Id:{Id}", model.Chatname, model.Id);
			var element = await _chatStorage.GetElementAsync(model);
			if (element == null)
			{
				_logger.LogWarning("ReadElement element not found");
				return null;
			}
			_logger.LogInformation("ReadElement find. Id:{Id}", element.Id);
			return element;
		}

		public async Task<bool> CreateAsync(ChatBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _chatStorage.InsertAsync(model) == null)
			{
				_logger.LogWarning("Insert operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> UpdateAsync(ChatBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _chatStorage.UpdateAsync(model) == null)
			{
				_logger.LogWarning("Update operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> DeleteAsync(ChatBindingModel model)
		{
			await CheckModelAsync(model, false);
			_logger.LogInformation("Delete. Id:{Id}", model.Id);
			if (await _chatStorage.DeleteAsync(model) == null)
			{
				_logger.LogWarning("Delete operation failed");
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
			if (string.IsNullOrEmpty(model.Chatname))
			{
				throw new ArgumentNullException("Нет имени пользователя", nameof(model.Chatname));
			}
			_logger.LogInformation("Chat. Name:{Name}. Id: {Id}", model.Username, model.Id);
			var element = await _chatStorage.GetElementAsync(new UserSearchModel
			{
				Username = model.Username,
			});

			if (element != null && element.Id != model.Id)
			{
				throw new InvalidOperationException("Такая кафедра на факультете уже есть");
			}
		}
	}
}
