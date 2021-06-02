﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Configuration
{
    public partial class frmEditGeneral : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucGeneralWork _parent;
        string _latestgid;
        string _rowdate;

        public frmEditGeneral(HttpClient httpClient, MainForm mainForm, ucGeneralWork parentForm, string latestGID)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _latestgid = latestGID;

                cmbWorkType.SelectedIndex = 0;
                cmbRunState.SelectedIndex = 0;
                cmbLogging.SelectedIndex = 0;

                if (_latestgid == null || _latestgid == "")
                {
                    // new group (empty form)
                    this.Text = "New GeneralWork Configuration";
                    btnDelete.Enabled = false;
                    tbxIntervalVal.Text = "0";
                    tbxNextRun.Text = "0";
                    tbxStartID.Text = "0";
                    tbxFinalID.Text = "0";
                    tbxMaxDuration.Text = "0";
                }
                else
                {
                    // update of existing group (description only)
                    this.Text = "Edit GeneralWork Configuration";
                    btnDelete.Enabled = true;
                    PopulateForm();
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditGeneral", ex);
                MessageBox.Show(ex.Message, "frmEditGeneral", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateForm()
        {
            try
            {
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(CommonFields.SchemaFlag, BoolVal.TRUE);
                methparams.Add(CommonFields.rec_gid, _latestgid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("GeneralWork.GetByID.base",
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
                            _rowdate = dr[CommonFields.row_ms].ToString();


                            cmbWorkType.Text = dr[WorkFields.WorkType].ToString();

                            cmbNetwork.Text = dr[WorkFields.Network].ToString();
                            tbxSchemaTable.Text = dr[WorkFields.SchemaTable].ToString();
                            tbxSrcDBName.Text = dr[WorkFields.SrcDBName].ToString();
                            tbxShardName.Text = dr[FileFields.ShardName].ToString();
                            tbxSrcURL.Text = dr[WorkFields.SrcURL].ToString();
                            tbxSrcMethod.Text = dr[WorkFields.SrcMethod].ToString();
                            tbxDestURL.Text = dr[WorkFields.DestURL].ToString();
                            tbxDestMethod.Text = dr[WorkFields.DestMethod].ToString();
                            cmbIntervalType.Text = dr[WorkFields.IntervalType].ToString();
                            tbxIntervalVal.Text = dr[WorkFields.IntervalVal].ToString();
                            tbxNextRun.Text = dr[WorkFields.NextRun].ToString();
                            cmbRunState.Text = dr[WorkFields.RunState].ToString();
                            cmbLogging.Text = dr[WorkFields.Logging].ToString();
                            tbxStartID.Text = dr[WorkFields.StartID].ToString();
                            tbxFinalID.Text = dr[WorkFields.FinalID].ToString();
                            tbxMaxDuration.Text = dr[WorkFields.MaxDurMS].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditGeneral.Populate", ex);
                MessageBox.Show(ex.Message, "frmEditGeneral.Populate", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    apimethcall = "GeneralWork.New.base";
                    isValid = CheckFields("NEW");
                }
                else
                {
                    apimethcall = "GeneralWork.Save.base";
                    methparams.Add(CommonFields.rec_gid, tbxGlobalID.Text);
                    methparams.Add(CommonFields.row_ms, _rowdate);
                    isValid = CheckFields("UPDATE");
                }

                if (isValid)
                {
                    methparams.Add(WorkFields.Network, cmbNetwork.Text);
                    methparams.Add(WorkFields.SchemaTable, tbxSchemaTable.Text);
                    methparams.Add(WorkFields.SrcDBName, tbxSrcDBName.Text);
                    methparams.Add(FileFields.ShardName, tbxShardName.Text);
                    methparams.Add(WorkFields.WorkType, cmbWorkType.Text);
                    methparams.Add(WorkFields.SrcURL, tbxSrcURL.Text);
                    methparams.Add(WorkFields.SrcMethod, tbxSrcMethod.Text);
                    methparams.Add(WorkFields.DestURL, tbxDestURL.Text);
                    methparams.Add(WorkFields.DestMethod, tbxDestMethod.Text);
                    methparams.Add(WorkFields.StartID, tbxStartID.Text);
                    methparams.Add(WorkFields.FinalID, tbxFinalID.Text);
                    methparams.Add(WorkFields.IntervalType, cmbIntervalType.Text);
                    methparams.Add(WorkFields.IntervalVal, tbxIntervalVal.Text);
                    methparams.Add(WorkFields.NextRun, tbxNextRun.Text);
                    methparams.Add(WorkFields.RunState, cmbRunState.Text);
                    methparams.Add(WorkFields.Logging, cmbLogging.Text);
                    methparams.Add(WorkFields.MaxDurMS, tbxMaxDuration.Text);

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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditGeneral.btnSave_Click", ex);
                MessageBox.Show(ex.Message, "frmEditGeneral.btnSave_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    methparams.Add(CommonFields.rec_gid, tbxGlobalID.Text);
                    methparams.Add(CommonFields.row_ms, _rowdate);
                    methparams.Add(WorkFields.Network, cmbNetwork.Text);
                    methparams.Add(WorkFields.SchemaTable, tbxSchemaTable.Text);
                    methparams.Add(WorkFields.SrcDBName, tbxSrcDBName.Text);
                    methparams.Add(FileFields.ShardName, tbxShardName.Text);
                    methparams.Add(WorkFields.WorkType, cmbWorkType.Text);
                    methparams.Add(WorkFields.SrcURL, tbxSrcURL.Text);
                    methparams.Add(WorkFields.SrcMethod, tbxSrcMethod.Text);
                    methparams.Add(WorkFields.DestURL, tbxDestURL.Text);
                    methparams.Add(WorkFields.DestMethod, tbxDestMethod.Text);
                    methparams.Add(WorkFields.StartID, tbxStartID.Text);
                    methparams.Add(WorkFields.FinalID, tbxFinalID.Text);
                    methparams.Add(WorkFields.IntervalType, cmbIntervalType.Text);
                    methparams.Add(WorkFields.IntervalVal, tbxIntervalVal.Text);
                    methparams.Add(WorkFields.NextRun, tbxNextRun.Text);
                    methparams.Add(WorkFields.RunState, cmbRunState.Text);
                    methparams.Add(WorkFields.Logging, cmbLogging.Text);
                    methparams.Add(WorkFields.MaxDurMS, tbxMaxDuration.Text);

                    MsgUtil msgUtil = new MsgUtil();
                    ResInfo methresult = msgUtil.ApiMethHelper("GeneralWork.Delete.base",
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
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmEditGeneral.btnDelete_Click", ex);
                MessageBox.Show(ex.Message, "frmEditGeneral.btnDelete_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            btnDelete.Enabled = false;
        }

        private bool CheckFields(string action)
        {
            bool fieldsOK = true;

            if (action == "SAVE")
            {
                if (tbxGlobalID.Text == null || tbxGlobalID.Text == "") fieldsOK = false;
            }

            return fieldsOK;
        }

    }
}
