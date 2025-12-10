using Microsoft.EntityFrameworkCore;
using Models.Binding;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using Storage.Models;

namespace Storage.Repositories
{
	public class UserStorage : IUserStorage
	{
		private readonly string _username;
		public UserStorage(string username)
		{
			_username = username;
		}
		public async Task<UserViewModel?> DeleteAsync(UserBindingModel model)
		{
			using var _context = new ChatDatabase(_username);

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
			using var _context = new ChatDatabase(_username);
			var query = _context.Users.AsQueryable();


			if (!string.IsNullOrEmpty(model.Username))
			{
				query = query.Where(x => x.Username == model.Username);
			}

			if (!string.IsNullOrEmpty(model.Email))
			{
				query = query.Where(x => x.Email == model.Email);
			}

			var entity = await query.FirstOrDefaultAsync();

			if (entity != null)
			{
				return new UserViewModel
				{
					Id = entity.Id,
					Username = entity.Username,
					Email = entity.Email,
				};
			}

			return null;
		}

		public async Task<List<UserViewModel>> GetFilteredListAsync(UserSearchModel model)
		{
			using var _context = new ChatDatabase(_username);
			var query = _context.Users.AsQueryable();

			if (!string.IsNullOrEmpty(model.Username))
			{
				query = query.Where(x => x.Username == model.Username);
			}

			if (!string.IsNullOrEmpty(model.Email))
			{
				query = query.Where(x => x.Email == model.Email);
			}

			return await query
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<List<UserViewModel>> GetFullListAsync()
		{
			using var _context = new ChatDatabase(_username);
			return await _context.Users
				.Select(x => x.GetViewModel)
				.ToListAsync();
		}

		public async Task<UserViewModel?> InsertAsync(UserBindingModel model)
		{
			using var _context = new ChatDatabase(_username);
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
			using var _context = new ChatDatabase(_username);
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
