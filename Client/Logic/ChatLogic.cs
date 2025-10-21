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
	public class ChatLogic : IChatLogic
	{
		private readonly IChatStorage _chatStorage;
		private readonly IMessageStorage _messageStorage;

		public ChatLogic(string userName)
		{
			_chatStorage = new ChatStorage(userName);
			_messageStorage = new MessageStorage(userName);
		}

		public async Task<ChatViewModel> GetChatAsync(int chatId)
		{
			var chat = await _chatStorage.GetChatAsync(new ChatSearchModel()); // Нужно добавить метод в storage
			//var messages = await _messageStorage.GetMessagesAsync(new MessageSearchModel { ChatId = chatId});
			return MapToViewModel(chat);
		}

		public async Task<ChatViewModel> GetOrCreateChatAsync(string currentUser, string interlocutor)
		{
			var search = new ChatSearchModel
			{
				CurrentUser = currentUser,
				Interlocutor = interlocutor
			};

			var chat = await _chatStorage.GetChatAsync(search);

			if (chat == null)
			{
				chat = await _chatStorage.CreateChatAsync(new ChatBindingModel
				{
					CurrentUser = currentUser,
					Interlocutor = interlocutor
				});
			}

			// Получаем последние сообщения для превью
			var messagesSearch = new MessageSearchModel { ChatId = chat.Id };
			var messages = await _messageStorage.GetMessagesAsync(messagesSearch);

			return MapToViewModel(chat);
		}

		public async Task<List<ChatViewModel>> GetUserChatsAsync(string currentUser)
		{
			var search = new ChatSearchModel { CurrentUser = currentUser };
			var chats = await _chatStorage.GetChatsAsync(search);

			var result = new List<ChatViewModel>();

			foreach (var chat in chats)
			{
				var messagesSearch = new MessageSearchModel { ChatId = chat.Id };
				var messages = await _messageStorage.GetMessagesAsync(messagesSearch);
				result.Add(MapToViewModel(chat));
			}

			return result;
		}

		private ChatViewModel MapToViewModel(ChatBindingModel chat)
		{
			return new ChatViewModel
			{
				Id = chat.Id,
				CurrentUser = chat.CurrentUser,
				Interlocutor = chat.Interlocutor,
			};
		}

		public void Dispose()
		{
			_chatStorage?.Dispose();
			_messageStorage?.Dispose();
		}
	}
}
