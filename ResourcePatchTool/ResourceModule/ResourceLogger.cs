using DResourceModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceModule
{
	public class ResourceLogger
	{
		public void WriteLogLine(eLogType log_type, string log)
		{
			WriteLog(log_type, $"{log}{Environment.NewLine}");
		}

		public void WriteLog(eLogType log_type, string log)
		{
			if (logEvent != null)
			{
				logEvent(log_type, log);
			}
		}

		public Action<eLogType, string> logEvent;
	}
}
