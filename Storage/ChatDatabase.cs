using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Storage.Models;

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
				var databaseName = $"ChatClient_{_userName}";
				var connectionString = @$"Data Source=localhost\SQLEXPRESS;
									Initial Catalog={databaseName};
									Integrated Security=True;
									MultipleActiveResultSets=True;
									TrustServerCertificate=True";

				optionsBuilder.UseSqlServer(connectionString);
			}
			base.OnConfiguring(optionsBuilder);
		}

		// УБЕРИ метод EnsureDatabaseCreated - он не нужен

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Chat> Chats { get; set; }
		public virtual DbSet<Message> Messages { get; set; }
	}
}
