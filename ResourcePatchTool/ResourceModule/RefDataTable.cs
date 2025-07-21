using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ResourceModule.Column;

namespace ResourceModule
{
	public class RefDataTable
	{
		public string		table_name;
		public int			key_column_index;
		public List<Column>	columns = new List<Column>();
		public List<Record> records = new List<Record>();
		public JObject ToJson()
		{
			JObject jsonObject = new JObject();
			jsonObject.Add("table_name", table_name);
			JArray ja_columns = new JArray();
			columns.ForEach(c =>
			{
				ja_columns.Add(c.ToJson());
			});
			jsonObject.Add("columns", ja_columns);


			JArray ja_records = new JArray();
			records.ForEach(r =>
			{
				ja_records.Add(r.ToJson());
			});
			jsonObject.Add("records", ja_records);

			return jsonObject;
		}
        public void Serialize(ByteBuffer byteBuffer, List<EnumData> enums)
        {
            byteBuffer.WriteString(table_name);

			byteBuffer.WriteInteger(columns.Count);
            columns.ForEach(c =>
            {
				c.Serialize(byteBuffer);
            });

            byteBuffer.WriteInteger(records.Count);
            records.ForEach(r =>
            {
                r.Serialize(byteBuffer, columns, enums);
            });
        }

		public void Deserialize(ByteBuffer byteBuffer)
		{
			table_name = byteBuffer.ReadString();
            int column_count = byteBuffer.ReadInteger();
            columns.Clear();
            for (int i = 0; i < column_count; i++)
            {
                Column column = new Column(byteBuffer);
                columns.Add(column);
            }
            int record_count = byteBuffer.ReadInteger();
            records.Clear();
            for (int i = 0; i < record_count; i++)
            {
                Record record = new Record(byteBuffer, columns[i]);
                records.Add(record);
            }
        }
    }

	public class Column
    {
		public Column(ByteBuffer _buffer)
		{
            data_name = _buffer.ReadString();
            enum_type_name = _buffer.ReadString();
            column_type = (eColumnType)_buffer.ReadInteger();
            data_type = (eDataType)_buffer.ReadInteger();
        }
        public Column(string _data_name, string _data_type, string _column_type)
		{
			SetDataName(_data_name);
			SetDataType(_data_type);
			SetColumnType(_column_type);
        }

		public Column(JObject _json)
		{
			SetDataName(_json.GetValue("data_name").ToObject<string>());
			string data_type_stirng = _json.GetValue("data_type").ToObject<string>();
			Enum.TryParse<eDataType>(data_type_stirng, out data_type);
			string column_type_stirng = _json.GetValue("column_type").ToObject<string>();
			Enum.TryParse<eColumnType>(column_type_stirng, out column_type);
		}

		private void SetDataName(string _data_name)
		{
			data_name = _data_name;
		}
		private void SetEnumTypeName(string _enum_type_name)
        {
            enum_type_name = _enum_type_name;
        }
        private void SetColumnType(string _column_type)
		{
			if (_column_type.Equals("Key", StringComparison.OrdinalIgnoreCase))
			{
				column_type = eColumnType.Key;
			}
			else if (_column_type.Equals("MultiKey", StringComparison.OrdinalIgnoreCase))
			{
				column_type = eColumnType.MultiKey;
			}
			else if (_column_type.Equals("Data", StringComparison.OrdinalIgnoreCase))
			{
				column_type = eColumnType.Data;
			}
			else
			{
				throw new Exception($"invalid column_type {_column_type}");
			}
		}
		private void SetDataType(string _data_type)
		{
			if (_data_type.Equals("int64", StringComparison.OrdinalIgnoreCase))
			{
				data_type = eDataType.INT64;
			}
			else if (_data_type.Equals("int32", StringComparison.OrdinalIgnoreCase) || _data_type.Equals("int", StringComparison.OrdinalIgnoreCase))
			{
				data_type = eDataType.INT32;
			}
			else if (_data_type.Equals("int16", StringComparison.OrdinalIgnoreCase))
			{
				data_type = eDataType.INT16;
			}
			else if (_data_type.Equals("string", StringComparison.OrdinalIgnoreCase))
			{
				data_type = eDataType.STRING;
            }
            else if (_data_type.Equals("wstring", StringComparison.OrdinalIgnoreCase))
            {
                data_type = eDataType.WSTRING;
            }
            else if (_data_type.StartsWith("enum_", StringComparison.OrdinalIgnoreCase))
			{
				string parsedValue = _data_type.Substring("enum_".Length);
				data_type = eDataType.ENUM;
				enum_type_name = parsedValue;
            }
            else if (_data_type.Equals("float", StringComparison.OrdinalIgnoreCase))
            {
                data_type = eDataType.FLOAT;
            }
            else if (_data_type.Equals("double", StringComparison.OrdinalIgnoreCase))
            {
                data_type = eDataType.DOUBLE;
            }
            else if (_data_type.Equals("boolean", StringComparison.OrdinalIgnoreCase))
            {
                data_type = eDataType.BOOLEAN;
            }
            else if (_data_type.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                data_type = eDataType.DATE;
            }
            else
			{
				throw new Exception($"invalid data_type {_data_type}");
			}
		}

