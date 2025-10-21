using Client.Models.Binding;
using Client.Models.Interfaces;
using Client.Models.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Entities
{
	public class MessageEntity : IMessage
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Sender { get; set; }

		[Required]
		[MaxLength(100)]
		public string Recipient { get; set; }

		[Required]
		public string Content { get; set; }

		public DateTime Timestamp { get; set; }

		public bool IsSent { get; set; }

		[ForeignKey("Chat")]
		public int ChatId { get; set; }

		public virtual ChatEntity Chat { get; set; }

		public MessageEntity()
		{
			Timestamp = DateTime.Now;
		}

		public MessageBindingModel GetBindingModel => new()
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
