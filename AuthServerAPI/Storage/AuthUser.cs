using AuthServerAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthServerAPI.Storage
{
	// AuthServer/Models/AuthUser.cs
	public class AuthUser
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		//public bool IsEmailVerified { get; set; }
		//public DateTime CreatedAt { get; set; }
		//public DateTime LastLoginAt { get; set; }


		public static AuthUser? Create(UserBindingModel model)
		{
			if (model == null)
			{
				return null;
			}
			return new AuthUser()
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
		public UserBindingModel GetBindingModel => new()
		{
			Id = Id,
			Username = Username,
			Email = Email,
		};
	}

}
