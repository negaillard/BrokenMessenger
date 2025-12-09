using Models.Binding;
using Models.Pagination;
using Models.Search;
using Models.View;

namespace Models.LogicContracts
{
	public interface IMessageLogic
	{
		Task<List<MessageViewModel>?> ReadListAsync(MessageSearchModel? model);
		Task<MessageViewModel?> ReadElementAsync(MessageSearchModel model);
		Task<bool> CreateAsync(MessageBindingModel model);
		Task<bool> UpdateAsync(MessageBindingModel model);
		Task<bool> DeleteAsync(MessageBindingModel model);
		Task<PaginatedResult<MessageViewModel>> SearchMessagesAsync(
		   MessageSearchModel searchModel,
		   int page = 1,
		   int pageSize = 50);
		Task<PaginatedResult<MessageViewModel>> GetMessagesByChatIdAsync(
		   int chatId,
		   int page = 1,
		   int pageSize = 50);
	}
}
