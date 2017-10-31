namespace Konamiman.SuperBookmarks.Options
{
    partial class ConfirmationOptionsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.chkDocument = new System.Windows.Forms.CheckBox();
            this.chkOpenFiles = new System.Windows.Forms.CheckBox();
            this.chkFolder = new System.Windows.Forms.CheckBox();
            this.chkProject = new System.Windows.Forms.CheckBox();
            this.chkSolution = new System.Windows.Forms.CheckBox();
            this.chkConfirmOnImportReplace = new System.Windows.Forms.CheckBox();
            this.chkConfirmOnManualLoad = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(685, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ask for confirmation when deleting all bookmarks in...";
            // 
            // chkDocument
            // 
            this.chkDocument.AutoSize = true;
            this.chkDocument.Location = new System.Drawing.Point(57, 60);
            this.chkDocument.Margin = new System.Windows.Forms.Padding(5);
            this.chkDocument.Name = "chkDocument";
            this.chkDocument.Size = new System.Drawing.Size(182, 36);
            this.chkDocument.TabIndex = 1;
            this.chkDocument.Text = "Document";
            this.chkDocument.UseVisualStyleBackColor = true;
            // 
            // chkOpenFiles
            // 
            this.chkOpenFiles.AutoSize = true;
            this.chkOpenFiles.Location = new System.Drawing.Point(57, 108);
            this.chkOpenFiles.Margin = new System.Windows.Forms.Padding(5);
            this.chkOpenFiles.Name = "chkOpenFiles";
            this.chkOpenFiles.Size = new System.Drawing.Size(182, 36);
            this.chkOpenFiles.TabIndex = 2;
            this.chkOpenFiles.Text = "Open files";
            this.chkOpenFiles.UseVisualStyleBackColor = true;
            // 
            // chkFolder
            // 
            this.chkFolder.AutoSize = true;
            this.chkFolder.Location = new System.Drawing.Point(57, 156);
            this.chkFolder.Margin = new System.Windows.Forms.Padding(5);
            this.chkFolder.Name = "chkFolder";
            this.chkFolder.Size = new System.Drawing.Size(134, 36);
            this.chkFolder.TabIndex = 3;
            this.chkFolder.Text = "Folder";
            this.chkFolder.UseVisualStyleBackColor = true;
            // 
            // chkProject
            // 
            this.chkProject.AutoSize = true;
            this.chkProject.Location = new System.Drawing.Point(57, 204);
            this.chkProject.Margin = new System.Windows.Forms.Padding(5);
            this.chkProject.Name = "chkProject";
            this.chkProject.Size = new System.Drawing.Size(142, 36);
            this.chkProject.TabIndex = 4;
            this.chkProject.Text = "Project";
            this.chkProject.UseVisualStyleBackColor = true;
            // 
            // chkSolution
            // 
            this.chkSolution.AutoSize = true;
            this.chkSolution.Location = new System.Drawing.Point(57, 252);
            this.chkSolution.Margin = new System.Windows.Forms.Padding(5);
            this.chkSolution.Name = "chkSolution";
            this.chkSolution.Size = new System.Drawing.Size(158, 36);
            this.chkSolution.TabIndex = 5;
            this.chkSolution.Text = "Solution";
            this.chkSolution.UseVisualStyleBackColor = true;
            // 
            // chkConfirmOnImportReplace
            // 
            this.chkConfirmOnImportReplace.AutoSize = true;
            this.chkConfirmOnImportReplace.Location = new System.Drawing.Point(23, 332);
            this.chkConfirmOnImportReplace.Margin = new System.Windows.Forms.Padding(5);
            this.chkConfirmOnImportReplace.Name = "chkConfirmOnImportReplace";
            this.chkConfirmOnImportReplace.Size = new System.Drawing.Size(814, 36);
            this.chkConfirmOnImportReplace.TabIndex = 6;
            this.chkConfirmOnImportReplace.Text = "Deleting all bookmarks when importing requires confirmation";
            this.chkConfirmOnImportReplace.UseVisualStyleBackColor = true;
            // 
            // chkConfirmOnManualLoad
            // 
            this.chkConfirmOnManualLoad.AutoSize = true;
            this.chkConfirmOnManualLoad.Location = new System.Drawing.Point(23, 378);
            this.chkConfirmOnManualLoad.Margin = new System.Windows.Forms.Padding(5);
            this.chkConfirmOnManualLoad.Name = "chkConfirmOnManualLoad";
            this.chkConfirmOnManualLoad.Size = new System.Drawing.Size(646, 68);
            this.chkConfirmOnManualLoad.TabIndex = 7;
            this.chkConfirmOnManualLoad.Text = "Deleting all bookmarks when manually loading \r\n.SuperBookmarks.dat requires confi" +
    "rmation";
            this.chkConfirmOnManualLoad.UseVisualStyleBackColor = true;
            // 
            // ConfirmationOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkConfirmOnManualLoad);
            this.Controls.Add(this.chkConfirmOnImportReplace);
            this.Controls.Add(this.chkSolution);
            this.Controls.Add(this.chkProject);
            this.Controls.Add(this.chkFolder);
            this.Controls.Add(this.chkOpenFiles);
            this.Controls.Add(this.chkDocument);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ConfirmationOptionsControl";
            this.Size = new System.Drawing.Size(889, 482);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDocument;
        private System.Windows.Forms.CheckBox chkOpenFiles;
        private System.Windows.Forms.CheckBox chkFolder;
        private System.Windows.Forms.CheckBox chkProject;
        private System.Windows.Forms.CheckBox chkSolution;
        private System.Windows.Forms.CheckBox chkConfirmOnImportReplace;
        private System.Windows.Forms.CheckBox chkConfirmOnManualLoad;
    }
}
