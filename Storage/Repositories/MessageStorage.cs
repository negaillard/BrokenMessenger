using Interfaces;
using Models.Binding;
using Models.Search;
using Models.StorageContracts;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Repositories
{
	public class MessageStorage : IChatStorage
	{
		public Task<ChatViewModel?> DeleteAsync(ChatBindingModel model)
		{
			throw new NotImplementedException();
		}

		public Task<ChatViewModel?> GetElementAsync(ChatSearchModel model)
		{
			throw new NotImplementedException();
		}

		public Task<List<ChatViewModel>> GetFilteredListAsync(ChatSearchModel model)
		{
			throw new NotImplementedException();
		}

		public Task<List<ChatViewModel>> GetFullListAsync()
		{
			throw new NotImplementedException();
		}

		public Task<ChatViewModel?> InsertAsync(ChatBindingModel model)
		{
			throw new NotImplementedException();
		}

		public Task<ChatViewModel?> UpdateAsync(ChatBindingModel model)
		{
			throw new NotImplementedException();
		}
	}
}