		public enum eColumnType
		{
			Key,
			MultiKey,
			Data
		}
		public enum eDataType
		{
			INT16,
			INT32,
			INT64,
			STRING,
            WSTRING,
            FLOAT,
			DOUBLE,
			BOOLEAN,
            DATE,
            ENUM,
		}
		public eColumnType	column_type;
		public eDataType	data_type;
		public string		data_name;

		public string		enum_type_name = String.Empty;


		public JObject ToJson()
		{
			JObject jsonObject = new JObject();
			jsonObject.Add("column_type", column_type.ToString());
			jsonObject.Add("data_type", data_type.ToString());
			jsonObject.Add("data_name", data_name);
			return jsonObject;
		}

		public void Serialize(ByteBuffer byteBuffer)
		{
            byteBuffer.WriteString(data_name);
            byteBuffer.WriteString(enum_type_name);
            byteBuffer.WriteInteger((int)column_type);
            byteBuffer.WriteInteger((int)data_type);
        }
	}

	public class Record
	{
		public List<string> data_value = new List<string>();
		public Record() { }
		public Record(JArray _json)
		{
			foreach(JToken token in _json)
			{
				string data = token.ToObject<string>();
				data_value.Add(data);
			}
		}

		public Record(ByteBuffer buffer, Column column)
        {
            int data_count = buffer.ReadInteger();
            for (int i = 0; i < data_count; i++)
            {
				switch (column.data_type)
				{
                    case eDataType.INT16:
                        data_value.Add(buffer.ReadShort().ToString());
                        break;
                    case eDataType.INT32:
                        data_value.Add(buffer.ReadInteger().ToString());
                        break;
                    case eDataType.INT64:
                        data_value.Add(buffer.ReadLong().ToString());
                        break;
                    case eDataType.STRING:
                        data_value.Add(buffer.ReadString());
                        break;
                    case eDataType.WSTRING:
                        data_value.Add(buffer.ReadString());
                        break;
                    case eDataType.FLOAT:
                        data_value.Add(buffer.ReadFloat().ToString());
                        break;
                    case eDataType.DOUBLE:
                        data_value.Add(buffer.ReadDouble().ToString());
                        break;
                    case eDataType.BOOLEAN:
                        data_value.Add(buffer.ReadBool().ToString());
                        break;
                    case eDataType.DATE:
                        data_value.Add(DateTime.FromBinary(buffer.ReadLong()).ToString("yyyy-MM-dd"));
                        break;
                    case eDataType.ENUM:
                        data_value.Add(buffer.ReadString());
                        break;
                }
            }
        }

        public JArray ToJson()
		{
			JArray ja = new JArray();
			data_value.ForEach(i => ja.Add(i));
			return ja;
		}

		public void Serialize(ByteBuffer byteBuffer, List<Column> columns, List<EnumData> enums)
        {
            for (int i = 0; i < data_value.Count; i++)
            {
				switch(columns[i].data_type)
				{
					case eDataType.INT16:
                        byteBuffer.WriteShort(Convert.ToInt16(data_value[i]));
                        break;
                    case eDataType.INT32:
                        byteBuffer.WriteInteger(Convert.ToInt32(data_value[i]));
                        break;
                    case eDataType.INT64:
                        byteBuffer.WriteLong(Convert.ToInt64(data_value[i]));
                        break;
                    case eDataType.STRING:
                        byteBuffer.WriteString(data_value[i]);
                        break;
                    case eDataType.WSTRING:
                        byteBuffer.WriteStringUnicode(data_value[i]);
                        break;
                    case eDataType.FLOAT:
                        byteBuffer.WriteFloat(Convert.ToSingle(data_value[i]));
                        break;
                    case eDataType.DOUBLE:
                        byteBuffer.WriteDouble(Convert.ToDouble(data_value[i]));
                        break;
                    case eDataType.BOOLEAN:
                        byteBuffer.WriteBool(Convert.ToBoolean(data_value[i]));
                        break;
                    case eDataType.DATE:
                        DateTime dateValue = DateTime.Parse(data_value[i]);
                        byteBuffer.WriteLong(dateValue.Ticks);
                        break;
                    case eDataType.ENUM:
                        EnumData enum_data = enums.Find(x => x.data_name == data_value[i] && x.type_name == columns[i].enum_type_name);
                        if (enum_data == null)
                        {
                            throw new Exception($"Enum data not found for {data_value[i]} in {columns[i].enum_type_name}");
                        }
                        byteBuffer.WriteInteger(enum_data.data_index);
                        break;
					default:
                        throw new Exception($"Invalid data type {columns[i].data_type}");
                }
            }
        }
    }
}
