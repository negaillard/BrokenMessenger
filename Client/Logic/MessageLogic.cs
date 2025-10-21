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
	public class MessageLogic : IMessageLogic
	{
		private readonly IMessageStorage _messageStorage;
		private readonly IChatStorage _chatStorage;

		public MessageLogic(string userName)
		{
			_messageStorage = new MessageStorage(userName);
			_chatStorage = new ChatStorage(userName);
		}

		public async Task<List<MessageViewModel>> GetChatMessagesAsync(int chatId)
		{
			var messages = await _messageStorage.GetMessagesAsync(new MessageSearchModel { ChatId = chatId});
			return messages.Select(MapToViewModel).ToList();
		}

		public async Task<MessageViewModel> SendMessageAsync(string sender, string recipient, string content)
		{
			// Получаем или создаем чат
			var chatSearch = new ChatSearchModel
			{
				CurrentUser = sender,
				Interlocutor = recipient
			};

			var chat = await _chatStorage.GetChatAsync(chatSearch);
			if (chat == null)
			{
				chat = await _chatStorage.CreateChatAsync(new ChatBindingModel
				{
					CurrentUser = sender,
					Interlocutor = recipient
				});
			}

			// Создаем сообщение
			var message = await _messageStorage.CreateMessageAsync(new MessageBindingModel
			{
				Sender = sender,
				Recipient = recipient,
				Content = content,
				IsSent = true,
				ChatId = chat.Id
			});

			return MapToViewModel(message);
		}

		public async Task<List<MessageViewModel>> ReceiveMessagesAsync(string recipient, string sender = null)
		{
			var search = new MessageSearchModel { Recipient = recipient };

			if (!string.IsNullOrEmpty(sender))
				search.Sender = sender;

			var messages = await _messageStorage.GetMessagesAsync(search);
			return messages.Select(MapToViewModel).ToList();
		}

		private MessageViewModel MapToViewModel(MessageBindingModel message)
		{
			return new MessageViewModel
			{
				Id = message.Id,
				Sender = message.Sender,
				Recipient = message.Recipient,
				Content = message.Content,
				Timestamp = message.Timestamp,
				IsSent = message.IsSent,
				ChatId = message.ChatId
			};
		}

		public void Dispose()
		{
			_messageStorage?.Dispose();
			_chatStorage?.Dispose();
		}
	}
}
