using ResourceModule;
using System.Collections.Generic;

namespace DResourceModule
{
	public enum eResult
	{
		Success,
		Error_NotFoundFolder
	}

    public class RefDataLoadManager
    {
		public Dictionary<string, RefDataTable> ref_data_tables = new Dictionary<string, RefDataTable>();
		public List<EnumData> enum_datas = new List<EnumData>();
		public ResourceLogger logger = new ResourceLogger();
		public RefDataLoadManager()
		{

		}
    }
}
