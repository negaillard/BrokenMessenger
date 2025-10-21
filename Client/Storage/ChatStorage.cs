using Client.Models.Binding;
using Client.Models.Entities;
using Client.Models.Search;
using Client.Models.StorageContracts;
using Client.Models.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Storage
{
	public class ChatStorage : IChatStorage
	{
		private readonly ChatDbContext _context;

		public ChatStorage(string userName)
		{
			_context = new ChatDbContext(userName);
		}

		public async Task<ChatBindingModel> GetChatAsync(ChatSearchModel search)
		{
			var query = _context.Chats.AsQueryable();

			if (!string.IsNullOrEmpty(search.CurrentUser))
				query = query.Where(c => c.CurrentUser == search.CurrentUser);

			if (!string.IsNullOrEmpty(search.Interlocutor))
				query = query.Where(c => c.Interlocutor == search.Interlocutor);

			var entity = await query.FirstOrDefaultAsync();
			return entity.GetBindingModel;
		}

		public async Task<List<ChatBindingModel>> GetChatsAsync(ChatSearchModel search)
		{
			var query = _context.Chats.Include(c => c.Messages).AsQueryable();

			if (!string.IsNullOrEmpty(search.CurrentUser))
				query = query.Where(c => c.CurrentUser == search.CurrentUser);

			if (search.FromDate.HasValue)
				query = query.Where(c => c.LastActivity >= search.FromDate.Value);

			var entities = await query.OrderByDescending(c => c.LastActivity).ToListAsync();
			return entities.Select(x => x.GetBindingModel).ToList();
		}

		public async Task<ChatBindingModel> CreateChatAsync(ChatBindingModel chat)
		{
			var entity = new ChatEntity
			{
				CurrentUser = chat.CurrentUser,
				Interlocutor = chat.Interlocutor,
				CreatedAt = DateTime.Now,
				LastActivity = DateTime.Now
			};

			_context.Chats.Add(entity);
			await _context.SaveChangesAsync();

			return entity.GetBindingModel;
		}

		public void Dispose() => _context?.Dispose();

		public Task<ChatBindingModel> UpdateChatAsync(ChatBindingModel chat)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteChatAsync(ChatBindingModel chat)
		{
			throw new NotImplementedException();
		}
	}
}
