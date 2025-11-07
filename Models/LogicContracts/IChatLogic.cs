using Models.Binding;
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
	}
}
