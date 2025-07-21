using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using ResourceModule;
using DResourceModule;
using OfficeOpenXml;
using System.IO.Compression;

namespace ResourceModuleTool
{
	public class ExcelParser
	{
		public RefDataLoadManager ref_data_manager;
		public ResourceLogger logger => ref_data_manager.logger;
		public Dictionary<string, RefDataTable> refDataTable => ref_data_manager.ref_data_tables;

		private const int DATA_NAME_LNIE = 0;
        private const int DATA_TYPE_LINE = 1;
        private const int COLUMN_TYPE_LINE = 2;
        private const int DATA_LINE = 3;

        public ExcelParser(RefDataLoadManager _data_manager)
		{
			ref_data_manager = _data_manager;
		}
		public void LoadExcels(string folder_path)
		{
			List<string> files = new List<string>();
			LoadExcelFileList(folder_path, out files);
			files.ForEach(i => LoadExcel(i));
		}

		public void LoadExcel(string file_path)
		{
			RefDataTable ref_data_table = new RefDataTable();
			ref_data_table.table_name = Path.GetFileNameWithoutExtension(file_path);

			logger.WriteLogLine(eLogType.Info, $"include table [{ref_data_table.table_name}]");

			ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // 비상업적 사용 설정

			using (var package = new ExcelPackage(new FileInfo(file_path)))
			{
				var worksheet = package.Workbook.Worksheets[0]; // 첫 번째 워크시트
				int rowCount = worksheet.Dimension.Rows; // 사용된 행 수
				int colCount = worksheet.Dimension.Columns; // 사용된 열 수
				var table = worksheet.Tables[ref_data_table.table_name];         // 테이블 이름으로 테이블 가져오기

				if (table != null)
				{
					// 테이블 범위
					var address = table.Address; // 테이블의 시작/끝 범위
												 //WriteLogLine(eLogType.Info, $"테이블 범위: {address.Start.Address} - {address.End.Address}");


					//컬럼 파싱
					Dictionary<int, Column> table_index_to_column = new Dictionary<int, Column>();
					for (int col = address.Start.Column; col <= address.End.Column; col++)
					{
						var data_name = worksheet.Cells[address.Start.Row + DATA_NAME_LNIE, col].Text;
						var data_type = worksheet.Cells[address.Start.Row + DATA_TYPE_LINE, col].Text;
						var colmun_type = worksheet.Cells[address.Start.Row + COLUMN_TYPE_LINE, col].Text;
						Column column = new Column(data_name, data_type, colmun_type);

						ref_data_table.columns.Add(column);
						table_index_to_column.Add(col, column);
					}
					//데이터 파싱
					for (int row = address.Start.Row + DATA_LINE; row <= address.End.Row; row++) // 헤더 제외
					{
						Record record = new Record();
						for (int col = address.Start.Column; col <= address.End.Column; col++)
						{
							var cellValue = worksheet.Cells[row, col].Text; // 셀 값 가져오기
							record.data_value.Add(cellValue);
						}
						ref_data_table.records.Add(record);
					}
				}
				else
				{
					logger.WriteLogLine(eLogType.Error, $"테이블 '{ref_data_table.table_name}'을 찾을 수 없습니다.");
				}
			}
			/*
			ref_data_table.columns.ForEach(c => WriteLog(eLogType.Info, $"{c.data_name}\t"));
			WriteLogLine(eLogType.Info, $"");
			ref_data_table.columns.ForEach(c => WriteLog(eLogType.Info, $"{c.column_type.ToString()}\t"));
			WriteLogLine(eLogType.Info, $"");
			ref_data_table.columns.ForEach(c => WriteLog(eLogType.Info, $"{c.data_type.ToString()}\t"));
			WriteLogLine(eLogType.Info, $"");

			foreach (var r in ref_data_table.records)
			{
				r.data_value.ForEach(d => WriteLog(eLogType.Info, $"{d}\t"));
				WriteLogLine(eLogType.Info, $"");
			}
			*/

			if(ref_data_table.table_name == "Enum")
			{
				List<EnumData> enum_datas = new List<EnumData>();

                foreach (var r in ref_data_table.records)
                {
                    EnumData enum_data = new EnumData();
                    enum_data.type_name = r.data_value[0];
                    enum_data.data_name = r.data_value[1];
                    enum_data.data_index = int.Parse(r.data_value[2]);
                    enum_data.comment = r.data_value[3];
                    enum_datas.Add(enum_data);
                }

                ref_data_manager.enum_datas = enum_datas;

            } else
			{
                refDataTable.Add(ref_data_table.table_name, ref_data_table);
            }

		}

