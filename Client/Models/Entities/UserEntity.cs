using Client.Models.Binding;
using Client.Models.Interfaces;
using Client.Models.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Entities
{
	public class UserEntity : IUser
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Username { get; set; }

		public virtual List<ChatEntity> Chats { get; set; }
		public UserEntity()
		{
			Chats = new List<ChatEntity>();
		}
		public UserBindingModel GetBindingModel => new()
		{
			Id = Id,
			Username = Username,
			//CurrentInterlocutor = CurrentInte, //// нахуя нам каррент интерлокутер здесь и откуда его брать блять
		};
	}
}
