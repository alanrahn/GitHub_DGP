﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Security
{
    public partial class frmEditRole : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucApiRoles _parent;
        string _rolegid;
        string _rowdate;

        public frmEditRole(HttpClient httpClient, MainForm mainForm, ucApiRoles parentForm, string roleGID)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _rolegid = roleGID;

                DisableControls();

                if (_rolegid == null || _rolegid == "")
                {
                    // new method (empty form)
                    this.Text = "New Role";
                    tbxRoleName.Enabled = true;
                    tbxRoleName.TabStop = true;
                    btnDelete.Enabled = false;
                    btnDelete.TabStop = false;

                }
                else
                {
                    // update of existing method (description only)
                    this.Text = "Edit Role";
                    btnDelete.Enabled = true;
                    btnDelete.TabStop = true;
                    Populate();
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditRole", ex);
                MessageBox.Show(ex.Message, "frmEditRole", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                methparams.Add(APIRoleFields.RoleGID, _rolegid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("APIRole.GetByID.base",
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
                            tbxRoleName.Text = dr[APIRoleFields.RoleName].ToString();
                            tbxDescription.Text = dr[APIRoleFields.RoleDescrip].ToString();
                            _rowdate = dr[CommonFields.row_ms].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditRole.Populate", ex);
                MessageBox.Show(ex.Message, "frmEditRole.Populate", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    apimethcall = "APIRole.New.base";
                    isValid = CheckFields("NEW");
                }
                else
                {
                    apimethcall = "APIRole.Save.base";
                    methparams.Add(APIRoleFields.RoleGID, tbxGlobalID.Text);
                    methparams.Add(CommonFields.row_ms, _rowdate);
                    isValid = CheckFields("SAVE");
                }

                if (isValid)
                {
                    methparams.Add(APIRoleFields.RoleName, tbxRoleName.Text);
                    methparams.Add(APIRoleFields.RoleDescrip, tbxDescription.Text);

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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditRole.btnSave_Click", ex);
                MessageBox.Show(ex.Message, "frmEditRole.btnSave_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckFields("delete"))
                {
                    Dictionary<string, string> methparams = new Dictionary<string, string>();
                    methparams.Add(APIRoleFields.RoleGID, tbxGlobalID.Text);
                    methparams.Add(APIRoleFields.RoleName, tbxRoleName.Text);
                    methparams.Add(APIRoleFields.RoleDescrip, tbxDescription.Text);
                    methparams.Add(CommonFields.row_ms, _rowdate);

                    MsgUtil msgUtil = new MsgUtil();
                    ResInfo methresult = msgUtil.ApiMethHelper("APIRole.Delete.base",
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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditRole.btnDelete_Click", ex);
                MessageBox.Show(ex.Message, "EditRole:btnDelete_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void DisableControls()
        {
            tbxGlobalID.Enabled = false;
            tbxGlobalID.TabStop = false;
            tbxRoleName.Enabled = false;
            tbxRoleName.TabStop = false;
            btnDelete.Enabled = false;
        }

        private bool CheckFields(string action)
        {
            bool fieldsOK = true;

            if (action == "SAVE")
            {
                if (tbxGlobalID.Text == null || tbxGlobalID.Text == "") fieldsOK = false;
            }

            if (tbxRoleName.Text == null || tbxRoleName.Text == "") fieldsOK = false;
            if (tbxDescription.Text == null || tbxDescription.Text == "") fieldsOK = false;

            return fieldsOK;
        }
    }
}
