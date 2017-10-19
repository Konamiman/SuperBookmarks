namespace Konamiman.SuperBookmarks
{
    partial class OptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsControl));
            this.chkDeletingLineDeletesBookmark = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbInEdit = new System.Windows.Forms.RadioButton();
            this.rbInTopLevel = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlRequiresRestaring = new System.Windows.Forms.Panel();
            this.lblInfoMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.btnResetColor = new System.Windows.Forms.Button();
            this.lblGlyphColor = new System.Windows.Forms.Label();
            this.pnlChooseColor = new System.Windows.Forms.Panel();
            this.chkNavInFolderIncludesSubfolders = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.pnlRequiresRestaring.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.panel1.Controls.Add(this.rbInEdit);
            this.panel1.Controls.Add(this.rbInTopLevel);
            this.panel1.Location = new System.Drawing.Point(14, 122);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 106);
            this.panel1.TabIndex = 2;
            // 
            // rbInEdit
            // 
            this.rbInEdit.AutoSize = true;
            this.rbInEdit.Location = new System.Drawing.Point(2, 3);
            this.rbInEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbInEdit.Name = "rbInEdit";
            this.rbInEdit.Size = new System.Drawing.Size(385, 24);
            this.rbInEdit.TabIndex = 0;
            this.rbInEdit.TabStop = true;
            this.rbInEdit.Text = "A \"SuperBookmarks\" submenu in the \"Edit\" menu";
            this.rbInEdit.UseVisualStyleBackColor = true;
            // 
            // rbInTopLevel
            // 
            this.rbInTopLevel.AutoSize = true;
            this.rbInTopLevel.Location = new System.Drawing.Point(2, 29);
            this.rbInTopLevel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbInTopLevel.Name = "rbInTopLevel";
            this.rbInTopLevel.Size = new System.Drawing.Size(290, 24);
            this.rbInTopLevel.TabIndex = 1;
            this.rbInTopLevel.TabStop = true;
            this.rbInTopLevel.Text = "A \"SuperBookmarks\" top level menu";
            this.rbInTopLevel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 96);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Show commands in";
            // 
            // pnlRequiresRestaring
            // 
            this.pnlRequiresRestaring.Controls.Add(this.lblInfoMessage);
            this.pnlRequiresRestaring.Controls.Add(this.pictureBox1);
            this.pnlRequiresRestaring.Location = new System.Drawing.Point(16, 177);
            this.pnlRequiresRestaring.Name = "pnlRequiresRestaring";
            this.pnlRequiresRestaring.Size = new System.Drawing.Size(384, 38);
            this.pnlRequiresRestaring.TabIndex = 3;
            this.pnlRequiresRestaring.Visible = false;
            // 
            // lblInfoMessage
            // 
            this.lblInfoMessage.AutoSize = true;
            this.lblInfoMessage.Location = new System.Drawing.Point(22, 3);
            this.lblInfoMessage.Name = "lblInfoMessage";
            this.lblInfoMessage.Size = new System.Drawing.Size(339, 20);
            this.lblInfoMessage.TabIndex = 0;
            this.lblInfoMessage.Text = "Requires restarting Visual Studio to take effect";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 7);
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
            this.btnResetColor.Location = new System.Drawing.Point(259, 212);
            this.btnResetColor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnResetColor.Name = "btnResetColor";
            this.btnResetColor.Size = new System.Drawing.Size(94, 32);
            this.btnResetColor.TabIndex = 6;
            this.btnResetColor.Text = "Reset";
            this.btnResetColor.UseVisualStyleBackColor = true;
            this.btnResetColor.Click += new System.EventHandler(this.btnResetColor_Click);
            // 
            // lblGlyphColor
            // 
            this.lblGlyphColor.AutoSize = true;
            this.lblGlyphColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblGlyphColor.Location = new System.Drawing.Point(13, 217);
            this.lblGlyphColor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGlyphColor.Name = "lblGlyphColor";
            this.lblGlyphColor.Size = new System.Drawing.Size(164, 20);
            this.lblGlyphColor.TabIndex = 4;
            this.lblGlyphColor.Text = "Bookmark glyph color:";
            this.lblGlyphColor.Click += new System.EventHandler(this.lblGlyphColor_Click);
            // 
            // pnlChooseColor
            // 
            this.pnlChooseColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlChooseColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlChooseColor.Location = new System.Drawing.Point(186, 212);
            this.pnlChooseColor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlChooseColor.Name = "pnlChooseColor";
            this.pnlChooseColor.Size = new System.Drawing.Size(52, 32);
            this.pnlChooseColor.TabIndex = 5;
            this.pnlChooseColor.Click += new System.EventHandler(this.pnlChooseColor_Click);
            // 
            // chkNavInFolderIncludesSubfolders
            // 
            this.chkNavInFolderIncludesSubfolders.AutoSize = true;
            this.chkNavInFolderIncludesSubfolders.Location = new System.Drawing.Point(14, 49);
            this.chkNavInFolderIncludesSubfolders.Name = "chkNavInFolderIncludesSubfolders";
            this.chkNavInFolderIncludesSubfolders.Size = new System.Drawing.Size(360, 24);
            this.chkNavInFolderIncludesSubfolders.TabIndex = 1;
            this.chkNavInFolderIncludesSubfolders.Text = "\"Go to prev/next in Folder\" includes subfolders";
            this.chkNavInFolderIncludesSubfolders.UseVisualStyleBackColor = true;
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkNavInFolderIncludesSubfolders);
            this.Controls.Add(this.btnResetColor);
            this.Controls.Add(this.pnlChooseColor);
            this.Controls.Add(this.lblGlyphColor);
            this.Controls.Add(this.pnlRequiresRestaring);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkDeletingLineDeletesBookmark);
            this.Name = "OptionsControl";
            this.Size = new System.Drawing.Size(408, 352);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlRequiresRestaring.ResumeLayout(false);
            this.pnlRequiresRestaring.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDeletingLineDeletesBookmark;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbInEdit;
        private System.Windows.Forms.RadioButton rbInTopLevel;
        private System.Windows.Forms.Panel pnlRequiresRestaring;
        private System.Windows.Forms.Label lblInfoMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button btnResetColor;
        private System.Windows.Forms.Label lblGlyphColor;
        private System.Windows.Forms.Panel pnlChooseColor;
        private System.Windows.Forms.CheckBox chkNavInFolderIncludesSubfolders;
    }
}
