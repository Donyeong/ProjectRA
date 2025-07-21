using ResourceModule;
using System;
using System.Collections.Generic;

namespace DResourceModule
{
	public enum eLogType {
		Info,
		Warning,
		Error
	}

	public class RefDataLoaderBase
	{

		public Dictionary<string, RefDataTable> ref_data_tble = new Dictionary<string, RefDataTable>();
		public ResourceLogger logger = new ResourceLogger();
		public void ClearTables()
		{
			ref_data_tble.Clear();
		}
	}
}
