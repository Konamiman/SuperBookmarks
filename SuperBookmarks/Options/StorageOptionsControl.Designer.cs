namespace Konamiman.SuperBookmarks
{
    partial class StorageOptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageOptionsControl));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbInOwnFile = new System.Windows.Forms.RadioButton();
            this.rbInSuo = new System.Windows.Forms.RadioButton();
            this.chkAutoIncludeInGitignore = new System.Windows.Forms.CheckBox();
            this.btnIncludeInGitignoreNow = new System.Windows.Forms.Button();
            this.btnOpenSuoFolder = new System.Windows.Forms.Button();
            this.lblIfGitignoreExists = new System.Windows.Forms.Label();
            this.pnlInfoMessage = new System.Windows.Forms.Panel();
            this.lblInfoMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.pnlInfoMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "On solution close, save bookmarks in";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbInOwnFile);
            this.panel1.Controls.Add(this.rbInSuo);
            this.panel1.Location = new System.Drawing.Point(7, 28);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(419, 54);
            this.panel1.TabIndex = 1;
            // 
            // rbInOwnFile
            // 
            this.rbInOwnFile.AutoSize = true;
            this.rbInOwnFile.Location = new System.Drawing.Point(4, 27);
            this.rbInOwnFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbInOwnFile.Name = "rbInOwnFile";
            this.rbInOwnFile.Size = new System.Drawing.Size(397, 21);
            this.rbInOwnFile.TabIndex = 1;
            this.rbInOwnFile.TabStop = true;
            this.rbInOwnFile.Text = ".SuperBookmarks.dat file in the same folder of the .suo file";
            this.rbInOwnFile.UseVisualStyleBackColor = true;
            // 
            // rbInSuo
            // 
            this.rbInSuo.AutoSize = true;
            this.rbInSuo.Location = new System.Drawing.Point(4, 3);
            this.rbInSuo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbInSuo.Name = "rbInSuo";
            this.rbInSuo.Size = new System.Drawing.Size(225, 21);
            this.rbInSuo.TabIndex = 0;
            this.rbInSuo.TabStop = true;
            this.rbInSuo.Text = "User solution options file (.suo)";
            this.rbInSuo.UseVisualStyleBackColor = true;
            // 
            // chkAutoIncludeInGitignore
            // 
            this.chkAutoIncludeInGitignore.AutoSize = true;
            this.chkAutoIncludeInGitignore.Location = new System.Drawing.Point(37, 79);
            this.chkAutoIncludeInGitignore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkAutoIncludeInGitignore.Name = "chkAutoIncludeInGitignore";
            this.chkAutoIncludeInGitignore.Size = new System.Drawing.Size(409, 21);
            this.chkAutoIncludeInGitignore.TabIndex = 2;
            this.chkAutoIncludeInGitignore.Text = "Include this file in .gitignore when solution loads if necessary";
            this.chkAutoIncludeInGitignore.UseVisualStyleBackColor = true;
            // 
            // btnIncludeInGitignoreNow
            // 
            this.btnIncludeInGitignoreNow.Location = new System.Drawing.Point(11, 146);
            this.btnIncludeInGitignoreNow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnIncludeInGitignoreNow.Name = "btnIncludeInGitignoreNow";
            this.btnIncludeInGitignoreNow.Size = new System.Drawing.Size(329, 28);
            this.btnIncludeInGitignoreNow.TabIndex = 4;
            this.btnIncludeInGitignoreNow.Text = "Include file in .gitignore of current solution";
            this.btnIncludeInGitignoreNow.UseVisualStyleBackColor = true;
            this.btnIncludeInGitignoreNow.Click += new System.EventHandler(this.btnIncludeInGitignoreNow_Click);
            // 
            // btnOpenSuoFolder
            // 
            this.btnOpenSuoFolder.Location = new System.Drawing.Point(11, 182);
            this.btnOpenSuoFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOpenSuoFolder.Name = "btnOpenSuoFolder";
            this.btnOpenSuoFolder.Size = new System.Drawing.Size(329, 28);
            this.btnOpenSuoFolder.TabIndex = 5;
            this.btnOpenSuoFolder.Text = "Open folder of .suo file for current solution";
            this.btnOpenSuoFolder.UseVisualStyleBackColor = true;
            this.btnOpenSuoFolder.Click += new System.EventHandler(this.btnOpenSuoFolder_Click);
            // 
            // lblIfGitignoreExists
            // 
            this.lblIfGitignoreExists.AutoSize = true;
            this.lblIfGitignoreExists.Location = new System.Drawing.Point(59, 99);
            this.lblIfGitignoreExists.Name = "lblIfGitignoreExists";
            this.lblIfGitignoreExists.Size = new System.Drawing.Size(323, 17);
            this.lblIfGitignoreExists.TabIndex = 3;
            this.lblIfGitignoreExists.Text = "(if .gitignore exists and file is not already included)";
            // 
            // pnlInfoMessage
            // 
            this.pnlInfoMessage.Controls.Add(this.lblInfoMessage);
            this.pnlInfoMessage.Controls.Add(this.pictureBox1);
            this.pnlInfoMessage.Location = new System.Drawing.Point(11, 223);
            this.pnlInfoMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlInfoMessage.Name = "pnlInfoMessage";
            this.pnlInfoMessage.Size = new System.Drawing.Size(480, 30);
            this.pnlInfoMessage.TabIndex = 6;
            // 
            // lblInfoMessage
            // 
            this.lblInfoMessage.AutoSize = true;
            this.lblInfoMessage.Location = new System.Drawing.Point(20, 2);
            this.lblInfoMessage.Name = "lblInfoMessage";
            this.lblInfoMessage.Size = new System.Drawing.Size(274, 17);
            this.lblInfoMessage.TabIndex = 0;
            this.lblInfoMessage.Text = "Ooops, you shouldn\'t be able to see this...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(26, 25);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // StorageOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlInfoMessage);
            this.Controls.Add(this.lblIfGitignoreExists);
            this.Controls.Add(this.btnOpenSuoFolder);
            this.Controls.Add(this.btnIncludeInGitignoreNow);
            this.Controls.Add(this.chkAutoIncludeInGitignore);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "StorageOptionsControl";
            this.Size = new System.Drawing.Size(503, 256);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlInfoMessage.ResumeLayout(false);
            this.pnlInfoMessage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbInOwnFile;
        private System.Windows.Forms.RadioButton rbInSuo;
        private System.Windows.Forms.CheckBox chkAutoIncludeInGitignore;
        private System.Windows.Forms.Button btnIncludeInGitignoreNow;
        private System.Windows.Forms.Button btnOpenSuoFolder;
        private System.Windows.Forms.Label lblIfGitignoreExists;
        private System.Windows.Forms.Panel pnlInfoMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblInfoMessage;
    }
}
