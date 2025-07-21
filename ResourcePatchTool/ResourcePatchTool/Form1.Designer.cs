namespace ResourcePatchTool
{
	partial class Form1
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.include_folder_find = new System.Windows.Forms.Button();
			this.includeFolderPathText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.excludeFolderPathText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.exclude_folder_find = new System.Windows.Forms.Button();
			this.logTextBox = new System.Windows.Forms.TextBox();
			this.export = new System.Windows.Forms.Button();
			this.SelectProject = new System.Windows.Forms.Button();
			this.TestLoad = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// include_folder_find
			// 
			this.include_folder_find.Location = new System.Drawing.Point(622, 46);
			this.include_folder_find.Name = "include_folder_find";
			this.include_folder_find.Size = new System.Drawing.Size(75, 23);
			this.include_folder_find.TabIndex = 0;
			this.include_folder_find.Text = "찾기";
			this.include_folder_find.UseVisualStyleBackColor = true;
			this.include_folder_find.Click += new System.EventHandler(this.IncludeFolderSelectClick);
			// 
			// includeFolderPathText
			// 
			this.includeFolderPathText.Location = new System.Drawing.Point(12, 46);
			this.includeFolderPathText.Name = "includeFolderPathText";
			this.includeFolderPathText.Size = new System.Drawing.Size(604, 21);
			this.includeFolderPathText.TabIndex = 1;
			this.includeFolderPathText.TextChanged += new System.EventHandler(this.includeFolderPathText_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "엑셀 폴더 선택";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(12, 123);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 3;
			this.button2.Text = "Include";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new System.Drawing.Point(14, 152);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(160, 388);
			this.listBox1.TabIndex = 4;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// excludeFolderPathText
			// 
			this.excludeFolderPathText.Location = new System.Drawing.Point(12, 85);
			this.excludeFolderPathText.Name = "excludeFolderPathText";
			this.excludeFolderPathText.Size = new System.Drawing.Size(604, 21);
			this.excludeFolderPathText.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "출력 폴더 선택";
			// 
			// exclude_folder_find
			// 
			this.exclude_folder_find.Location = new System.Drawing.Point(622, 85);
			this.exclude_folder_find.Name = "exclude_folder_find";
			this.exclude_folder_find.Size = new System.Drawing.Size(75, 23);
			this.exclude_folder_find.TabIndex = 7;
			this.exclude_folder_find.Text = "찾기";
			this.exclude_folder_find.UseVisualStyleBackColor = true;
			this.exclude_folder_find.Click += new System.EventHandler(this.ExcludeFolderSelectClick);
			// 
			// logTextBox
			// 
			this.logTextBox.Location = new System.Drawing.Point(205, 152);
			this.logTextBox.Multiline = true;
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logTextBox.Size = new System.Drawing.Size(492, 388);
			this.logTextBox.TabIndex = 8;
			this.logTextBox.TextChanged += new System.EventHandler(this.logTextBox_TextChanged);
			// 
			// export
			// 
			this.export.Location = new System.Drawing.Point(93, 123);
			this.export.Name = "export";
			this.export.Size = new System.Drawing.Size(75, 23);
			this.export.TabIndex = 9;
			this.export.Text = "export";
			this.export.UseVisualStyleBackColor = true;
			this.export.Click += new System.EventHandler(this.export_Click);
			// 
			// SelectProject
			// 
			this.SelectProject.Location = new System.Drawing.Point(205, 123);
			this.SelectProject.Name = "SelectProject";
			this.SelectProject.Size = new System.Drawing.Size(190, 23);
			this.SelectProject.TabIndex = 10;
			this.SelectProject.Text = "프로젝트 폴더 선택";
			this.SelectProject.UseVisualStyleBackColor = true;
			this.SelectProject.Click += new System.EventHandler(this.SelectProject_Click);
			// 
			// TestLoad
			// 
			this.TestLoad.Location = new System.Drawing.Point(401, 123);
			this.TestLoad.Name = "TestLoad";
			this.TestLoad.Size = new System.Drawing.Size(190, 23);
			this.TestLoad.TabIndex = 11;
			this.TestLoad.Text = "TestLoad";
			this.TestLoad.UseVisualStyleBackColor = true;
			this.TestLoad.Click += new System.EventHandler(this.TestLoad_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(746, 569);
			this.Controls.Add(this.TestLoad);
			this.Controls.Add(this.SelectProject);
			this.Controls.Add(this.export);
			this.Controls.Add(this.logTextBox);
			this.Controls.Add(this.exclude_folder_find);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.excludeFolderPathText);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.includeFolderPathText);
			this.Controls.Add(this.include_folder_find);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button include_folder_find;
		private System.Windows.Forms.TextBox includeFolderPathText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TextBox excludeFolderPathText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button exclude_folder_find;
		private System.Windows.Forms.TextBox logTextBox;
		private System.Windows.Forms.Button export;
		private System.Windows.Forms.Button SelectProject;
		private System.Windows.Forms.Button TestLoad;
	}
}

