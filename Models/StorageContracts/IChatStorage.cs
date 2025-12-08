using Models.Binding;
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

	}
}
