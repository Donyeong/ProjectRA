
using System.Configuration;

namespace PatchConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PatchTool patchTool = new PatchTool();
            string path = ConfigurationManager.AppSettings["ProjectPath"];
			patchTool.LoadExcel(path);
			//patchTool.LoadExcel("F:\\Works\\Projects\\ProjectDDA");

		}
    }
}