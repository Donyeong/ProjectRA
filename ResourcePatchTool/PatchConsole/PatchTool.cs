
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DResourceModule;
using ResourceModule;
using ResourceModuleTool;

namespace PatchConsole
{
    class PatchTool
    {
        private RefDataLoadManager m_ref_data_manager;
        private ExcelParser m_parser;
        private string m_include_folder_path;
        private string m_exclude_folder_path;

        public PatchTool()
        {
            m_ref_data_manager = new RefDataLoadManager();
            m_parser = new ExcelParser(m_ref_data_manager);
        }

        private void SetIncludeFolderPath(string _path)
        {
            m_include_folder_path = _path;
        }

        private void SetExcludeFolderPath(string _path)
        {
            m_exclude_folder_path = _path;
        }


        public void LoadExcel(string _project_path)
        {
            string excel_path = Path.Combine(_project_path, "Excel");
            string resource_path = Path.Combine(_project_path, "ProjectRA", "Assets", "StreamingAssets");
            SetIncludeFolderPath(excel_path);
            SetExcludeFolderPath(resource_path);

            m_parser.LoadExcels(excel_path);


            string code_path_cs = Path.Combine(_project_path, "ProjectRA\\Assets\\Scripts\\RefData");

            ReferenceCodeGeneratorCS ref_code_generator_cs = new ReferenceCodeGeneratorCS();
            foreach (var table in m_ref_data_manager.ref_data_tables.Values)
            {
                ref_code_generator_cs.GenerateRefFile(table, code_path_cs);
                ref_code_generator_cs.GenerateEnumFile(m_ref_data_manager.enum_datas, code_path_cs);
            }
            ref_code_generator_cs.GenerateFactoryFile(m_ref_data_manager.ref_data_tables.Values.ToList(), code_path_cs);
            m_parser.packs(m_exclude_folder_path);

        }
    }
}
