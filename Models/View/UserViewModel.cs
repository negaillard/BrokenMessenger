using Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.View
{
	public class UserViewModel : IUser
	{
		public int Id { get; set; }
		[DisplayName("имя пользователя")]
		public string Username { get; set; } = string.Empty;
	}
}
