using Models.Binding;
using Models.Search;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.StorageContracts
{
	public interface IChatStorage
	{
		Task<List<ChatViewModel>> GetFullList();
		Task<List<ChatViewModel>> GetFilteredList(ChatSearchModel model);
		Task<ChatViewModel>? GetElement(ChatSearchModel model);
		Task<ChatViewModel>? Insert(ChatBindingModel model);
		Task<ChatViewModel>? Update(ChatBindingModel model);
		Task<ChatViewModel>? Delete(ChatBindingModel model);
	}
}
