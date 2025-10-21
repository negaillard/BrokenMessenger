using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
	public interface IChat
	{
		int Id { get; set; }
		string CurrentUser { get; set; }
		string Interlocutor { get; set; }
	}
}
