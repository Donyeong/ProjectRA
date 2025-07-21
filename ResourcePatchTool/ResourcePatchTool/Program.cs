using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


public class RefSkill
{
	int skill_id;
}
namespace ResourcePatchTool
{
	internal static class Program
	{
		/// <summary>
		/// 해당 애플리케이션의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new Form1());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			};
		}
	}
}
