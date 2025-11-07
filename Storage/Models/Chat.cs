using Interfaces;
using Models.Binding;
using Models.View;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Models
{
	public class Chat : IChat
	{
		public int Id { get; private set; }
		[Required]
		public string CurrentUser { get; private set; } = string.Empty;
		[Required]
		public string Interlocutor { get; private set; } = string.Empty;

		[ForeignKey("ChatId")]
		public virtual List<Message> Messages { get; set; } = new();

		public static Chat? Create(ChatBindingModel model)
		{
			if (model == null) return null;

			return new Chat()
			{
				Id = model.Id,
				CurrentUser = model.CurrentUser,
				Interlocutor = model.Interlocutor,
			};
		}
		public void Update(ChatBindingModel model)
		{
			if (model == null)
			{
				return;
			}
		}

		public ChatViewModel GetViewModel => new()
		{
			Id = Id,
			CurrentUser = CurrentUser,
			Interlocutor = Interlocutor,
		};

	}
}
