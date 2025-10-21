using Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.LogicContracts
{
	public interface IChatLogic
	{
		Task<ChatViewModel> GetOrCreateChatAsync(string currentUser, string interlocutor);
		Task<List<ChatViewModel>> GetUserChatsAsync(string currentUser);
		Task<ChatViewModel> GetChatAsync(int chatId);
	}
}
