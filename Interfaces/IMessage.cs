using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
	public interface IMessage
	{
		int Id { get;  }
		string Sender { get;  }
		string Recipient { get;  }
		string Content { get;  }
		DateTime Timestamp { get; }
		bool IsSent { get; }
		int ChatId { get; }
	}
}
