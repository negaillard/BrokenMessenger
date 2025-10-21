using Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.LogicContracts
{
	public interface IUserLogic
	{
		Task<UserViewModel> GetOrCreateUserAsync(string username);
		Task SetCurrentInterlocutorAsync(string currentUser, string interlocutor);
		Task<UserViewModel> GetCurrentUserAsync(string username);
	}
}
