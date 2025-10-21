using Models.Binding;
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
	}
}
