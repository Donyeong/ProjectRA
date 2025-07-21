using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using ResourceModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
namespace DResourceModule
{
	public class RefDataLoaderFromFile : RefDataLoaderBase
	{
		public Dictionary<string, List<RefDataBase>> ref_datas = new Dictionary<string, List<RefDataBase>>();

		public Dictionary<string, byte[]> LoadRefData(string _file_path)
		{
            Dictionary<string, byte[]> datas = null;

            try
			{
				// Resources 폴더에서 압축 파일을 읽기
				byte[] compressedData = LoadCompressedFile(_file_path);

				if (compressedData != null)
				{
                    // 압축 해제
                    datas = UnzipInMemory(compressedData);

					logger.WriteLogLine(eLogType.Info, "압축 해제 완료");
				}
				else
				{
					logger.WriteLogLine(eLogType.Error, "압축 파일을 로드할 수 없습니다.");
				}
			}
			catch (InvalidDataException ide)
			{
				logger.WriteLogLine(eLogType.Error, $"압축 파일 데이터 오류: {ide.Message}");
			}
			catch (Exception ex)
			{
				logger.WriteLogLine(eLogType.Error, $"압축 해제 중 오류 발생: {ex.Message} #### {ex.StackTrace}");
			}

			return datas;
        }

		// Resources 폴더에서 압축 파일 읽기
		byte[] LoadCompressedFile(string _file_path)
		{

			if (File.Exists(_file_path))
			{
				byte[] fileBytes = File.ReadAllBytes(_file_path);
				logger.WriteLogLine(eLogType.Info, "파일 읽기 성공, 바이트 크기: {fileBytes.Length}");
				return fileBytes; // 파일을 바이트 배열로 반환
			}
			else
			{
				logger.WriteLogLine(eLogType.Error, $"파일을 찾을 수 없습니다: {_file_path}");
				return null; // 파일을 바이트 배열로 반환
			}
		}




		// 메모리에서 압축 해제
		Dictionary<string, byte[]> UnzipInMemory(byte[] compressedData)
		{
			Dictionary<string, byte[]> unzippedData = new Dictionary<string, byte[]>();
            using (MemoryStream memoryStream = new MemoryStream(compressedData))
			{
				using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						using (MemoryStream entryStream = new MemoryStream())
						{
							// 압축 해제된 파일을 메모리 스트림으로 읽기
							using (Stream entryData = entry.Open())
							{
								entryData.CopyTo(entryStream);
							}

							// 메모리 스트림에서 데이터 처리
							byte[] entryDataBytes = entryStream.ToArray();
							logger.WriteLogLine(eLogType.Info, $"Read RefData: [{entry.FullName}] Size: {entryDataBytes.Length}");
							ByteBuffer byteBuffer = new ByteBuffer();
                            byteBuffer.WriteBytes(entryDataBytes);
                            unzippedData.Add(entry.FullName, entryDataBytes);
                            //RefDataTable table = new RefDataTable();
                            //table.Deserialize(byteBuffer);
                            //ref_data_tble.Add(table.table_name, table);
                            /*
							RefDataBase ref_data = ConvertToDataClass(table);

							if (!ref_datas.ContainsKey(table.table_name))
							{
								ref_datas[table.table_name] = new List<RefDataBase>();
							}
							ref_datas[table.table_name].Add(ref_data);
							*/
                        }
					}
				}
			}

			return unzippedData;
        }

		RefDataTable ParseJson(string _json_data)
		{
			RefDataTable refDataTable = new RefDataTable();
			JObject json = JObject.Parse(_json_data);
			refDataTable.table_name = GetJsonValue<string>(json , "table_name");
			JArray columns = GetJsonValue<JArray>(json, "columns");
			JArray records = GetJsonValue<JArray>(json, "records");

			logger.WriteLogLine(eLogType.Info, $"column : {columns.ToString()}");
			foreach(JToken i in columns)
			{
				JObject column_json = i.ToObject<JObject>();
				Column column = new Column(column_json);
				refDataTable.columns.Add(column);
			}
			foreach (JToken i in records)
			{
				JArray record_json = i.ToObject<JArray>();
				Record record = new Record(record_json);
				refDataTable.records.Add(record);
			}
			return refDataTable;
		}

		public T GetJsonValue<T>(JObject jo, string key)
		{
			// key가 존재하면 해당 값을 반환, 없으면 기본값(default)을 반환
			if (jo.TryGetValue(key, out JToken value))
			{
				return value.ToObject<T>();
			}
			return default;
		}
	}
}
