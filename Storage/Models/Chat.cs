using Interfaces;
using Models.Binding;
using Models.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Models
{
	public class Chat : IChat
	{
		public int Id {  get; private set; }
		[Required]
		public string CurrentUser {  get; private set; } = string.Empty;
		[Required]
		public string Interlocutor { get; private set; } = string.Empty;

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

		public ChatViewModel GetViewModel => new()
		{
			Id = Id,
			CurrentUser = CurrentUser,
			Interlocutor = Interlocutor,
		};

	}
}
