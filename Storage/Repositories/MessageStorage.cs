using Microsoft.EntityFrameworkCore;
using Models.Binding;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Models;

namespace Storage.Repositories
{
	public class MessageStorage : IMessageStorage
	{
		private readonly ChatDatabase _context;

		public MessageStorage(string username)
		{
			_context = new ChatDatabase(username);
			_context.Database.EnsureCreated();
		}
		public async Task<MessageViewModel?> DeleteAsync(MessageBindingModel model)
		{
			var element = await _context.Messages.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			if (element != null)
			{
				_context.Messages.Remove(element);
				await _context.SaveChangesAsync();
				return element.GetViewModel;
			}
			return null;
		}

		public async Task<MessageViewModel?> GetElementAsync(MessageSearchModel model)
		{
			if (string.IsNullOrEmpty(model.Content) && !model.Id.HasValue && !model.ChatId.HasValue)
			{
				return null;
			}

			var entity = await _context.Messages
				.FirstOrDefaultAsync(x =>
					(!string.IsNullOrEmpty(model.Content) && x.Content == model.Content) ||
					(model.Id.HasValue && x.Id == model.Id) ||
					(model.ChatId.HasValue && x.ChatId == model.ChatId));

			if (entity != null)
			{
				return entity.GetViewModel;
			}
			return null;
		}

		public async Task<List<MessageViewModel>> GetFilteredListAsync(MessageSearchModel model)
		{
			var query = _context.Messages.AsQueryable();

			// Добавляем условия фильтрации, если параметры указаны
			if (model.Id.HasValue)
			{
				query = query.Where(x => x.Id == model.Id.Value);
			}

			if (model.ChatId.HasValue)
			{
				query = query.Where(x => x.ChatId == model.ChatId.Value);
			}

			if (!string.IsNullOrEmpty(model.Sender))
			{
				query = query.Where(x => x.Sender == model.Sender);
			}

			if (!string.IsNullOrEmpty(model.Recipient))
			{
				query = query.Where(x => x.Recipient == model.Recipient);
			}

			if (!string.IsNullOrEmpty(model.Content))
			{
				query = query.Where(x => x.Content.Contains(model.Content));
			}

			return await query
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<List<MessageViewModel>> GetFullListAsync()
		{
			return await _context.Messages
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<MessageViewModel?> InsertAsync(MessageBindingModel model)
		{
			var newMessage = Message.Create(model);
			if (newMessage == null)
			{
				return null;
			}
			await _context.Messages.AddAsync(newMessage);
			await _context.SaveChangesAsync();
			return newMessage.GetViewModel;
		}

		public async Task<MessageViewModel?> UpdateAsync(MessageBindingModel model)
		{
			var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == model.Id);
			if (message == null)
			{
				return null;
			}
			message.Update(model);
			await _context.SaveChangesAsync();
			return message.GetViewModel;
		}
	}
}
