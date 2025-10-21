using Client.Models.Binding;
using Client.Models.LogicContracts;
using Client.Models.Search;
using Client.Models.StorageContracts;
using Client.Models.View;
using Client.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Logic
{
	public class UserLogic : IUserLogic
	{
		private readonly IUserStorage _userStorage;
		private readonly IChatStorage _chatStorage;

		public UserLogic(string userName)
		{
			_userStorage = new UserStorage(userName);
			_chatStorage = new ChatStorage(userName);
		}

		public async Task<UserViewModel> GetOrCreateUserAsync(string username)
		{
			var search = new UserSearchModel { Username = username };
			var user = await _userStorage.GetUserAsync(search);

			if (user == null)
			{
				user = await _userStorage.CreateUserAsync(new UserBindingModel
				{
					Username = username
				});
			}

			return GetViewModel(user);
		}

		public async Task SetCurrentInterlocutorAsync(string currentUser, string interlocutor)
		{
			// Создаем чат при установке собеседника
			var chatSearch = new ChatSearchModel
			{
				CurrentUser = currentUser,
				Interlocutor = interlocutor
			};

			var chat = await _chatStorage.GetChatAsync(chatSearch);
			if (chat == null)
			{
				await _chatStorage.CreateChatAsync(new ChatBindingModel
				{
					CurrentUser = currentUser,
					Interlocutor = interlocutor
				});
			}
		}

		public async Task<UserViewModel> GetCurrentUserAsync(string username)
		{
			var user = await GetOrCreateUserAsync(username);

			//// Получаем чаты для подсчета непрочитанных
			//var chatsSearch = new ChatSearchModel { CurrentUser = username };
			//var chats = await _chatStorage.GetChatsAsync(chatsSearch);

			return user;
		}

		public UserViewModel GetViewModel(UserBindingModel model)
		{
			return new UserViewModel
			{
				Id = model.Id,
				Username = model.Username
			};
		}

		public void Dispose()
		{
			_userStorage?.Dispose();
			_chatStorage?.Dispose();
		}
	}
}
