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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.APIs;
using System.Text;
using System.Windows.Forms;
using DotSVN.Common;
using DotSVN.Common.Entities;
using DotSVN.Common.Util;
using DotSVN.Server.RepositoryAccess;

namespace DotSVN.GUISample
{
    public partial class RepositoryBrowserForm : Form
    {
        #region Private Fields

        
        private static object dummyObject = new object();
        private static TreeListViewItem dummyItem = new TreeListViewItem("");

        private const string ICON_UNKNOWN = "UNKNOWN";
        private const string FOLDER_CLOSE = "FOLDER_CLOSE";
        private const string FOLDER_OPEN = "FOLDER_OPEN";

        private ISVNRepository repository = null;
        private long currentRevision = -1;

        #endregion

        #region DLL Import

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize,
                                                    [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer,
                                                    int bufferSize);

        #endregion


        public RepositoryBrowserForm()
        {
            InitializeComponent();
            dummyItem.Tag = dummyObject;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDlg.ShowDialog() == DialogResult.OK)
            {
                string newReposPath = "file://" + folderBrowserDlg.SelectedPath;
                TryOpenRepository(newReposPath);
            }
        }

        private void TryOpenRepository(string rootPath)
        {
            if (TestRepositoryPath(rootPath))
            {
                ResetRepository();

                repository = SVNRepositoryFactory.Create(new SVNURL(rootPath));
                repository.OpenRepository();
                BrowseRepository(currentRevision);
            }
            else
            {
                MessageBox.Show("Selected path is not a valid SVN FSFS repositiry root", "Invalid path",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TestRepositoryPath(string rootPath)
        {
            try
            {
                repository = SVNRepositoryFactory.Create(new SVNURL(rootPath));
                repository.TestConnection();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void ResetRepository()
        {
            if (repository != null)
            {
                repository.CloseRepository();
                repository = null;
                currentRevision = -1;
                repositoryTreeView.Items.Clear();
            }
        }

        private void BrowseRepository(long revision)
        {
            try
            {
                ICollection<SVNDirEntry> dirEntries = repository.GetDir(string.Empty, revision, null);

                SortEntries(dirEntries);

                txtURL.Text = repository.GetRepositoryRoot(true).ToString();
                TreeListViewItem root = repositoryTreeView.Items.Add(txtURL.Text, 0);

                foreach (SVNDirEntry dirEntry in dirEntries)
                {
                    AddItem(dirEntry, root);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error orrcured while connecting to the repository\n" + ex, "Exception",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SortEntries(ICollection<SVNDirEntry> dirEntries)
        {
            List<SVNDirEntry> list = dirEntries as List<SVNDirEntry>;
            if (list == null)
                return;

            list.Sort(CompareBySVNKind);
            int index = 0;
            for (; index < list.Count; index++)
            {
                if (list[index].Kind != SVNNodeKind.dir)
                    break;
            }
            CompareAlphabetically comparer = new CompareAlphabetically();

            list.Sort(0, index, comparer);
            list.Sort(index, list.Count - index, comparer);
        }

        private static int CompareBySVNKind(SVNDirEntry x, SVNDirEntry y)
        {
            if (x == null)
            {
                if (y == null)          // If x is null and y is null, they're equal. 
                    return 0;
                else                    // If x is null and y is not null, y is greater. 
                    return -1;
            }
            else
            {
                // If x is not null and y is null, x is greater.
                if (y == null)
                    return 1;
                else
                {
                    if (x.Kind == y.Kind)
                        return 0;
                    else if (x.Kind == SVNNodeKind.file)
                        return 1;
                    else
                        return -1;
                }
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            if (repository != null)
            {
                SelectRevisionForm selectRevForm = new SelectRevisionForm();
                if (selectRevForm.ShowDialog() == DialogResult.OK)
                {
                    repositoryTreeView.Items.Clear();
                    currentRevision = selectRevForm.SelectedRevision;
                    BrowseRepository(currentRevision);
                }
            }
            else
            {
                MessageBox.Show("Please open a repository first.", "Choose a repository", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private void repositoryTreeView_BeforeExpand(object sender, TreeListViewCancelEventArgs e)
        {
            TreeListViewItem selectedItem = e.Item;
            if (selectedItem.Tag != null)
            {
                if (selectedItem.Tag is SVNDirEntry)
                {
                    if (selectedItem.Items.Count == 1)
                    {
                        if (selectedItem.Items[0].Tag == dummyObject)
                        {
                            selectedItem.Items.RemoveAt(0);

                            // Check if we have some children to add
                            // Done only once
                            SVNDirEntry entry = selectedItem.Tag as SVNDirEntry;
                            if (entry != null && entry.Kind == SVNNodeKind.dir)
                            {
                                string newPath = selectedItem.FullPath.Substring(repository.Location.ToString().Length);
                                newPath = newPath.Replace('\\', '/');
                                ICollection<SVNDirEntry> dirEntries = repository.GetDir(newPath, -1, null);
                                SortEntries(dirEntries);
                                foreach (SVNDirEntry dirEntry in dirEntries)
                                {
                                    AddItem(dirEntry, selectedItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RepositoryBrowserForm_Load(object sender, EventArgs e)
        {
            Icon newIcon = APIsShell.GetFolderIcon(string.Empty, true, false);
            imageLst.Images.Add(FOLDER_CLOSE, newIcon);
            DestroyIcon(newIcon.Handle);

            newIcon = APIsShell.GetFolderIcon(string.Empty, true, true);
            imageLst.Images.Add(FOLDER_OPEN, newIcon);

            Icon = (Icon)newIcon.Clone();
            DestroyIcon(newIcon.Handle);

            newIcon = APIsShell.GetFileIcon(ICON_UNKNOWN, true);
            imageLst.Images.Add(ICON_UNKNOWN, newIcon);
            DestroyIcon(newIcon.Handle);
        }

        private void AddItem(SVNDirEntry entry, TreeListViewItem parent)
        {
            string fileExtension = Path.GetExtension(entry.Name);

            int imageIndex;
            if (entry.Kind == SVNNodeKind.dir)
            {
                imageIndex = 0;
            }
            else
            {
                if(string.IsNullOrEmpty(fileExtension))
                {
                    imageIndex = imageLst.Images.IndexOfKey(ICON_UNKNOWN);
                }
                else if (imageLst.Images.ContainsKey(fileExtension))
                    imageIndex = imageLst.Images.IndexOfKey(fileExtension);
                else
                {
                    Icon newIcon = APIsShell.GetFileIcon(fileExtension, true);
                    imageLst.Images.Add(fileExtension, newIcon);

                    DestroyIcon(newIcon.Handle);
                    imageIndex = imageLst.Images.IndexOfKey(fileExtension);
                }
            }
            TreeListViewItem newItem = new TreeListViewItem(entry.Name, imageIndex);
            newItem.Tag = entry;


            if (entry.Kind == SVNNodeKind.dir)
            {
                newItem.Items.Add(dummyItem);
            }

            newItem.SubItems.Add(fileExtension.TrimStart('.')); // Extension
            newItem.SubItems.Add(entry.Revision.ToString());    // Revision
            newItem.SubItems.Add(entry.Author);                 // Author
            newItem.SubItems.Add(FormatSize(entry.Size));       // Size 
            newItem.SubItems.Add(FormateDate(entry.Date));      // Commit Date

            if (parent != null)
                parent.Items.Add(newItem);
            else
                repositoryTreeView.Items.Add(newItem);
        }

        private string FormatSize(long size)
        {
            if (size == 0)
                return string.Empty;

            StringBuilder sbBuffer = new StringBuilder(20);
            StrFormatByteSize(size, sbBuffer, 20);
            return sbBuffer.ToString();
        }

        private string FormateDate(DateTime inputDate)
        {
            return inputDate.ToLocalTime().ToString();
        }

        private void txtURL_KeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Enter)
            {
                string newReposPath;
                if( txtURL.Text.StartsWith("file://"))
                {
                    newReposPath = txtURL.Text;
                }
                else
                {
                    newReposPath = "file://" + txtURL.Text;
                }
                TryOpenRepository(newReposPath);
            }
        }
    }

    public class CompareAlphabetically : IComparer<SVNDirEntry>
    {
        public int Compare(SVNDirEntry x, SVNDirEntry y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }
}