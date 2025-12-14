using AuthServerAPI.Models;

namespace AuthServerAPI.Logic.Interfaces
{
	public interface IUserLogic
	{
		Task<List<UserBindingModel>?> ReadListAsync(UserSearchModel? model);
		Task<UserBindingModel?> ReadElementAsync(UserSearchModel model);
		Task<bool> CreateAsync(UserBindingModel model);
		Task<bool> UpdateAsync(UserBindingModel model);
		Task<bool> DeleteAsync(UserBindingModel model);
		Task<PaginatedResult<UserBindingModel>> SearchUsersAsync(
		   string username,
		   int page = 1,
		   int pageSize = 30);
	}
}
