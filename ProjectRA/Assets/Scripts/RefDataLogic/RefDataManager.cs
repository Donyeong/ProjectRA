using DResourceModule;
using Newtonsoft.Json.Linq;
using ReferenceTable;
using ResourceModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RefDataManager : CSingleton<RefDataManager>
{
	public string file_name = "data.refs"; // 압축 파일 이름 (Resources 폴더 안의 파일)
	Dictionary<string, List<RefDataItem>> map_ref_datas = new Dictionary<string, List<RefDataItem>>();
	public List<T> GetRefDatas<T>() where T : RefDataItem
	{
		string table_name = typeof(T).Name;
		if(!map_ref_datas.ContainsKey(table_name))
		{
			Debug.LogError($"not found ref data [{table_name}]");
			return null;
		}
		List<RefDataItem> refDataItems = map_ref_datas[table_name];
		return refDataItems.OfType<T>().ToList();
	}

	public void LoadRefData()
	{
		map_ref_datas = new Dictionary<string, List<RefDataItem>>();
		string file_path = Path.Combine(Application.streamingAssetsPath, file_name);
		RefDataLoaderFromFile loader = new RefDataLoaderFromFile();
		loader.logger.logEvent = (t, content) =>
		{
			Debug.Log(content);
		};
		Dictionary<string, byte[]> datas = loader.LoadRefData(file_path);

/*		foreach(var table in loader.ref_data_tble)
		{
			List<RefDataItem> list_ref_datas = ConvertToDataClass(table.Value);
			if(list_ref_datas == null)
			{
				continue;
			}
			map_ref_datas.Add(table.Key, list_ref_datas);
		}*/

		foreach(var data in datas)
		{
			string table_name = Path.GetFileNameWithoutExtension(data.Key);
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteBytes(data.Value);

			string class_name = $"Ref{table_name}";

			string table_name_from_data = byteBuffer.ReadString();

			int column_count = byteBuffer.ReadInteger();
			if (column_count <= 0)
			{
				Debug.LogError($"invalid column count [{class_name}]");
				continue;
			}

			for (int i = 0; i < column_count; i++)
			{
				string data_name;
				string enum_type_name;
				int column_type;
				int data_type;

				data_name = byteBuffer.ReadString();
				enum_type_name = byteBuffer.ReadString();
				column_type = byteBuffer.ReadInteger();
				data_type = byteBuffer.ReadInteger();
			}

			int record_count = byteBuffer.ReadInteger();
			List<RefDataItem> items = RefFactory.CreateRefData(class_name, record_count);

			for (int i = 0; i < record_count; i++)
			{
				RefDataItem item = items[i];
				item.Deserialize(byteBuffer);
			}
			if(map_ref_datas.ContainsKey(class_name))
			{
				Debug.LogError($"duplicated ref data class name [{class_name}]");
				continue;
			}
			map_ref_datas.Add(class_name, items);
			Debug.Log($"loaded ref data : [{class_name}]");
		}

		Debug.Log("complete ref data load");

	}



	public List<RefDataItem> ConvertToDataClass(RefDataTable _source_table)
	{
		List<RefDataItem> res = new List<RefDataItem>();
		string class_name = $"Ref{_source_table.table_name}";
		Type type = Type.GetType(class_name);
		if (type == null)
		{
			Debug.LogError($"invalid ref data class name [{class_name}]");
			return null;
		}
		foreach(var record in _source_table.records)
		{
			object obj = Activator.CreateInstance(type);
			//FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			for(int i = 0; i < record.data_value.Count; i++)
			{
				Column column_info = _source_table.columns[i];

				string data_value = record.data_value[i];
				FieldInfo field = type.GetField(column_info.data_name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				if(field == null)
				{
					Debug.LogError($"invalid ref data field name [{class_name}].[{column_info.data_name}]");
					break;
				}
				Type field_type = field.FieldType;
				object converted_value = ConvertValue(data_value, field_type);

				field.SetValue(obj, converted_value);

			}
			res.Add(obj as RefDataItem);
		}

		return res;
	}

	static object ConvertValue(string data_value, Type targetType)
	{
		try
		{
			if (targetType == typeof(string))
			{
				return data_value; // 문자열은 그대로 반환
			}
			else if (targetType.IsEnum)
			{
				return Enum.Parse(targetType, data_value); // 열거형 변환
			}
			else if (targetType == typeof(bool))
			{
				return bool.Parse(data_value); // 불리언 변환
			}
			else
			{
				// 기본 타입 변환 (int, float, double 등)
				return Convert.ChangeType(data_value, targetType);
			}
		}
		catch
		{
			// 변환 실패 시 null 반환
			return null;
		}
	}
} 
