namespace Konamiman.SuperBookmarks
{
    partial class GeneralOptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralOptionsControl));
            this.chkDeletingLineDeletesBookmark = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbMenuDontShow = new System.Windows.Forms.RadioButton();
            this.rbMenuShowSuperBookmarks = new System.Windows.Forms.RadioButton();
            this.rbMenuShowBookmarks = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlRequiresRestaring = new System.Windows.Forms.Panel();
            this.lblInfoMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.btnResetColor = new System.Windows.Forms.Button();
            this.lblGlyphColor = new System.Windows.Forms.Label();
            this.pnlChooseColor = new System.Windows.Forms.Panel();
            this.chkNavInFolderIncludesSubfolders = new System.Windows.Forms.CheckBox();
            this.chkDelAllInFolderIncludesSubfolder = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbImportReplaces = new System.Windows.Forms.RadioButton();
            this.rbImportMerges = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.pnlRequiresRestaring.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDeletingLineDeletesBookmark
            // 
            this.chkDeletingLineDeletesBookmark.AutoSize = true;
            this.chkDeletingLineDeletesBookmark.Location = new System.Drawing.Point(14, 19);
            this.chkDeletingLineDeletesBookmark.Name = "chkDeletingLineDeletesBookmark";
            this.chkDeletingLineDeletesBookmark.Size = new System.Drawing.Size(320, 24);
            this.chkDeletingLineDeletesBookmark.TabIndex = 0;
            this.chkDeletingLineDeletesBookmark.Text = "Deleting a line deletes the line bookmark";
            this.chkDeletingLineDeletesBookmark.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbMenuDontShow);
            this.panel1.Controls.Add(this.rbMenuShowSuperBookmarks);
            this.panel1.Controls.Add(this.rbMenuShowBookmarks);
            this.panel1.Location = new System.Drawing.Point(14, 140);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 156);
            this.panel1.TabIndex = 2;
            // 
            // rbMenuDontShow
            // 
            this.rbMenuDontShow.AutoSize = true;
            this.rbMenuDontShow.Location = new System.Drawing.Point(2, 56);
            this.rbMenuDontShow.Margin = new System.Windows.Forms.Padding(2);
            this.rbMenuDontShow.Name = "rbMenuDontShow";
            this.rbMenuDontShow.Size = new System.Drawing.Size(113, 24);
            this.rbMenuDontShow.TabIndex = 2;
            this.rbMenuDontShow.TabStop = true;
            this.rbMenuDontShow.Text = "Don\'t show";
            this.rbMenuDontShow.UseVisualStyleBackColor = true;
            // 
            // rbMenuShowSuperBookmarks
            // 
            this.rbMenuShowSuperBookmarks.AutoSize = true;
            this.rbMenuShowSuperBookmarks.Location = new System.Drawing.Point(2, 3);
            this.rbMenuShowSuperBookmarks.Margin = new System.Windows.Forms.Padding(2);
            this.rbMenuShowSuperBookmarks.Name = "rbMenuShowSuperBookmarks";
            this.rbMenuShowSuperBookmarks.Size = new System.Drawing.Size(278, 24);
            this.rbMenuShowSuperBookmarks.TabIndex = 0;
            this.rbMenuShowSuperBookmarks.TabStop = true;
            this.rbMenuShowSuperBookmarks.Text = "Show, with title \"SuperBookmarks\"";
            this.rbMenuShowSuperBookmarks.UseVisualStyleBackColor = true;
            // 
            // rbMenuShowBookmarks
            // 
            this.rbMenuShowBookmarks.AutoSize = true;
            this.rbMenuShowBookmarks.Location = new System.Drawing.Point(2, 29);
            this.rbMenuShowBookmarks.Margin = new System.Windows.Forms.Padding(2);
            this.rbMenuShowBookmarks.Name = "rbMenuShowBookmarks";
            this.rbMenuShowBookmarks.Size = new System.Drawing.Size(235, 24);
            this.rbMenuShowBookmarks.TabIndex = 1;
            this.rbMenuShowBookmarks.TabStop = true;
            this.rbMenuShowBookmarks.Text = "Show, with title \"Bookmarks\"";
            this.rbMenuShowBookmarks.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 119);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "SuperBookmarks menu in Menu Bar";
            // 
            // pnlRequiresRestaring
            // 
            this.pnlRequiresRestaring.Controls.Add(this.lblInfoMessage);
            this.pnlRequiresRestaring.Controls.Add(this.pictureBox1);
            this.pnlRequiresRestaring.Location = new System.Drawing.Point(16, 229);
            this.pnlRequiresRestaring.Name = "pnlRequiresRestaring";
            this.pnlRequiresRestaring.Size = new System.Drawing.Size(551, 61);
            this.pnlRequiresRestaring.TabIndex = 4;
            // 
            // lblInfoMessage
            // 
            this.lblInfoMessage.AutoSize = true;
            this.lblInfoMessage.Location = new System.Drawing.Point(22, -4);
            this.lblInfoMessage.Name = "lblInfoMessage";
            this.lblInfoMessage.Size = new System.Drawing.Size(464, 60);
            this.lblInfoMessage.TabIndex = 0;
            this.lblInfoMessage.Text = "You can still use the toolbars (to show: Tools - Customize)\r\nand open the Options" +
    " dialog (Tools - Options - SuperBookmarks)\r\nwhen the menu is not visible in the " +
    "Menu Bar";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(29, 31);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.FullOpen = true;
            this.colorDialog.SolidColorOnly = true;
            // 
            // btnResetColor
            // 
            this.btnResetColor.Location = new System.Drawing.Point(260, 400);
            this.btnResetColor.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetColor.Name = "btnResetColor";
            this.btnResetColor.Size = new System.Drawing.Size(94, 32);
            this.btnResetColor.TabIndex = 7;
            this.btnResetColor.Text = "Reset";
            this.btnResetColor.UseVisualStyleBackColor = true;
            this.btnResetColor.Click += new System.EventHandler(this.btnResetColor_Click);
            // 
            // lblGlyphColor
            // 
            this.lblGlyphColor.AutoSize = true;
            this.lblGlyphColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblGlyphColor.Location = new System.Drawing.Point(13, 405);
            this.lblGlyphColor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGlyphColor.Name = "lblGlyphColor";
            this.lblGlyphColor.Size = new System.Drawing.Size(164, 20);
            this.lblGlyphColor.TabIndex = 5;
            this.lblGlyphColor.Text = "Bookmark glyph color:";
            this.lblGlyphColor.Click += new System.EventHandler(this.lblGlyphColor_Click);
            // 
            // pnlChooseColor
            // 
            this.pnlChooseColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlChooseColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlChooseColor.Location = new System.Drawing.Point(186, 400);
            this.pnlChooseColor.Margin = new System.Windows.Forms.Padding(2);
            this.pnlChooseColor.Name = "pnlChooseColor";
            this.pnlChooseColor.Size = new System.Drawing.Size(52, 32);
            this.pnlChooseColor.TabIndex = 6;
            this.pnlChooseColor.Click += new System.EventHandler(this.pnlChooseColor_Click);
            // 
            // chkNavInFolderIncludesSubfolders
            // 
            this.chkNavInFolderIncludesSubfolders.AutoSize = true;
            this.chkNavInFolderIncludesSubfolders.Location = new System.Drawing.Point(14, 49);
            this.chkNavInFolderIncludesSubfolders.Name = "chkNavInFolderIncludesSubfolders";
            this.chkNavInFolderIncludesSubfolders.Size = new System.Drawing.Size(363, 24);
            this.chkNavInFolderIncludesSubfolders.TabIndex = 1;
            this.chkNavInFolderIncludesSubfolders.Text = "\"Go to Prev/Next in Folder\" includes subfolders";
            this.chkNavInFolderIncludesSubfolders.UseVisualStyleBackColor = true;
            // 
            // chkDelAllInFolderIncludesSubfolder
            // 
            this.chkDelAllInFolderIncludesSubfolder.AutoSize = true;
            this.chkDelAllInFolderIncludesSubfolder.Location = new System.Drawing.Point(14, 79);
            this.chkDelAllInFolderIncludesSubfolder.Name = "chkDelAllInFolderIncludesSubfolder";
            this.chkDelAllInFolderIncludesSubfolder.Size = new System.Drawing.Size(320, 24);
            this.chkDelAllInFolderIncludesSubfolder.TabIndex = 2;
            this.chkDelAllInFolderIncludesSubfolder.Text = "\"Delete All in Folder\" includes subfolders";
            this.chkDelAllInFolderIncludesSubfolder.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 303);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "\"Import Bookmarks\" behavior";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbImportReplaces);
            this.panel2.Controls.Add(this.rbImportMerges);
            this.panel2.Location = new System.Drawing.Point(12, 324);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(388, 65);
            this.panel2.TabIndex = 8;
            // 
            // rbImportReplaces
            // 
            this.rbImportReplaces.AutoSize = true;
            this.rbImportReplaces.Location = new System.Drawing.Point(2, 3);
            this.rbImportReplaces.Margin = new System.Windows.Forms.Padding(2);
            this.rbImportReplaces.Name = "rbImportReplaces";
            this.rbImportReplaces.Size = new System.Drawing.Size(232, 24);
            this.rbImportReplaces.TabIndex = 0;
            this.rbImportReplaces.TabStop = true;
            this.rbImportReplaces.Text = "Replace existing bookmarks";
            this.rbImportReplaces.UseVisualStyleBackColor = true;
            // 
            // rbImportMerges
            // 
            this.rbImportMerges.AutoSize = true;
            this.rbImportMerges.Location = new System.Drawing.Point(2, 29);
            this.rbImportMerges.Margin = new System.Windows.Forms.Padding(2);
            this.rbImportMerges.Name = "rbImportMerges";
            this.rbImportMerges.Size = new System.Drawing.Size(250, 24);
            this.rbImportMerges.TabIndex = 1;
            this.rbImportMerges.TabStop = true;
            this.rbImportMerges.Text = "Merge with existing bookmarks";
            this.rbImportMerges.UseVisualStyleBackColor = true;
            // 
            // GeneralOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.chkDelAllInFolderIncludesSubfolder);
            this.Controls.Add(this.chkNavInFolderIncludesSubfolders);
            this.Controls.Add(this.btnResetColor);
            this.Controls.Add(this.pnlChooseColor);
            this.Controls.Add(this.lblGlyphColor);
            this.Controls.Add(this.pnlRequiresRestaring);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkDeletingLineDeletesBookmark);
            this.Name = "GeneralOptionsControl";
            this.Size = new System.Drawing.Size(602, 520);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlRequiresRestaring.ResumeLayout(false);
            this.pnlRequiresRestaring.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDeletingLineDeletesBookmark;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbMenuShowSuperBookmarks;
        private System.Windows.Forms.RadioButton rbMenuShowBookmarks;
        private System.Windows.Forms.Panel pnlRequiresRestaring;
        private System.Windows.Forms.Label lblInfoMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button btnResetColor;
        private System.Windows.Forms.Label lblGlyphColor;
        private System.Windows.Forms.Panel pnlChooseColor;
        private System.Windows.Forms.CheckBox chkNavInFolderIncludesSubfolders;
        private System.Windows.Forms.CheckBox chkDelAllInFolderIncludesSubfolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbImportReplaces;
        private System.Windows.Forms.RadioButton rbImportMerges;
        private System.Windows.Forms.RadioButton rbMenuDontShow;
    }
}
