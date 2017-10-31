namespace Konamiman.SuperBookmarks.Options
{
    partial class DebugOptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugOptionsControl));
            this.chkShowErrorsInMessageBox = new System.Windows.Forms.CheckBox();
            this.pnlRequiresRestaring = new System.Windows.Forms.Panel();
            this.lblInfoMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnOpenActivityLog = new System.Windows.Forms.Button();
            this.btnCopyActivityLogPath = new System.Windows.Forms.Button();
            this.btnThrowTestException = new System.Windows.Forms.Button();
            this.chkWriteErrorsToOutput = new System.Windows.Forms.CheckBox();
            this.chkLogErrorsInActivityLog = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlRequiresRestaring.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // chkShowErrorsInMessageBox
            // 
            this.chkShowErrorsInMessageBox.AutoSize = true;
            this.chkShowErrorsInMessageBox.Location = new System.Drawing.Point(61, 87);
            this.chkShowErrorsInMessageBox.Margin = new System.Windows.Forms.Padding(5);
            this.chkShowErrorsInMessageBox.Name = "chkShowErrorsInMessageBox";
            this.chkShowErrorsInMessageBox.Size = new System.Drawing.Size(474, 36);
            this.chkShowErrorsInMessageBox.TabIndex = 0;
            this.chkShowErrorsInMessageBox.Text = "Show summary in a message box";
            this.chkShowErrorsInMessageBox.UseVisualStyleBackColor = true;
            // 
            // pnlRequiresRestaring
            // 
            this.pnlRequiresRestaring.Controls.Add(this.lblInfoMessage);
            this.pnlRequiresRestaring.Controls.Add(this.pictureBox1);
            this.pnlRequiresRestaring.Location = new System.Drawing.Point(25, 291);
            this.pnlRequiresRestaring.Margin = new System.Windows.Forms.Padding(5);
            this.pnlRequiresRestaring.Name = "pnlRequiresRestaring";
            this.pnlRequiresRestaring.Size = new System.Drawing.Size(736, 74);
            this.pnlRequiresRestaring.TabIndex = 5;
            // 
            // lblInfoMessage
            // 
            this.lblInfoMessage.AutoSize = true;
            this.lblInfoMessage.Location = new System.Drawing.Point(39, -6);
            this.lblInfoMessage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblInfoMessage.Name = "lblInfoMessage";
            this.lblInfoMessage.Size = new System.Drawing.Size(622, 64);
            this.lblInfoMessage.TabIndex = 0;
            this.lblInfoMessage.Text = "Errors will be recorded in the Activity Log only\r\nif Visual Studio is started wit" +
    "h the /log parameter\r\n";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(52, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnOpenActivityLog
            // 
            this.btnOpenActivityLog.Location = new System.Drawing.Point(25, 387);
            this.btnOpenActivityLog.Margin = new System.Windows.Forms.Padding(5);
            this.btnOpenActivityLog.Name = "btnOpenActivityLog";
            this.btnOpenActivityLog.Size = new System.Drawing.Size(468, 54);
            this.btnOpenActivityLog.TabIndex = 6;
            this.btnOpenActivityLog.Text = "Open the Activity Log file";
            this.btnOpenActivityLog.UseVisualStyleBackColor = true;
            this.btnOpenActivityLog.Click += new System.EventHandler(this.btnOpenActivityLog_Click);
            // 
            // btnCopyActivityLogPath
            // 
            this.btnCopyActivityLogPath.Location = new System.Drawing.Point(25, 460);
            this.btnCopyActivityLogPath.Margin = new System.Windows.Forms.Padding(5);
            this.btnCopyActivityLogPath.Name = "btnCopyActivityLogPath";
            this.btnCopyActivityLogPath.Size = new System.Drawing.Size(468, 54);
            this.btnCopyActivityLogPath.TabIndex = 7;
            this.btnCopyActivityLogPath.Text = "Copy the Activity Log file path\r\n";
            this.btnCopyActivityLogPath.UseVisualStyleBackColor = true;
            this.btnCopyActivityLogPath.Click += new System.EventHandler(this.btnCopyActivityLogPath_Click);
            // 
            // btnThrowTestException
            // 
            this.btnThrowTestException.Location = new System.Drawing.Point(25, 563);
            this.btnThrowTestException.Margin = new System.Windows.Forms.Padding(5);
            this.btnThrowTestException.Name = "btnThrowTestException";
            this.btnThrowTestException.Size = new System.Drawing.Size(468, 54);
            this.btnThrowTestException.TabIndex = 8;
            this.btnThrowTestException.Text = "Throw test exception\r\n";
            this.btnThrowTestException.UseVisualStyleBackColor = true;
            this.btnThrowTestException.Click += new System.EventHandler(this.btnThrowTestException_Click);
            // 
            // chkWriteErrorsToOutput
            // 
            this.chkWriteErrorsToOutput.AutoSize = true;
            this.chkWriteErrorsToOutput.Location = new System.Drawing.Point(61, 143);
            this.chkWriteErrorsToOutput.Margin = new System.Windows.Forms.Padding(5);
            this.chkWriteErrorsToOutput.Name = "chkWriteErrorsToOutput";
            this.chkWriteErrorsToOutput.Size = new System.Drawing.Size(662, 36);
            this.chkWriteErrorsToOutput.TabIndex = 9;
            this.chkWriteErrorsToOutput.Text = "Write details in the Output window (Debug pane)";
            this.chkWriteErrorsToOutput.UseVisualStyleBackColor = true;
            // 
            // chkLogErrorsInActivityLog
            // 
            this.chkLogErrorsInActivityLog.AutoSize = true;
            this.chkLogErrorsInActivityLog.Location = new System.Drawing.Point(61, 199);
            this.chkLogErrorsInActivityLog.Margin = new System.Windows.Forms.Padding(5);
            this.chkLogErrorsInActivityLog.Name = "chkLogErrorsInActivityLog";
            this.chkLogErrorsInActivityLog.Size = new System.Drawing.Size(440, 36);
            this.chkLogErrorsInActivityLog.TabIndex = 10;
            this.chkLogErrorsInActivityLog.Text = "Write details in the Activity Log";
            this.chkLogErrorsInActivityLog.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(423, 32);
            this.label1.TabIndex = 11;
            this.label1.Text = "On error or unhandled exception";
            // 
            // DebugOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkLogErrorsInActivityLog);
            this.Controls.Add(this.chkWriteErrorsToOutput);
            this.Controls.Add(this.btnThrowTestException);
            this.Controls.Add(this.btnCopyActivityLogPath);
            this.Controls.Add(this.btnOpenActivityLog);
            this.Controls.Add(this.pnlRequiresRestaring);
            this.Controls.Add(this.chkShowErrorsInMessageBox);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "DebugOptionsControl";
            this.Size = new System.Drawing.Size(889, 731);
            this.pnlRequiresRestaring.ResumeLayout(false);
            this.pnlRequiresRestaring.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowErrorsInMessageBox;
        private System.Windows.Forms.Panel pnlRequiresRestaring;
        private System.Windows.Forms.Label lblInfoMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnOpenActivityLog;
        private System.Windows.Forms.Button btnCopyActivityLogPath;
        private System.Windows.Forms.Button btnThrowTestException;
        private System.Windows.Forms.CheckBox chkWriteErrorsToOutput;
        private System.Windows.Forms.CheckBox chkLogErrorsInActivityLog;
        private System.Windows.Forms.Label label1;
    }
}
