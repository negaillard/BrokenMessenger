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
	public class UserLogic : IUserLogic
	{
		private readonly ILogger _logger;
		private readonly IUserStorage _userStorage;
		public UserLogic(ILogger<UserLogic> logger, IUserStorage UserStorage)
		{
			_logger = logger;
			_userStorage = UserStorage;
		}

		public async Task<List<UserViewModel>?> ReadListAsync(UserSearchModel? model)
		{
			_logger.LogInformation("ReadList. Name:{Name}.Id:{Id}", model?.Username, model?.Id);
			var list = model == null
				? await _userStorage.GetFullListAsync()
				: await _userStorage.GetFilteredListAsync(model);

			if (list == null)
			{
				_logger.LogWarning("ReadList return null list");
				return null;
			}
			_logger.LogInformation("ReadList. Count:{Count}", list.Count);
			return list;
		}

		public async Task<UserViewModel?> ReadElementAsync(UserSearchModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			_logger.LogInformation("ReadElement. Name:{Name}.Id:{Id}", model.Username, model.Id);
			var element = await _userStorage.GetElementAsync(model);
			if (element == null)
			{
				_logger.LogWarning("ReadElement element not found");
				return null;
			}
			_logger.LogInformation("ReadElement find. Id:{Id}", element.Id);
			return element;
		}

		public async Task<bool> CreateAsync(UserBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _userStorage.InsertAsync(model) == null)
			{
				_logger.LogWarning("Insert operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> UpdateAsync(UserBindingModel model)
		{
			await CheckModelAsync(model);
			if (await _userStorage.UpdateAsync(model) == null)
			{
				_logger.LogWarning("Update operation failed");
				return false;
			}
			return true;
		}

		public async Task<bool> DeleteAsync(UserBindingModel model)
		{
			await CheckModelAsync(model, false);
			_logger.LogInformation("Delete. Id:{Id}", model.Id);
			if (await _userStorage.DeleteAsync(model) == null)
			{
				_logger.LogWarning("Delete operation failed");
				return false;
			}
			return true;
		}

		private async Task CheckModelAsync(UserBindingModel model, bool withParams = true)
		{
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			if (!withParams)
			{
				return;
			}
			if (string.IsNullOrEmpty(model.Username))
			{
				throw new ArgumentNullException("Нет имени пользователя", nameof(model.Username));
			}
			_logger.LogInformation("User. Name:{Name}. Id: {Id}", model.Username, model.Id);
			var element = await _userStorage.GetElementAsync(new UserSearchModel
			{
				Username = model.Username,
			});

			if (element != null && element.Id != model.Id)
			{
				throw new InvalidOperationException("Такая пользователь уже есть");
			}
		}
	}
}
