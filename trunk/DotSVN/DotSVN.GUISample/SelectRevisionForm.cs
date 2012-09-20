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
using System.Windows.Forms;

namespace DotSVN.GUISample
{
    public partial class SelectRevisionForm : Form
    {
        private long selectedRevision;

        public long SelectedRevision
        {
            get { return selectedRevision; }
        }

        public SelectRevisionForm()
        {
            InitializeComponent();

            selectedRevision = -1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            selectedRevision = -1;
            if (rbRevision.Checked)
            {
                string revText = tbRevision.Text;
                long revision = -1;
                if (!string.IsNullOrEmpty(revText) && Int64.TryParse(revText, out revision))
                {
                    selectedRevision = revision;
                }
            }
            DialogResult = DialogResult.OK;
        }
    }
}