		public void WriteLogData(string table_name)
		{
			if (!refDataTable.ContainsKey(table_name))
			{
				return;
			}
			StringBuilder table_data_string = new StringBuilder();
			RefDataTable ref_data_table = refDataTable[table_name];
			ref_data_table.columns.ForEach(c => table_data_string.Append($"{c.data_name}\t"));
			table_data_string.AppendLine();
			ref_data_table.columns.ForEach(c => table_data_string.Append($"{c.column_type.ToString()}\t"));
			table_data_string.AppendLine();
			ref_data_table.columns.ForEach(c => table_data_string.Append($"{c.data_type.ToString()}\t"));
			table_data_string.AppendLine();

			foreach (var r in ref_data_table.records)
			{
				r.data_value.ForEach(d => table_data_string.Append($"{d}\t"));
				table_data_string.AppendLine();
			}
			logger.WriteLogLine(eLogType.Info, table_data_string.ToString());
		}

		public static eResult LoadExcelFileList(string folder_path, out List<string> files)
		{

			// 폴더가 존재하는지 확인
			if (Directory.Exists(folder_path))
			{
				files = Directory.GetFiles(folder_path, "*.xlsx")
													  .ToList();
				return eResult.Success;
			}
			else
			{
				files = new List<string>();
				return eResult.Error_NotFoundFolder;
			}
		}

		public void packs(string export_path)
		{
			foreach (var table in refDataTable.Values)
			{
				pack(export_path, table);
			}
			CreateZipFromFiles(export_path);
		}
		public void pack(string export_path, RefDataTable table)
		{
			// 파일 이름 정의
			string file_name = $"{table.table_name}.ref";

			// 저장할 전체 경로
			string folder_full_path = Path.Combine(export_path, "Tables");
			// 저장할 전체 경로
			string fullPath = Path.Combine(folder_full_path, file_name);

			// 폴더가 존재하지 않으면 생성
			if (!Directory.Exists(folder_full_path))
			{
				Directory.CreateDirectory(folder_full_path);
				logger.WriteLogLine(eLogType.Info, "폴더가 생성되었습니다: " + folder_full_path);
			}

			// List<string>을 파일로 저장
			try
			{
                ByteBuffer byteBuffer = new ByteBuffer();
                table.Serialize(byteBuffer, ref_data_manager.enum_datas);
                byte[] dataString = byteBuffer.ToArray();
                System.IO.File.WriteAllBytes(fullPath, dataString);
                logger.WriteLogLine(eLogType.Info, $"파일이 성공적으로 저장되었습니다: {fullPath}");
			}
			catch (Exception ex)
			{
				logger.WriteLogLine(eLogType.Error, $"파일 저장 중 오류 발생: {ex.Message}");
			}
		}

		public void CreateZipFromFiles(string export_path)
		{

			// 파일 이름 정의
			string file_name = $"data.refs";

			// 저장할 전체 경로
			string full_path = Path.Combine(export_path, file_name);

			// ZIP 파일을 생성 (파일이 이미 존재하면 덮어쓰기)
			using (FileStream zip_to_create = new FileStream(full_path, FileMode.Create))
			{
				using (ZipArchive archive = new ZipArchive(zip_to_create, ZipArchiveMode.Create))
				{
					foreach (var table in refDataTable.Values)
					{
						// 파일 이름 정의
						string table_file_name = $"{table.table_name}.ref";

						// 저장할 전체 경로
						string table_full_path = Path.Combine(export_path, "Tables", table_file_name);

						// 파일이 존재하는지 확인
						if (File.Exists(table_full_path))
						{
							// 압축 파일 내에 추가할 항목을 만들기
							archive.CreateEntryFromFile(table_full_path, table_file_name);
						}
						else
						{
							logger.WriteLogLine(eLogType.Error, $"파일이 존재하지 않습니다: {table_full_path}");
						}
					}
				}
			}
			logger.WriteLogLine(eLogType.Info, $"데이터 생성이 완료되었습니다: {full_path}");
		}
	}
}
