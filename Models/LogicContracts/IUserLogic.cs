using Models.Binding;
using Models.Search;
using Models.View;

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
