using AuthServerAPI.Models;

namespace AuthServerAPI.Storage
{
	public interface IUserStorage
	{
		Task<List<UserBindingModel>> GetFullListAsync();
		Task<List<UserBindingModel>> GetFilteredListAsync(UserSearchModel model);
		Task<UserBindingModel?> GetElementAsync (UserSearchModel model);
		Task<UserBindingModel?> InsertElementAsync(UserBindingModel model);
		Task<UserBindingModel?> UpdateAsync(UserBindingModel model);
		Task<UserBindingModel?> DeleteAsync(UserBindingModel model);
		Task<PaginatedResult<UserBindingModel>> SearchUsersAsync(UserSearchModel searchModel, int page, int pageSize);
	}
}
