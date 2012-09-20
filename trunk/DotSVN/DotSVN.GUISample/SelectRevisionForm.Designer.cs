#region Copyright
/*
* ====================================================================
* Copyright (c) 2007 www.dotsvn.net.  All rights reserved.
*
* This software is licensed as described in the file LICENSE, which
* you should have received as part of this distribution.  
* ====================================================================
*/
#endregion //Copyright

namespace DotSVN.GUISample
{
    partial class SelectRevisionForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbRevision = new System.Windows.Forms.TextBox();
            this.rbRevision = new System.Windows.Forms.RadioButton();
            this.rbHeadRevision = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbRevision);
            this.groupBox1.Controls.Add(this.rbRevision);
            this.groupBox1.Controls.Add(this.rbHeadRevision);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 83);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Revision";
            // 
            // tbRevision
            // 
            this.tbRevision.Location = new System.Drawing.Point(121, 43);
            this.tbRevision.Name = "tbRevision";
            this.tbRevision.Size = new System.Drawing.Size(144, 20);
            this.tbRevision.TabIndex = 2;
            // 
            // rbRevision
            // 
            this.rbRevision.AutoSize = true;
            this.rbRevision.Location = new System.Drawing.Point(18, 46);
            this.rbRevision.Name = "rbRevision";
            this.rbRevision.Size = new System.Drawing.Size(66, 17);
            this.rbRevision.TabIndex = 1;
            this.rbRevision.Text = "Revision";
            this.rbRevision.UseVisualStyleBackColor = true;
            // 
            // rbHeadRevision
            // 
            this.rbHeadRevision.AutoSize = true;
            this.rbHeadRevision.Checked = true;
            this.rbHeadRevision.Location = new System.Drawing.Point(18, 19);
            this.rbHeadRevision.Name = "rbHeadRevision";
            this.rbHeadRevision.Size = new System.Drawing.Size(94, 17);
            this.rbHeadRevision.TabIndex = 0;
            this.rbHeadRevision.TabStop = true;
            this.rbHeadRevision.Text = "&HEAD revision";
            this.rbHeadRevision.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(133, 101);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(217, 101);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SelectRevisionForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(302, 139);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectRevisionForm";
            this.Text = "Select Revision";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbRevision;
        private System.Windows.Forms.RadioButton rbRevision;
        private System.Windows.Forms.RadioButton rbHeadRevision;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}