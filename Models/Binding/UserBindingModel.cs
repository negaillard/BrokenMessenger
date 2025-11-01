using Interfaces;

namespace Models.Binding
{
	public class UserBindingModel : IUser
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	}
}
