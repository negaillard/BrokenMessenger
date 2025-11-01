using Microsoft.EntityFrameworkCore;
using Models.Binding;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Models;

namespace Storage.Repositories
{
	public class ChatStorage : IChatStorage
	{
		private readonly ChatDatabase _context;

		public ChatStorage(string username)
		{
			_context = new ChatDatabase(username);
			_context.Database.EnsureCreated();
		}

		public async Task<ChatViewModel?> DeleteAsync(ChatBindingModel model)
		{
			var element = await _context.Chats.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			if (element != null)
			{
				_context.Chats.Remove(element);
				await _context.SaveChangesAsync();
				return element.GetViewModel;
			}
			return null;
		}

		public async Task<ChatViewModel?> GetElemаentAsync(ChatSearchModel model)
		{
			if (string.IsNullOrEmpty(model.CurrentUser) && !model.Id.HasValue)
			{
				return null;
			}
			var entity = await _context.Chats
						.FirstOrDefaultAsync(x =>
						(!string.IsNullOrEmpty(model.CurrentUser) && x.CurrentUser == model.CurrentUser) ||
						(model.Id.HasValue && x.Id == model.Id));
			if (entity != null)
			{
				return entity.GetViewModel;
			}
			return null;
		}

		public async Task<ChatViewModel?> GetElementAsync(ChatSearchModel model)
		{
			if ((string.IsNullOrEmpty(model.CurrentUser) || string.IsNullOrEmpty(model.Interlocutor)) && !model.Id.HasValue)
			{
				return null;
			}
			var entity = await _context.Chats
				.FirstOrDefaultAsync(x =>
					(x.CurrentUser == model.CurrentUser &&
					x.Interlocutor == model.Interlocutor) ||
					(model.Id.HasValue && x.Id == model.Id));

			if (entity != null)
			{
				return entity.GetViewModel;
			}
			return null;
		}

		public async Task<List<ChatViewModel>> GetFilteredListAsync(ChatSearchModel model)
		{
			var query = _context.Chats.AsQueryable();

			if (!string.IsNullOrEmpty(model.CurrentUser))
			{
				query = query.Where(x => x.CurrentUser == model.CurrentUser);
			}

			if (!string.IsNullOrEmpty(model.Interlocutor))
			{
				query = query.Where(x => x.Interlocutor == model.Interlocutor);
			}

			return await query
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<List<ChatViewModel>> GetFullListAsync()
		{
			return await _context.Chats
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<ChatViewModel?> InsertAsync(ChatBindingModel model)
		{
			var newChat = Chat.Create(model);
			if (newChat == null)
			{
				return null;
			}
			await _context.Chats.AddAsync(newChat);
			await _context.SaveChangesAsync();
			return newChat.GetViewModel;
		}

		public async Task<ChatViewModel?> UpdateAsync(ChatBindingModel model)
		{
			var chat = await _context.Chats.FirstOrDefaultAsync(x => x.Id == model.Id);
			if (chat == null)
			{
				return null;
			}
			chat.Update(model);
			await _context.SaveChangesAsync();
			return chat.GetViewModel;
		}
	}
}
