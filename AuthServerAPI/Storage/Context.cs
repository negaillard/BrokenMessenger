using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AuthServerAPI.Storage
{
	public class Context : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=localhost\SQLEXPRESS;
		                                  Initial Catalog=ChatClient_Template;
		                                  Integrated Security=True;
		                                  TrustServerCertificate=True");
		}

		public virtual DbSet<AuthUser> Users { get; set; }
	}
}
