using Client.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
	public class Chat : IChat
	{
		public int Id { get; set; }
		public string CurrentUser { get; set; }
		public string Interlocutor {  get; set; }
		public List<Message> Messages { get; set; }

		public Chat()
		{
			Messages = new List<Message>();
		}
		public Chat(string currentUser, string interlocutor) : this()
		{
			CurrentUser = currentUser;
			Interlocutor = interlocutor;
		}

		public void AddMessage(Message message) { 
			Messages.Add(message);
			Messages = Messages.OrderBy(m => m.Timestamp).ToList();
		}

		public void AddReceivedMessage(string sender, string content) {
			var message = new Message(sender, CurrentUser, content, isSent: false);
			Messages.Add(message);
		}

		public void DisplayChatHistory()
		{
			Console.WriteLine($"\n📱 Чат с {Interlocutor}");
			Console.WriteLine("══════════════════════════════════");

			if (!Messages.Any())
			{
				Console.WriteLine("   Пока нет сообщений...");
				return;
			}

			foreach (var message in Messages)
			{
				Console.WriteLine($"   {message}");
			}

			Console.WriteLine("══════════════════════════════════");
		}
	}
}
