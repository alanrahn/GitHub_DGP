using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.User
{
    public partial class frmEditSelf : Form
    {
        HttpClient _httpClient;
        MainForm _main;

        public frmEditSelf(HttpClient httpClient, MainForm mainForm)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;

                // populate the form with current values
                Populate();
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditSelf", ex);
                MessageBox.Show(ex.Message, "frmEditSelf", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                methparams.Add(APIUserFields.UserGID, _main.UserGID);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("UserSelf.GetInfo.base",
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditSelf.Populate", ex);
                MessageBox.Show(ex.Message, "frmEditSelf.Populate", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckFields())
                {
                    Dictionary<string, string> methparams = new Dictionary<string, string>();
                    methparams.Add(CommonFields.rec_gid, tbxGlobalID.Text);
                    methparams.Add(APIUserFields.FirstName, tbxFirstName.Text);
                    methparams.Add(APIUserFields.MiddleName, tbxMiddleName.Text);
                    methparams.Add(APIUserFields.LastName, tbxLastName.Text);
                    methparams.Add(APIUserFields.Email, tbxEmail.Text);

                    MsgUtil msgUtil = new MsgUtil();
                    ResInfo methresult = msgUtil.ApiMethHelper("UserSelf.Save.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                _httpClient,
                                                                _main.SvcUrl);

                    if (methresult.RCode.ToUpper() == APIResult.OK)
                    {
                        // close the dialog
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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditSelf.btnSave_Click", ex);
                MessageBox.Show(ex.Message, "frmEditSelf.btnSave_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckFields()
        {
            bool fieldsOK = true;

            if (tbxGlobalID.Text == null || tbxGlobalID.Text == "") fieldsOK = false;
            if (tbxUserName.Text == null || tbxUserName.Text == "") fieldsOK = false;

            if (tbxFirstName.Text == null || tbxFirstName.Text == "") fieldsOK = false;
            if (tbxLastName.Text == null || tbxLastName.Text == "") fieldsOK = false;
            if (tbxEmail.Text == null || tbxEmail.Text == "") fieldsOK = false;

            return fieldsOK;
        }

    }

    
}
