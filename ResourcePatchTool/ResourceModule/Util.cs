using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceModule
{
	internal class Util
	{
		public static string ToSnakeCase(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			var sb = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (char.IsUpper(c))
				{
					if (i > 0)
						sb.Append('_');
					sb.Append(char.ToLower(c));
				}
				else
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}


		public static string GetCodeRegion(string[] _full_code_lines, string _region_name)
		{
			bool inRegion = false;
			int region_stack = 0;
			var sb = new StringBuilder();
			foreach (var line in _full_code_lines)
			{
				if (line.Trim().StartsWith($"#region {_region_name}"))
				{
					region_stack++;
					inRegion = true;
					continue;
				}
				if (inRegion && line.Trim().StartsWith("#endregion"))
				{
					region_stack--;
					if (region_stack == 0)
					{
						inRegion = false;
						break;
					}
				}
				if (inRegion)
				{
					sb.AppendLine(line);
				}
			}
			return sb.ToString();
		}
	}
}
