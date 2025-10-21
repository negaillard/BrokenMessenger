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
	public class UserStorage : IUserStorage
	{
		private readonly ChatDbContext _context;

		public UserStorage(string userName)
		{
			_context = new ChatDbContext(userName);
		}

		public async Task<UserBindingModel> GetUserAsync(UserSearchModel search)
		{
			var query = _context.Users.AsQueryable();

			if (!string.IsNullOrEmpty(search.Username))
				query = query.Where(u => u.Username == search.Username);

			var entity = await query.FirstOrDefaultAsync();
			return entity.GetBindingModel;
		}

		public async Task<List<UserBindingModel>> GetUsersAsync(UserSearchModel search)
		{
			var query = _context.Users.AsQueryable();

			if (!string.IsNullOrEmpty(search.Username))
				query = query.Where(u => u.Username.Contains(search.Username));

			var entities = await query.ToListAsync();
			return entities.Select(x => x.GetBindingModel).ToList();
		}

		public async Task<UserBindingModel> CreateUserAsync(UserBindingModel user)
		{
			var entity = new UserEntity
			{
				Username = user.Username,
			};

			_context.Users.Add(entity);
			await _context.SaveChangesAsync();

			return entity.GetBindingModel;
		}

		public Task<UserBindingModel> UpdateUserAsync(UserBindingModel user)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteUserAsync(UserBindingModel user)
		{
			throw new NotImplementedException();
		}

		public void Dispose() => _context?.Dispose();
	}
}
