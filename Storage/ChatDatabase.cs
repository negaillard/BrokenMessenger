using Microsoft.EntityFrameworkCore;
using Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
	public class ChatDatabase : DbContext
	{
		private readonly string _userName;

		public ChatDatabase(string userName)
		{
			_userName = userName;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				// Уникальное имя БД для каждого пользователя
				var databaseName = $"ChatClient_{_userName}";

				optionsBuilder.UseSqlServer(@$"Data Source=localhost\SQLEXPRESS;
												Initial Catalog={databaseName};
												Integrated Security=True;
												MultipleActiveResultSets=True;
												TrustServerCertificate=True");
			}
			base.OnConfiguring(optionsBuilder);
		}

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	// Настройка отношений
		//	modelBuilder.Entity<ChatEntity>()
		//		.HasMany(c => c.Messages)
		//		.WithOne(m => m.Chat)
		//		.HasForeignKey(m => m.ChatId)
		//		.OnDelete(DeleteBehavior.Cascade);

		//	modelBuilder.Entity<UserEntity>()
		//		.HasMany(u => u.Chats)
		//		.WithOne()
		//		.HasForeignKey(c => c.CurrentUser)
		//		.HasPrincipalKey(u => u.Username)
		//		.OnDelete(DeleteBehavior.Cascade);
		//}

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Chat> Chats { get; set; }
		public virtual DbSet<Message> Messages { get; set; }
	}
}
