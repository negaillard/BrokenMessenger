using Interfaces;
using Models.Binding;
using Models.View;
using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
	public class Message : IMessage
	{
		public int Id { get; private set; }
		[Required]
		public string Sender { get; private set; } = string.Empty;
		[Required]
		public string Recipient { get; private set; } = string.Empty;
		[Required]
		public string Content { get; private set; } = string.Empty;
		[Required]
		public DateTime Timestamp { get; private set; }
		[Required]
		public bool IsSent { get; private set; }
		[Required]
		public int ChatId { get; private set; }
		public virtual Chat Chat { get; private set; }

		public static Message? Create(MessageBindingModel model)
		{
			if (model == null)
			{
				return null;
			}
			return new Message()
			{
				Id = model.Id,
				Sender = model.Sender,
				Recipient = model.Recipient,
				Content = model.Content,
				Timestamp = model.Timestamp,
				IsSent = model.IsSent,
				ChatId = model.ChatId,
			};
		}
		public void Update(MessageBindingModel model)
		{
			if (model == null)
			{
				return;
			}
			Content = model.Content;
		}
		public MessageViewModel GetViewModel => new()
		{
			Id = Id,
			Sender = Sender,
			Recipient = Recipient,
			Content = Content,
			Timestamp = Timestamp,
			IsSent = IsSent,
			ChatId = ChatId,
		};
	}
}
