using Client.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
	public class User : IUser
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public List<Chat> Chats { get; set; }
		public string CurrentInterlocutor { get; set; }

		public User() {
			Chats = new List<Chat>();	
		}

		public User(string userName) : this() {
			Username = userName;
		}

		public Chat GetOrCreateChat(string interlocutor)
		{
			var chat = Chats.FirstOrDefault(c => c.Interlocutor == interlocutor);
			if (chat == null) {
				chat = new Chat(Username, interlocutor);
				Chats.Add(chat);
			}
			return chat;
		}

		public void SetCurrentInterlocutor(string interlocutor)
		{
			CurrentInterlocutor = interlocutor;
			GetOrCreateChat(interlocutor);
		}

		public void AddSentMessage(string content)
		{
			if (string.IsNullOrEmpty(CurrentInterlocutor))
				throw new InvalidOperationException("Сначала выберите собеседника");
			
			var chat = GetOrCreateChat(CurrentInterlocutor);
			chat.AddMessage(new Message
			{
				Content = content,
				Sender = Username,
				Recipient = CurrentInterlocutor,
				IsSent = true
			});
		}

		public void AddReceivedMessage(string sender, string content)
		{
			var chat = GetOrCreateChat(sender);
			chat.AddReceivedMessage(sender, content);
		}

		public void DisplayChats()
		{
			Console.WriteLine("\n💬 Ваши чаты:");
			Console.WriteLine("══════════════════════════════════");

			if (!Chats.Any())
			{
				Console.WriteLine("   Пока нет чатов...");
				return;
			}

			foreach (var chat in Chats)
			{
				Console.WriteLine($"   👤 {chat.Interlocutor} ");
			}
		}
	}
}
