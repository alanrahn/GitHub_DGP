using System;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;
using System.Collections.Generic;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Security
{
    public partial class frmMethodRoles : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucApiMethods _parent;
        string _methgid;

        public frmMethodRoles(HttpClient httpClient, MainForm mainForm, ucApiMethods parentForm, string methGID, string apiName, string methName)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _methgid = methGID;

                lblMethName.Text = apiName + "." + methName;
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(CommonFields.SchemaFlag, "true");
                methparams.Add(APIMethodFields.MethodGID, methGID);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("RoleMethod.GetMethodRoles.base",
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
                        dgvMethodRoles.DataSource = methtable;
                    }
                    else
                    {
                        dgvMethodRoles.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmMethodRoles", ex);
                MessageBox.Show(ex.Message, "frmMethodRoles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
