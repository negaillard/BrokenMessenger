using Models.Binding;
using Models.Pagination;
using Models.Search;
using Models.View;

namespace Models.StorageContracts
{
	public interface IMessageStorage
	{
		Task<List<MessageViewModel>> GetFullListAsync();
		Task<List<MessageViewModel>> GetFilteredListAsync(MessageSearchModel model);
		Task<MessageViewModel?> GetElementAsync(MessageSearchModel model);
		Task<MessageViewModel?> InsertAsync(MessageBindingModel model);
		Task<MessageViewModel?> UpdateAsync(MessageBindingModel model);
		Task<MessageViewModel?> DeleteAsync(MessageBindingModel model);
		Task<PaginatedResult<MessageViewModel>> SearchMessagesAsync(
			MessageSearchModel searchModel,
			int page,
			int pageSize);
		Task<PaginatedResult<MessageViewModel>> GetMessagesByChatIdAsync(
		int chatId,
		int page,
		int pageSize);
	}
}
