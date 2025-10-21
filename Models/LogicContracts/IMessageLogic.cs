using Models.View;
using Models.Search;
using Models.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
