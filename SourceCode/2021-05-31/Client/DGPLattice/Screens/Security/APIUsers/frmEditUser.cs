using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Security
{
    public partial class frmEditUser : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucApiUsers _parent;
        string _usergid;
        string _rowdate;

        public frmEditUser(HttpClient httpClient, MainForm mainForm, ucApiUsers parentForm, string userGID)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _usergid = userGID;

                if (_usergid != null && _usergid != "")
                {
                    // existing account, else new account
                    this.Text = "Edit User";
                    tbxUserName.Enabled = false;
                    tbxPassword.Enabled = false;
                    btnDelete.Enabled = true;

                    Populate();
                }
                else
                {
                    this.Text = "New User";
                    tbxUserName.Enabled = true;
                    tbxPassword.Enabled = true;
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditUser", ex);
                MessageBox.Show(ex.Message, "frmEditUser", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Populate()
        {
            try
            {
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(CommonFields.SchemaFlag, "true");
                methparams.Add(APIUserFields.UserGID, _usergid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("APIUser.GetByID.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                _httpClient,
                                                                _main.SvcUrl);

                if (methresult != null && methresult.RCode == APIResult.OK && methresult.DType == APIData.DataTable)
                {
                    CmnUtil cmnUtil = new CmnUtil();
                    DataTable methtable = cmnUtil.XmlToTable(methresult.RVal);
                    if (methtable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in methtable.Rows)
                        {
                            tbxGlobalID.Text = dr[CommonFields.rec_gid].ToString();
                            tbxUserName.Text = dr[APIUserFields.UserName].ToString();
                            tbxFirstName.Text = dr[APIUserFields.FirstName].ToString();
                            tbxMiddleName.Text = dr[APIUserFields.MiddleName].ToString();
                            tbxLastName.Text = dr[APIUserFields.LastName].ToString();
                            tbxEmail.Text = dr[APIUserFields.Email].ToString();
                            long unixtimeMS = Convert.ToInt64(dr[APIUserFields.ExpireDate]);
                            DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(unixtimeMS);
                            dtpExpiration.Value = dto.DateTime;
                            cbxAcctState.Text = dr[APIUserFields.AccountState].ToString();
                            cbxAcctType.Text = dr[APIUserFields.AccountType].ToString();
                            tbxRateLimit.Text = dr[APIUserFields.MethodLimit].ToString();
                            _rowdate = dr[CommonFields.row_ms].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditUser.Populate", ex);
                MessageBox.Show(ex.Message, "frmEditUser:Populate", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = false;
            try
            {
                string apimethcall = "";
                Dictionary<string, string> methparams = new Dictionary<string, string>();

                if (tbxGlobalID.Text == null || tbxGlobalID.Text == "")
                {
                    // new method, required parameters
                    apimethcall = "APIUser.New.base";
                    isValid = CheckFields("NEW");
                    methparams.Add(APIUserFields.Password, tbxPassword.Text);
                }
                else
                {
                    apimethcall = "APIUser.Save.base";
                    isValid = CheckFields("UPDATE");
                    methparams.Add(CommonFields.rec_gid, tbxGlobalID.Text);
                    methparams.Add(CommonFields.row_ms, _rowdate);
                }

                if (isValid)
                {
                    methparams.Add(APIUserFields.UserName, tbxUserName.Text);
                    methparams.Add(APIUserFields.FirstName, tbxFirstName.Text);
                    methparams.Add(APIUserFields.MiddleName, tbxMiddleName.Text);
                    methparams.Add(APIUserFields.LastName, tbxLastName.Text);
                    methparams.Add(APIUserFields.Email, tbxEmail.Text);
                    methparams.Add(APIUserFields.AccountType, cbxAcctType.Text);
                    methparams.Add(APIUserFields.AccountState, cbxAcctState.Text);
                    DateTimeOffset dto = new DateTimeOffset(dtpExpiration.Value);
                    methparams.Add(APIUserFields.ExpireDate, dto.ToUnixTimeMilliseconds().ToString());
                    methparams.Add(APIUserFields.MethodLimit, tbxRateLimit.Text);

                    MsgUtil msgUtil = new MsgUtil();
                    ResInfo methresult = msgUtil.ApiMethHelper(apimethcall,
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
                else
                {
                    MessageBox.Show("You must provide values for all required fields", "Required Field Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditUser.btnSave_Click", ex);
                MessageBox.Show(ex.Message, "frmEditUser.btnSave_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckFields("UPDATE"))
                {
                    Dictionary<string, string> methparams = new Dictionary<string, string>();
                    methparams.Add(CommonFields.rec_gid, tbxGlobalID.Text);
                    methparams.Add(APIUserFields.FirstName, tbxFirstName.Text);
                    methparams.Add(APIUserFields.MiddleName, tbxMiddleName.Text);
                    methparams.Add(APIUserFields.LastName, tbxLastName.Text);
                    methparams.Add(APIUserFields.Email, tbxEmail.Text);
                    methparams.Add(APIUserFields.AccountType, cbxAcctType.Text);
                    methparams.Add(APIUserFields.AccountState, cbxAcctState.Text);
                    DateTimeOffset dto = new DateTimeOffset(dtpExpiration.Value);
                    methparams.Add(APIUserFields.ExpireDate, dto.ToUnixTimeMilliseconds().ToString());
                    methparams.Add(APIUserFields.MethodLimit, tbxRateLimit.Text);
                    methparams.Add(CommonFields.row_ms, _rowdate);

                    MsgUtil msgUtil = new MsgUtil();
                    ResInfo methresult = msgUtil.ApiMethHelper("APIUser.Delete.base",
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
                        // error saving method info: display error message
                        MessageBox.Show(methresult.RVal, methresult.RCode, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You must provide values for all required fields", "Required Field Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditUser.btnDelete_Click", ex);
                MessageBox.Show(ex.Message, "frmEditUser.btnDelete_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // *********************************************************** //
        // *********************************************************** //

        private bool CheckFields(string action)
        {
            bool fieldsOK = true;

            if (action == "UPDATE")
            {
                if (tbxGlobalID.Text == null || tbxGlobalID.Text == "") fieldsOK = false;
            }
            else
            {
                if (tbxPassword.Text == null || tbxPassword.Text == "") fieldsOK = false;
            }

            if (tbxUserName.Text == null || tbxUserName.Text == "") fieldsOK = false;
            if (tbxFirstName.Text == null || tbxFirstName.Text == "") fieldsOK = false;
            if (tbxLastName.Text == null || tbxLastName.Text == "") fieldsOK = false;
            if (tbxEmail.Text == null || tbxEmail.Text == "") fieldsOK = false;
            if (dtpExpiration.Text == null || dtpExpiration.Text == "") fieldsOK = false;
            if (cbxAcctType.Text == null || cbxAcctType.Text == "") fieldsOK = false;
            if (cbxAcctState.Text == null || cbxAcctState.Text == "") fieldsOK = false;
            if (tbxRateLimit.Text == null || tbxRateLimit.Text == "") fieldsOK = false;

            return fieldsOK;
        }

    }
}
