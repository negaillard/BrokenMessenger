using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Binding
{
	public class UserBindingModel : IUser
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
