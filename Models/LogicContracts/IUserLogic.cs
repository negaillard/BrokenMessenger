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
		Task<List<UserViewModel>>? ReadList(UserSearchModel? model);
		Task<UserViewModel>? ReadElement(UserSearchModel model);
		Task<bool> Create(UserBindingModel model);
		Task<bool> Update(UserBindingModel model);
		Task<bool> Delete(UserBindingModel model);
	}
}
