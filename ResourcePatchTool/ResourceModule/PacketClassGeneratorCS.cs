using DResourceModule;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace ResourceModule
{
	public class PacketClassGeneratorCPP
	{
		public void GeneratePacketClassFile(PacketClassJson _packet_class_json, string _file_path)
		{
			GenerateClassHeader(_packet_class_json, _file_path);
			//GenerateClassCpp(_table, _folder_path);
		}

		private void GenerateClassHeader(PacketClassJson _packet_class_json, string _file_path)
		{
			// 1. 기존 파일에서 커스텀 코드 추출
			string customCode = string.Empty;
			string customInclude = string.Empty;
			string customUnpack = string.Empty;
			string customReceived = string.Empty;
			string customDefine = string.Empty;
			if (File.Exists(_file_path))
			{
				var lines = File.ReadAllLines(_file_path);
				customCode		= Util.GetCodeRegion(lines, "CustomCode");
				customInclude	= Util.GetCodeRegion(lines, "CustomInclude");
				customInclude	= Util.GetCodeRegion(lines, "CustomUnpack");
				customReceived	= Util.GetCodeRegion(lines, "CustomReceived");
				customDefine	= Util.GetCodeRegion(lines, "CustomDefine");
			}
			// 2. 새 파일 생성
			String class_code_str =
@"	
<custom_include>
<custom_define>
namespace DNNet
{
	public class <class_name> : <packet_base>
	{
<member>
        public override GS2CPacketType GetPacketType()
		{
			return GS2CPacketType.HeartBeet;
        }
        protected override void <parse_method_name>(DataPack _data_pack)
        {
<parse>
<custom_unpack>
        }

		<receive_method>
<custom_code>
	}
}
";
			String receive_method =
@"
		public override void OnReceived()
		{
<custom_received>
        }
";


			class_code_str = class_code_str.Replace("<packet_body_name>", _packet_class_json.GetPacketBodyName());
			class_code_str = class_code_str.Replace("<class_name>", _packet_class_json.packet_name);
			class_code_str = class_code_str.Replace("<member>", GenerateClassMember(_packet_class_json));
			class_code_str = class_code_str.Replace("<parse>", GenerateParseCode(_packet_class_json));
			class_code_str = class_code_str.Replace("<packet_base>", _packet_class_json.packet_type == PacketType.Server ? "ServerPacketBase" : "ClientPacketBase");
			class_code_str = class_code_str.Replace("<receive_method>", _packet_class_json.packet_type == PacketType.Server ? receive_method : "");
			class_code_str = class_code_str.Replace("<parse_method_name>", _packet_class_json.packet_type == PacketType.Server ? "UnpackBody" : "PackBody");
			class_code_str = class_code_str.Replace("<custom_code>", customCode?.TrimEnd() ?? "");
			class_code_str = class_code_str.Replace("<custom_include>", customInclude?.TrimEnd() ?? "");
			class_code_str = class_code_str.Replace("<custom_unpack>", customUnpack?.TrimEnd() ?? "");
			class_code_str = class_code_str.Replace("<custom_received>", customReceived?.TrimEnd() ?? "");
			class_code_str = class_code_str.Replace("<custom_define>", customDefine?.TrimEnd() ?? "");

			String class_header_file_path = Path.Combine(_file_path);
			File.WriteAllText(class_header_file_path, class_code_str);

			//String class_cpp_file_path = Path.Combine(_folder_path, String.Format("{0}.cpp", _table.table_name));
			//File.WriteAllText(class_cpp_file_path, class_code_str);
		}
		private string GenerateClassMember(PacketClassJson _packet_class_json)
		{
			StringBuilder sb = new StringBuilder();


			foreach (var member in _packet_class_json.members)
			{
				string type_name = member.type_name;
				sb.AppendLine($"\t{type_name}\t{member.member_name};");
			}

			//ClassType.Members.Add(_GenerateParseMethodCode());
			return sb.ToString();
		}

		private string GenerateParseCode(PacketClassJson _packet_class_json)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var member in _packet_class_json.members)
			{
				string type_name = member.type_name;
				string member_name = member.member_name;
				if(_packet_class_json.packet_type == PacketType.Client)
				{
					sb.AppendLine($"\t\t\t_data_pack.Pack<{type_name}>(\"{member_name}\", {member_name});");
				} else
				{
					sb.AppendLine($"\t\t\t{member_name} = _data_pack.Unpack<{type_name}>(\"{member_name}\");");
				}
			}

			return sb.ToString();
		}
	}
}
