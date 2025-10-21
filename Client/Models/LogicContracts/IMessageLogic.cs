using Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.LogicContracts
{
	public interface IMessageLogic
	{
		Task<MessageViewModel> SendMessageAsync(string sender, string recipient, string content);
		Task<List<MessageViewModel>> ReceiveMessagesAsync(string recipient, string sender = null);
		Task<List<MessageViewModel>> GetChatMessagesAsync(int chatId); 
	}
}
