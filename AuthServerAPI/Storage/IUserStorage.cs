using AuthServerAPI.Models;

namespace AuthServerAPI.Storage
{
	public interface IUserStorage
	{
		List<UserBindingModel> GetFullListAsync();
		List<UserBindingModel> GetFilteredListAsync(UserSearchModel model);
		UserBindingModel? GetElementAsync (UserSearchModel model);
		UserBindingModel? InsertElementAsync(UserBindingModel model);
		UserBindingModel? UpdateAsync(UserBindingModel model);
		UserBindingModel? DeleteAsync(UserBindingModel model);
	}
}
