using Client.Models.Binding;
using Client.Models.Search;
using Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.StorageContracts
{
	public interface IChatStorage
	{
		Task<ChatBindingModel> GetChatAsync(ChatSearchModel search);
		Task<List<ChatBindingModel>> GetChatsAsync(ChatSearchModel search);
		Task<ChatBindingModel> CreateChatAsync(ChatBindingModel chat);
		Task<ChatBindingModel> UpdateChatAsync(ChatBindingModel chat);
		Task<bool> DeleteChatAsync(ChatBindingModel search);
		void Dispose();
	}
}
