using System;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;
using System.Collections.Generic;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Security
{
    public partial class frmUserMethods : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucApiUsers _parent;

        public frmUserMethods(HttpClient httpClient, MainForm mainForm, ucApiUsers parentForm, string userGID, string userName)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;

                lblUserName.Text = userName;
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add("SchemaFlag", "true");
                methparams.Add("UserGID", userGID);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("RoleMethod.GetUserMethods.base",
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
                        dgvUserMethods.DataSource = methtable;
                    }
                    else
                    {
                        dgvUserMethods.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmUserMethods", ex);
                MessageBox.Show(ex.Message, "frmUserMethods", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
