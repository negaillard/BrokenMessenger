using Models.Binding;
using Models.Search;
using Models.View;

namespace Models.StorageContracts
{
	public interface IUserStorage
	{
		Task<List<UserViewModel>> GetFullListAsync();
		Task<List<UserViewModel>> GetFilteredListAsync(UserSearchModel model);
		Task<UserViewModel?> GetElementAsync(UserSearchModel model);
		Task<UserViewModel?> InsertAsync(UserBindingModel model);
		Task<UserViewModel?> UpdateAsync(UserBindingModel model);
		Task<UserViewModel?> DeleteAsync(UserBindingModel model);
	}
}
