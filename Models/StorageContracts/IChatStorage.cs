using Models.Binding;
using Models.Pagination;
using Models.Search;
using Models.View;

namespace Models.StorageContracts
{
	public interface IChatStorage
	{
		Task<List<ChatViewModel>> GetFullListAsync();
		Task<List<ChatViewModel>> GetFilteredListAsync(ChatSearchModel model);
		Task<ChatViewModel?> GetElementAsync(ChatSearchModel model);
		Task<ChatViewModel?> InsertAsync(ChatBindingModel model);
		Task<ChatViewModel?> UpdateAsync(ChatBindingModel model);
		Task<ChatViewModel?> DeleteAsync(ChatBindingModel model);
		Task<PaginatedResult<ChatViewModel>> GetRecentChatsAsync(int page, int pageSize);
		Task<PaginatedResult<ChatViewModel>> SearchChatsAsync(
			ChatSearchModel searchModel,
			int page,
			int pageSize);

	}
}
