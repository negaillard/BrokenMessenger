using Models.View;
using Models.Search;
using Models.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.LogicContracts
{
	public interface IUserLogic
	{
		Task<List<UserViewModel>?> ReadListAsync(UserSearchModel? model);
		Task<UserViewModel?> ReadElementAsync(UserSearchModel model);
		Task<bool> CreateAsync(UserBindingModel model);
		Task<bool> UpdateAsync(UserBindingModel model);
		Task<bool> DeleteAsync(UserBindingModel model);
	}
}
