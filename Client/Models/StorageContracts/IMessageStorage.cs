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
	public interface IMessageStorage
	{
		Task<MessageBindingModel> GetMessageAsync(MessageSearchModel search);
		Task<List<MessageBindingModel>> GetMessagesAsync(MessageSearchModel search);
		Task<MessageBindingModel> CreateMessageAsync(MessageBindingModel message);
		Task<MessageBindingModel> UpdateMessageAsync(MessageBindingModel message);
		void Dispose();
	}
}
