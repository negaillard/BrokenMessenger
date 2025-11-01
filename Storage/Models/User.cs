using Interfaces;
using Models.Binding;
using Models.View;
using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
	public class User : IUser
	{
		public int Id { get; private set; }
		[Required]
		public string Username { get; private set; } = string.Empty;

		[Required]
		public string Email { get; private set; } = string.Empty;

		public static User? Create(UserBindingModel model)
		{
			if (model == null)
			{
				return null;
			}
			return new User()
			{
				Id = model.Id,
				Username = model.Username,
				Email = model.Email,
			};
		}
		public void Update(UserBindingModel model)
		{
			if (model == null)
			{
				return;
			}
			Username = model.Username;
			Email = model.Email;
		}
		public UserViewModel GetViewModel => new()
		{
			Id = Id,
			Username = Username,
			Email = Email,
		};
	}
}
