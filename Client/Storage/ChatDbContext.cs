using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Client.Storage
{
	public class ChatDbContext : DbContext
	{
		private readonly string _userName;

		public ChatDbContext(string userName)
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
												TrustServerCertificate=True")
												.LogTo(Console.WriteLine, LogLevel.Information)
												.EnableSensitiveDataLogging();
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

		public virtual DbSet<UserEntity> Users { get; set; }
		public virtual DbSet<ChatEntity> Chats { get; set; }
		public virtual DbSet<MessageEntity> Messages { get; set; }
	}
}
