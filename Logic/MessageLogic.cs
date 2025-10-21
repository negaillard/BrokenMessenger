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
	public class MessageLogic : IMessageLogic
	{
		private readonly ILogger _logger;
		private readonly IMessageStorage _messageStorage;
		public MessageLogic(ILogger<MessageLogic> logger, IMessageStorage MessageStorage)
		{
			_logger = logger;
			_messageStorage = MessageStorage;
		}

		public async Task<List<MessageViewModel>?> ReadListAsync(MessageSearchModel? model)
		{
			_logger.LogInformation("ReadList. Name:{Name}.Id:{Id}", model?.Sender, model?.Id);
			var list = model == null
				? await _messageStorage.GetFullListAsync()
				: await _messageStorage.GetFilteredListAsync(model);

			if (list == null)
			{
				_logger.LogWarning("ReadList return null list");
				return null;
			}
			_logger.LogInformation("ReadList. Count:{Count}", list.Count);
			return list;
		}

		public async Task<MessageViewModel?> ReadElementAsync(MessageSearchModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			_logger.LogInformation("ReadElement. Name:{Name}.Id:{Id}", model?.Sender, model?.Id);
			var element = await _messageStorage.GetElementAsync(model);
			if (element == null)
			{
				_logger.LogWarning("ReadElement element not found");
				return null;
			}
			_logger.LogInformation("ReadElement find. Id:{Id}", element.Id);
			return element;
		}

		public async Task<bool> CreateAsync(MessageBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _messageStorage.InsertAsync(model) == null)
			{
				_logger.LogWarning("Insert operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> UpdateAsync(MessageBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _messageStorage.UpdateAsync(model) == null)
			{
				_logger.LogWarning("Update operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> DeleteAsync(MessageBindingModel model)
		{
			await CheckModelAsync(model, false);
			_logger.LogInformation("Delete. Id:{Id}", model.Id);
			if (await _messageStorage.DeleteAsync(model) == null)
			{
				_logger.LogWarning("Delete operation failed");
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
			_logger.LogInformation("Message. Name:{Name}. Id: {Id}", model.Sender, model.Id);
		}
	}
}
