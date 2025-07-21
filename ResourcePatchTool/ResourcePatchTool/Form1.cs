using Microsoft.WindowsAPICodePack.Dialogs;
using DResourceModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;

namespace ResourcePatchTool
{
	public partial class Form1 : Form
	{
		private string include_folder_path;
		private string exclude_folder_path;
		private ResourceToolManager tool_manager = new ResourceToolManager();
		public Form1()
		{
			InitializeComponent();


			tool_manager.ref_data_manager.logger.logEvent = (log_type, log_string) =>
			{
				logTextBox.AppendText(log_string);
			};
		}

		private void SetIncludeFolderPath(string path)
		{
			include_folder_path = path;
			includeFolderPathText.Text = path;
		}

		private void SetExcludeFolderPath(string path)
		{
			exclude_folder_path = path;
			excludeFolderPathText.Text = path;
		}

		private void IncludeFolderSelectClick(object sender, EventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true; // true : 폴더 선택 / false : 파일 선택

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				SetIncludeFolderPath(dialog.FileName);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			tool_manager.parser.LoadExcels(include_folder_path);
			updateListBoxUI();
		}

		private void updateListBoxUI()
		{
			listBox1.Items.Clear();
			foreach (var table in tool_manager.parser.refDataTable)
			{
				listBox1.Items.Add(table.Value.table_name);
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(listBox1.SelectedItem == null)
			{
				return;
			}
			string selectedItem = listBox1.SelectedItem.ToString();
			tool_manager.parser.WriteLogData(selectedItem);
		}

		private void ExcludeFolderSelectClick(object sender, EventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true; // true : 폴더 선택 / false : 파일 선택

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				SetExcludeFolderPath(dialog.FileName);
			}
		}

		private void includeFolderPathText_TextChanged(object sender, EventArgs e)
		{

		}

		private void logTextBox_TextChanged(object sender, EventArgs e)
		{

		}

		private void export_Click(object sender, EventArgs e)
		{
			tool_manager.parser.packs(exclude_folder_path);
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void SelectProject_Click(object sender, EventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true; // true : 폴더 선택 / false : 파일 선택

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				string project_path = dialog.FileName;
				string excel_path = Path.Combine(project_path, "Excel");
				string resource_path = Path.Combine(project_path, "DDA", "Assets", "StreamingAssets");
				SetIncludeFolderPath(excel_path);
				SetExcludeFolderPath(resource_path);
			}
		}

		private void TestLoad_Click(object sender, EventArgs e)
		{
			string path = Path.Combine(exclude_folder_path, "data.refs");
			RefDataLoaderFromFile loader = new RefDataLoaderFromFile();
			loader.logger = tool_manager.ref_data_manager.logger;
			var datas = loader.LoadRefData(path);

		}
	}
}
