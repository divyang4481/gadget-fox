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
    partial class RepositoryBrowserForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeListViewItemCollection.TreeListViewItemCollectionComparer treeListViewItemCollectionComparer1 = new System.Windows.Forms.TreeListViewItemCollection.TreeListViewItemCollectionComparer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRevision = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.repositoryTreeView = new System.Windows.Forms.TreeListView();
            this.columnHdrFile = new System.Windows.Forms.ColumnHeader();
            this.columnHdrExtension = new System.Windows.Forms.ColumnHeader();
            this.columnHdrRevision = new System.Windows.Forms.ColumnHeader();
            this.columnHdrAuthor = new System.Windows.Forms.ColumnHeader();
            this.columnHdrSize = new System.Windows.Forms.ColumnHeader();
            this.columnHdrDate = new System.Windows.Forms.ColumnHeader();
            this.columnHdrLock = new System.Windows.Forms.ColumnHeader();
            this.imageLst = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL:";
            // 
            // txtURL
            // 
            this.txtURL.AcceptsReturn = true;
            this.txtURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURL.Location = new System.Drawing.Point(51, 6);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(476, 21);
            this.txtURL.TabIndex = 1;
            this.txtURL.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtURL_KeyUp);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(566, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Revision:";
            // 
            // btnRevision
            // 
            this.btnRevision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevision.Location = new System.Drawing.Point(619, 4);
            this.btnRevision.Name = "btnRevision";
            this.btnRevision.Size = new System.Drawing.Size(61, 23);
            this.btnRevision.TabIndex = 4;
            this.btnRevision.Text = "HEAD";
            this.btnRevision.UseVisualStyleBackColor = true;
            this.btnRevision.Click += new System.EventHandler(this.btnRevision_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(605, 371);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 6;
            this.btnExit.Text = "OK";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(530, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 21);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // repositoryTreeView
            // 
            this.repositoryTreeView.AllowColumnReorder = true;
            this.repositoryTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.repositoryTreeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHdrFile,
            this.columnHdrExtension,
            this.columnHdrRevision,
            this.columnHdrAuthor,
            this.columnHdrSize,
            this.columnHdrDate,
            this.columnHdrLock});
            treeListViewItemCollectionComparer1.Column = 0;
            treeListViewItemCollectionComparer1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.repositoryTreeView.Comparer = treeListViewItemCollectionComparer1;
            this.repositoryTreeView.Location = new System.Drawing.Point(12, 33);
            this.repositoryTreeView.Name = "repositoryTreeView";
            this.repositoryTreeView.Size = new System.Drawing.Size(668, 332);
            this.repositoryTreeView.SmallImageList = this.imageLst;
            this.repositoryTreeView.Sorting = System.Windows.Forms.SortOrder.None;
            this.repositoryTreeView.TabIndex = 5;
            this.repositoryTreeView.UseCompatibleStateImageBehavior = false;
            this.repositoryTreeView.UseXPHighlightStyle = false;
            this.repositoryTreeView.BeforeExpand += new System.Windows.Forms.TreeListViewCancelEventHandler(this.repositoryTreeView_BeforeExpand);
            // 
            // columnHdrFile
            // 
            this.columnHdrFile.Text = "File";
            this.columnHdrFile.Width = 250;
            // 
            // columnHdrExtension
            // 
            this.columnHdrExtension.Text = "Extension";
            // 
            // columnHdrRevision
            // 
            this.columnHdrRevision.Text = "Revision";
            this.columnHdrRevision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHdrAuthor
            // 
            this.columnHdrAuthor.Text = "Author";
            // 
            // columnHdrSize
            // 
            this.columnHdrSize.Text = "Size";
            // 
            // columnHdrDate
            // 
            this.columnHdrDate.Text = "Date";
            this.columnHdrDate.Width = 114;
            // 
            // columnHdrLock
            // 
            this.columnHdrLock.Text = "Lock";
            // 
            // imageLst
            // 
            this.imageLst.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageLst.ImageSize = new System.Drawing.Size(16, 16);
            this.imageLst.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // folderBrowserDlg
            // 
            this.folderBrowserDlg.Description = "Select the root of an SVN FSFS repository.";
            this.folderBrowserDlg.ShowNewFolderButton = false;
            // 
            // RepositoryBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(692, 400);
            this.Controls.Add(this.repositoryTreeView);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRevision);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RepositoryBrowserForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "DotSVN Repository Browser";
            this.Load += new System.EventHandler(this.RepositoryBrowserForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRevision;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TreeListView repositoryTreeView;
        private System.Windows.Forms.ColumnHeader columnHdrFile;
        private System.Windows.Forms.ColumnHeader columnHdrExtension;
        private System.Windows.Forms.ColumnHeader columnHdrRevision;
        private System.Windows.Forms.ColumnHeader columnHdrAuthor;
        private System.Windows.Forms.ColumnHeader columnHdrSize;
        private System.Windows.Forms.ColumnHeader columnHdrDate;
        private System.Windows.Forms.ColumnHeader columnHdrLock;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDlg;
        private System.Windows.Forms.ImageList imageLst;
    }
}

