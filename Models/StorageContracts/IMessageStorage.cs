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
	public interface IMessageStorage
	{
		Task<List<MessageViewModel>> GetFullList();
		Task<List<MessageViewModel>> GetFilteredList(MessageSearchModel model);
		Task<MessageViewModel>? GetElement(MessageSearchModel model);
		Task<MessageViewModel>? Insert(MessageBindingModel model);
		Task<MessageViewModel>? Update(MessageBindingModel model);
		Task<MessageViewModel>? Delete(MessageBindingModel model);
	}
}
