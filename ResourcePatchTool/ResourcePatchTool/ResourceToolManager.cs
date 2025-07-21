using DResourceModule;
using ResourceModuleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcePatchTool
{
	class ResourceToolManager
	{
		public RefDataLoadManager ref_data_manager;
		public ExcelParser parser;

		public ResourceToolManager()
		{
			ref_data_manager = new RefDataLoadManager();
			parser = new ExcelParser(ref_data_manager);
		}
	}
}
