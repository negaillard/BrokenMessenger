using Logic;
using Models.LogicContracts;
using Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DesktopClient
{
	public class UserService
	{
		public readonly IChatLogic _chatLogic;
		private readonly APIClient _apiClient;

		public UserService(string username) {

			_chatLogic = new ChatLogic(username);


		}

		public ChatViewModel GetChatsFromServer(string name)
		{

		}

		public ChatViewModel GetChatsFromLocal(string name) {
			
		}
	}
}
