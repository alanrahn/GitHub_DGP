using System;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;
using System.Collections.Generic;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Security
{
    public partial class frmGroupHist : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucDataGroup _parent;
        string _groupgid;

        public frmGroupHist(HttpClient httpClient, MainForm mainForm, ucDataGroup parentForm, string groupGID, string groupName)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _groupgid = groupGID;

                lblGroupName.Text = groupName;
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(CommonFields.SchemaFlag, "true");
                methparams.Add(DataGroupFields.GroupGID, _groupgid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("DataGroup.GetHistory.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                httpClient,
                                                                _main.SvcUrl);

                if (methresult != null && methresult.RCode == APIResult.OK && methresult.DType == APIData.DataTable)
                {
                    CmnUtil cmnUtil = new CmnUtil();
                    DataTable methtable = cmnUtil.XmlToTable(methresult.RVal);
                    if (methtable.Rows.Count > 0)
                    {
                        dgvGroupHistory.DataSource = methtable;
                        if (_parent.SearchState == RecState.Active) btnRecoverSelected.Enabled = true;
                    }
                    else
                    {
                        dgvGroupHistory.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmGroupHist", ex);
                MessageBox.Show(ex.Message, "frmGroupHist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRecoverSelected_Click(object sender, EventArgs e)
        {
            // read values from selected row in grid
            string recgid = dgvGroupHistory.SelectedRows[0].Cells[CommonFields.rec_gid].Value.ToString();
            string rowid = dgvGroupHistory.SelectedRows[0].Cells[CommonFields.row_id].Value.ToString();

            // call process method to recover edited record
            Dictionary<string, string> methparams = new Dictionary<string, string>();
            methparams.Add("Action", ReplicaAction.Update);
            methparams.Add(CommonFields.rec_gid, recgid);
            methparams.Add(CommonFields.row_id, rowid);

            MsgUtil msgUtil = new MsgUtil();
            ResInfo methresult = msgUtil.ApiMethHelper("DataGroup.Recover.base",
                                                            _main.UserName,
                                                            _main.Password,
                                                            methparams,
                                                            _httpClient,
                                                            _main.SvcUrl);

            if (methresult.RCode.ToUpper() == APIResult.OK)
            {
                // update the parent form and then close the dialog
                _parent.Search();
                this.Close();
            }
            else
            {
                // error saving user info: display error message
                MessageBox.Show(methresult.RVal, methresult.RCode, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
