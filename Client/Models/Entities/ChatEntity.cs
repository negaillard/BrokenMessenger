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
	public class ChatEntity : IChat
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string CurrentUser { get; set; }

		[Required]
		[MaxLength(100)]
		public string Interlocutor { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime LastActivity { get; set; }

		public virtual List<MessageEntity> Messages { get; set; }

		public ChatEntity()
		{
			Messages = new List<MessageEntity>();
			CreatedAt = DateTime.Now;
			LastActivity = DateTime.Now;
		}

		public ChatViewModel GetViewModel => new()
		{
			Id = Id,
			CurrentUser = CurrentUser,
			Interlocutor = Interlocutor,
		};

		public ChatBindingModel GetBindingModel => new()
		{
			Id = Id,
			CurrentUser = CurrentUser,
			Interlocutor = Interlocutor,
		};
	}
}
