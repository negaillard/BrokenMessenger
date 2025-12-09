using Models.Binding;
using Models.Pagination;
using Models.Search;
using Models.View;

namespace Models.LogicContracts
{
	public interface IChatLogic
	{
		Task<List<ChatViewModel>?> ReadListAsync(ChatSearchModel? model);
		Task<ChatViewModel?> ReadElementAsync(ChatSearchModel model);
		Task<bool> CreateAsync(ChatBindingModel model);
		Task<bool> UpdateAsync(ChatBindingModel model);
		Task<bool> DeleteAsync(ChatBindingModel model);
		Task<PaginatedResult<ChatViewModel>> GetRecentChatsAsync(int page = 1, int pageSize = 30);
		Task<PaginatedResult<ChatViewModel>> SearchChatsAsync(
			string interlocutorName,
			int page,
			int pageSize);
	}
}
