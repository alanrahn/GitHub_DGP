﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.FileStore
{
    public partial class frmEditTag : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucTags _parent;
        string _taggid;
        string _row_ms;

        public frmEditTag(HttpClient httpClient, MainForm mainForm, ucTags parentForm, string tagGID)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _taggid = tagGID;

                DisableControls();

                if (_taggid == null || _taggid == "")
                {
                    // new method (empty form)
                    this.Text = "New Tag";
                    tbxTagName.Enabled = true;
                    tbxTagName.TabStop = true;
                    btnDelete.Enabled = false;
                    btnDelete.TabStop = false;

                }
                else
                {
                    // update of existing method (description only)
                    this.Text = "Edit Tag";
                    btnDelete.Enabled = true;
                    btnDelete.TabStop = true;
                    Populate();
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditTag", ex);
                MessageBox.Show(ex.Message, "frmEditTag", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                methparams.Add(TagFields.TagGID, _taggid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("Tag.GetByID.base",
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
                            tbxTagName.Text = dr[TagFields.TagName].ToString();
                            tbxDescription.Text = dr[TagFields.TagDescrip].ToString();
                            _row_ms = dr[CommonFields.row_ms].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditTag.Populate", ex);
                MessageBox.Show(ex.Message, "frmEditTag.Populate", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    apimethcall = "Tag.New.base";
                    isValid = CheckFields("NEW");
                }
                else
                {
                    apimethcall = "Tag.Save.base";
                    methparams.Add(TagFields.TagGID, tbxGlobalID.Text);
                    methparams.Add(CommonFields.row_ms, _row_ms);
                    isValid = CheckFields("SAVE");
                }

                if (isValid)
                {
                    methparams.Add(TagFields.TagName, tbxTagName.Text);
                    methparams.Add(TagFields.TagDescrip, tbxDescription.Text);

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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditTag.btnSave_Click", ex);
                MessageBox.Show(ex.Message, "frmEditTag.btnSave_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    methparams.Add(TagFields.TagGID, tbxGlobalID.Text);
                    methparams.Add(TagFields.TagName, tbxTagName.Text);
                    methparams.Add(TagFields.TagDescrip, tbxDescription.Text);
                    methparams.Add(CommonFields.row_ms, _row_ms);

                    MsgUtil msgUtil = new MsgUtil();
                    ResInfo methresult = msgUtil.ApiMethHelper("Tag.Delete.base",
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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditTag.btnDelete_Click", ex);
                MessageBox.Show(ex.Message, "frmEditTag.btnDelete_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            tbxTagName.Enabled = false;
            tbxTagName.TabStop = false;
            btnDelete.Enabled = false;
        }

        private bool CheckFields(string action)
        {
            bool fieldsOK = true;

            if (action == "SAVE")
            {
                if (tbxGlobalID.Text == null || tbxGlobalID.Text == "") fieldsOK = false;
            }

            if (tbxTagName.Text == null || tbxTagName.Text == "") fieldsOK = false;

            return fieldsOK;
        }
    }
}
