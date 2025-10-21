using Models.Binding;
using Models.Search;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.StorageContracts
{
	public interface IUserStorage
	{
		Task<List<UserViewModel>> GetFullList();
		Task<List<UserViewModel>> GetFilteredList(UserSearchModel model);
		Task<UserViewModel>? GetElement(UserSearchModel model);
		Task<UserViewModel>? Insert(UserBindingModel model);
		Task<UserViewModel>? Update(UserBindingModel model);
		Task<UserViewModel>? Delete(UserBindingModel model);
	}
}
