using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace ReferenceTable
{
    
    
    public partial class RefFactory
    {
        
        public static List<RefDataItem> CreateRefDataList<T>(int count)
            where T : RefDataItem, new()
        {

		var list = new List<RefDataItem>(count);
		for (int i = 0; i < count; i++)
		{
			var ref_data_item = new T();
			list.Add(ref_data_item);
		}
		return list;

        }
        
        public static List<RefDataItem> CreateRefData(string table_name, int count)
        {

		if (table_name == "RefMap")
		{
			return CreateRefDataList<RefMap>(count);
		}


		if (table_name == "RefMonster")
		{
			return CreateRefDataList<RefMonster>(count);
		}


		if (table_name == "RefProp")
		{
			return CreateRefDataList<RefProp>(count);
		}


		if (table_name == "RefPropSpawner")
		{
			return CreateRefDataList<RefPropSpawner>(count);
		}


		if (table_name == "RefRoom")
		{
			return CreateRefDataList<RefRoom>(count);
		}


		if (table_name == "RefString")
		{
			return CreateRefDataList<RefString>(count);
		}


Debug.LogError($"invalid ref data table name{table_name}");
return null;


        }
    }
}
