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
		Task<List<MessageViewModel>> GetFullListAsync();
		Task<List<MessageViewModel>> GetFilteredListAsync(MessageSearchModel model);
		Task<MessageViewModel?> GetElementAsync(MessageSearchModel model);
		Task<MessageViewModel?> InsertAsync(MessageBindingModel model);
		Task<MessageViewModel?> UpdateAsync(MessageBindingModel model);
		Task<MessageViewModel?> DeleteAsync(MessageBindingModel model);
	}
}
