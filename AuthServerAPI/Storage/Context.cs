using System.Collections.Generic;

namespace AuthServerAPI.Storage
{
	public class Context
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=localhost\SQLEXPRESS;
		                                  Initial Catalog=ChatClient_Template;
		                                  Integrated Security=True;
		                                  TrustServerCertificate=True");
		}

		public virtual DbSet<User> Users { get; set; }
	}
}
