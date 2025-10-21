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
	public interface IChatLogic
	{
		Task<List<ChatViewModel>>? ReadList(ChatSearchModel? model);
		Task<ChatViewModel>? ReadElement(ChatSearchModel model);
		Task<bool> Create(ChatBindingModel model);
		Task<bool> Update(ChatBindingModel model);
		Task<bool> Delete(ChatBindingModel model);
	}
}
