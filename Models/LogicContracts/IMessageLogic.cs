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
		Task<List<MessageViewModel>>? ReadList(MessageSearchModel? model);
		Task<MessageViewModel>? ReadElement(MessageSearchModel model);
		Task<bool> Create(MessageBindingModel model);
		Task<bool> Update(MessageBindingModel model);
		Task<bool> Delete(MessageBindingModel model);
	}
}
