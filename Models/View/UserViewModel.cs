using Interfaces;
using System.ComponentModel;

namespace Models.View
{
	public class UserViewModel : IUser
	{
		public int Id { get; set; }
		[DisplayName("имя пользователя")]
		public string Username { get; set; } = string.Empty;
		[DisplayName("Email")]
		public string Email { get; set; } = string.Empty;
	}
}
