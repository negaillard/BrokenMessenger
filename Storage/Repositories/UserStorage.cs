using Microsoft.EntityFrameworkCore;
using Models.Binding;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Repositories
{
	public class UserStorage : IUserStorage
	{
		private readonly ChatDatabase _context;

		public UserStorage(string username)
		{
			_context = new ChatDatabase(username);
		}
		public async Task<UserViewModel?> DeleteAsync(UserBindingModel model)
		{
			var element = await _context.Users.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			if (element != null)
			{
				_context.Users.Remove(element);
				await _context.SaveChangesAsync();
				return element.GetViewModel;
			}
			return null;
		}

		public async Task<UserViewModel?> GetElementAsync(UserSearchModel model)
		{
			if (string.IsNullOrEmpty(model.Username) && !model.Id.HasValue)
			{
				return null;
			}
			var entity = await _context.Users
						.FirstOrDefaultAsync(x =>
						(!string.IsNullOrEmpty(model.Username) && x.Username == model.Username) ||
						(model.Id.HasValue && x.Id == model.Id));
			if (entity != null)
			{
				return entity.GetViewModel;
			}
			return null;
		}

		public async Task<List<UserViewModel>> GetFilteredListAsync(UserSearchModel model)
		{
			return await _context.Users
				.Where(x => !string.IsNullOrEmpty(model.Username) && x.Username == model.Username)
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<List<UserViewModel>> GetFullListAsync()
		{
			return await _context.Users
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<UserViewModel?> InsertAsync(UserBindingModel model)
		{
			var newUser = User.Create(model);
			if (newUser == null)
			{
				return null;
			}
			await _context.Users.AddAsync(newUser);
			await _context.SaveChangesAsync();
			return newUser.GetViewModel;
		}

		public async Task<UserViewModel?> UpdateAsync(UserBindingModel model)
		{
			var chat = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
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
