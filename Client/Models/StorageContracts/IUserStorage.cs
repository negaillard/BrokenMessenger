using Client.Models.Binding;
using Client.Models.Search;
using Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.StorageContracts
{
	public interface IUserStorage
	{
		Task<UserBindingModel> GetUserAsync(UserSearchModel search);
		Task<List<UserBindingModel>> GetUsersAsync(UserSearchModel search);
		Task<UserBindingModel> CreateUserAsync(UserBindingModel user);
		Task<UserBindingModel> UpdateUserAsync(UserBindingModel user);
		Task<bool> DeleteUserAsync(UserBindingModel user);
		void Dispose();
	}
}
