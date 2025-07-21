using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceModule
{
	public enum PacketType
	{
		Server,
		Client,
	}
	[Serializable]
	public class PacketClassJson
	{
		public string packet_name;
		public List<PacketClassMemberJson> members = new List<PacketClassMemberJson>();
		public List<string> attributes = new List<string>(); // 클래스 어트리뷰트
		public PacketType packet_type;

		public string ToJson()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
		}
		public bool hasAttribute(string attribute_name)
		{
			return attributes.Any(attr => attr.Contains(attribute_name));
		}

		public string GetPacketBodyName()
		{
			return  packet_name.StartsWith("Packet_")
				  ? packet_name.Substring("Packet_".Length)
				  : packet_name;
		}
	}

	[Serializable]
	public class PacketClassMemberJson
	{
		public string type_name;
		public string member_name;
		public List<string> attributes = new List<string>(); // 멤버 어트리뷰트

		public bool hasAttribute(string attribute_name)
		{
			return attributes.Any(attr => attr.Contains(attribute_name));
		}
	}


	public class PacketClassParser
	{
		public static Dictionary<string, PacketClassJson> ParseFromCsFileFolder(string folder_path)
		{
			Dictionary<string, PacketClassJson> packet_classes = new Dictionary<string, PacketClassJson>();
			var files = System.IO.Directory.GetFiles(folder_path, "*.cs", System.IO.SearchOption.AllDirectories);
			foreach (var file in files)
			{
				try
				{
					var packet_class = ParseFromCsFile(file);
					if (packet_class != null && !string.IsNullOrEmpty(packet_class.packet_name))
					{
						packet_classes.Add(packet_class.packet_name, packet_class);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error parsing file {file}: {ex.Message}");
				}
			}
			return packet_classes;
		}
		public static PacketClassJson ParseFromCsFile(string file_path)
		{
			var packet_class = new PacketClassJson();
			var lines = System.IO.File.ReadAllLines(file_path);

			bool inClass = false;
			List<string> pendingAttributes = new List<string>();

			foreach (var rawLine in lines)
			{
				var line = rawLine.Trim();

				// 어트리뷰트 파싱 ([Attr] 또는 [Attr(...)] 등)
				if (line.StartsWith("[") && line.EndsWith("]"))
				{
					pendingAttributes.Add(line);
					continue;
				}

				// 클래스 선언 찾기
				if (!inClass && ((line.Contains(": ClientPacketBase") || line.Contains(": ServerPacketBase")) && line.Contains("class ")))
				{
					var tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					int idx = Array.IndexOf(tokens, "class");
					if (idx >= 0 && idx + 1 < tokens.Length)
					{
						packet_class.packet_name = tokens[idx + 1];
						packet_class.attributes.AddRange(pendingAttributes); // 클래스 어트리뷰트 저장
						pendingAttributes.Clear();
						inClass = true;

						if(line.Contains(": ClientPacketBase"))
						{
							packet_class.packet_type = PacketType.Client;
						} else
						{
							packet_class.packet_type = PacketType.Server;
						}
					}
					continue;
				}

				// 클래스 내부 멤버(필드/프로퍼티) 추출
				if (inClass)
				{
					// 멤버 어트리뷰트 파싱
					if (line.StartsWith("[") && line.EndsWith("]"))
					{
						pendingAttributes.Add(line);
						continue;
					}

					// 필드: public int id;
					// 프로퍼티: public string Name { get; set; }
					if ((line.StartsWith("public ") || line.StartsWith("private ") || line.StartsWith("protected ")))
					{
						// 필드
						if (line.EndsWith(";"))
						{
							var tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
							if (tokens.Length >= 3)
							{
								string type = tokens[1];
								string name = tokens[2].TrimEnd(';');
								packet_class.members.Add(new PacketClassMemberJson
								{
									type_name = type,
									member_name = name,
									attributes = new List<string>(pendingAttributes)
								});
								pendingAttributes.Clear();
							}
						}
						// 프로퍼티
						else if (line.Contains("{") && line.Contains("}"))
						{
							var tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
							if (tokens.Length >= 3)
							{
								string type = tokens[1];
								string name = tokens[2];
								packet_class.members.Add(new PacketClassMemberJson
								{
									type_name = type,
									member_name = name,
									attributes = new List<string>(pendingAttributes)
								});
								pendingAttributes.Clear();
							}
						}
					}
					// 클래스 끝
					if (line.StartsWith("}"))
						break;
				}
			}

			return packet_class;
		}
	}
}
