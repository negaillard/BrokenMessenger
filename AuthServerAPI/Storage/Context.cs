using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace AuthServerAPI.Storage
{
	public class Context : DbContext
	{
		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = "Server=db;Database=AuthServer;User Id=api_user;Password=password;TrustServerCertificate=True;";
			optionsBuilder.UseSqlServer(connectionString, options =>
			{
				options.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: TimeSpan.FromSeconds(30),
					errorNumbersToAdd: null
				);
			});
		}
		// проверка что бд создалась
		// docker exec -it authserverapi-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "YourStrong@Passw0rd" -Q "SELECT name FROM sys.databases;"


		public virtual DbSet<AuthUser> Users { get; set; }
	}
}
