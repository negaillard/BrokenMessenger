using Client.Models.Binding;
using Client.Models.Entities;
using Client.Models.Search;
using Client.Models.StorageContracts;
using Client.Models.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Storage
{
	public class MessageStorage : IMessageStorage
	{
		private readonly ChatDbContext _context;

		public MessageStorage(string userName)
		{
			_context = new ChatDbContext(userName);
		}

		public async Task<List<MessageBindingModel>> GetMessagesAsync(MessageSearchModel search)
		{
			var query = _context.Messages.AsQueryable();

			if (search.ChatId.HasValue)
				query = query.Where(m => m.ChatId == search.ChatId.Value);

			if (!string.IsNullOrEmpty(search.Sender))
				query = query.Where(m => m.Sender == search.Sender);

			if (!string.IsNullOrEmpty(search.Recipient))
				query = query.Where(m => m.Recipient == search.Recipient);

			if (search.FromDate.HasValue)
				query = query.Where(m => m.Timestamp >= search.FromDate.Value);

			var entities = await query.OrderBy(m => m.Timestamp).ToListAsync();
			return entities.Select(x => x.GetBindingModel).ToList();
		}

		public async Task<MessageBindingModel> CreateMessageAsync(MessageBindingModel message)
		{
			var entity = new MessageEntity
			{
				Sender = message.Sender,
				Recipient = message.Recipient,
				Content = message.Content,
				Timestamp = DateTime.Now,
				IsSent = message.IsSent,
				ChatId = message.ChatId
			};

			_context.Messages.Add(entity);

			await _context.SaveChangesAsync();

			return entity.GetBindingModel;
		}

		public void Dispose() => _context?.Dispose();

		public Task<MessageBindingModel> GetMessageAsync(MessageSearchModel message)
		{
			throw new NotImplementedException();
		}

		public Task<MessageBindingModel> UpdateMessageAsync(MessageBindingModel message)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteMessageAsync(MessageBindingModel message)
		{
			throw new NotImplementedException();
		}
	}
}
