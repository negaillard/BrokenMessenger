using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Interfaces
{
	public interface IUser
	{
		int Id { get; set; }
		string Username { get; set; }
	}
}
