using Models.Binding;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Repositories
{
	public class UserStorage : IUserStorage
	{
		public Task<UserViewModel?> DeleteAsync(UserBindingModel model)
		{
			throw new NotImplementedException();
		}

		public Task<UserViewModel?> GetElementAsync(UserSearchModel model)
		{
			throw new NotImplementedException();
		}

		public Task<List<UserViewModel>> GetFilteredListAsync(UserSearchModel model)
		{
			throw new NotImplementedException();
		}

		public Task<List<UserViewModel>> GetFullListAsync()
		{
			throw new NotImplementedException();
		}

		public Task<UserViewModel?> InsertAsync(UserBindingModel model)
		{
			throw new NotImplementedException();
		}

		public Task<UserViewModel?> UpdateAsync(UserBindingModel model)
		{
			throw new NotImplementedException();
		}
	}
}
