﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;
using System.Data;

namespace DGPLattice.Screens.FileStore
{
    public partial class frmFileTags : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucFileStore _parent;

        string _filegid;
        string _filename;
        string _tagname = "";

        public frmFileTags(HttpClient httpClient, MainForm mainForm, ucFileStore parentForm, string fileGID, string fileName)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;

                _filegid = fileGID;
                _filename = fileName;

                // set title
                string baseTitle = "File Tags: ";
                this.Text = baseTitle + fileName;

                PopulateGrids();
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileTags", ex);
                MessageBox.Show(ex.Message, "frmFileTags", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void PopulateGrids()
        {
            try
            {
                dgvAssignedTags.DataSource = null;
                dgvAvailableTags.DataSource = null;

                Dictionary<string, string> methsearchparams = new Dictionary<string, string>();
                methsearchparams.Add(FileFields.FileGID, _filegid);
                methsearchparams.Add(TagFields.TagName, _tagname);
                methsearchparams.Add(CommonFields.SchemaFlag, BoolVal.TRUE);

                MsgUtil msgUtil = new MsgUtil();
                string methlist = msgUtil.CreateXMLMethod("FileTag.GetAssigned.base", methsearchparams);
                methlist += msgUtil.CreateXMLMethod("FileTag.GetAvailable.base", methsearchparams);

                string reqMsg = msgUtil.CreateXMLRequest(_main.UserName, _main.Password, "", methlist);

                string respMsg = msgUtil.HttpPost(_httpClient, _main.SvcUrl, reqMsg);

                // read response message results
                RespInfo respinfo = new RespInfo();
                Dictionary<string, ResInfo> methresults = msgUtil.ReadResponseString(respinfo, respMsg);

                if (respinfo.Auth.ToUpper() == AuthState.OK)
                {
                    CmnUtil cmnUtil = new CmnUtil();

                    ResInfo assignedresult = msgUtil.GetResult("FileTag.GetAssigned.base" + "_" + MethReturn.Default, methresults);
                    if (assignedresult != null && assignedresult.RCode == APIResult.OK && assignedresult.DType == APIData.DataTable)
                    {
                        DataTable assignedTbl = cmnUtil.XmlToTable(assignedresult.RVal);
                        if (assignedTbl.Rows.Count > 0)
                        {
                            dgvAssignedTags.DataSource = assignedTbl;
                        }
                    }

                    ResInfo availableresult = msgUtil.GetResult("FileTag.GetAvailable.base" + "_" + MethReturn.Default, methresults);
                    if (availableresult != null && availableresult.RCode == APIResult.OK && availableresult.DType == APIData.DataTable)
                    {
                        DataTable availableTbl = cmnUtil.XmlToTable(availableresult.RVal);
                        if (availableTbl.Rows.Count > 0)
                        {
                            dgvAvailableTags.DataSource = availableTbl;
                        }
                    }
                }
                else
                {
                    RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileTags:PopulateGrids", "Authentication error.", "Authorization error = " + respinfo.Auth);
                    MessageBox.Show(respinfo.Info, "frmFileTags:PopulateGrids", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileTags.PopulateGrids", ex);
                MessageBox.Show(ex.Message, "frmFileTags:PopulateGrids", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            _tagname = tbxFilterTagName.Text;
            PopulateGrids();
        }

        /// <summary>
        /// add a tag to the file
        /// </summary>
        private void dgvAvailableTags_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // add selected method
                this.dgvAvailableTags.Rows[e.RowIndex].Selected = true;

                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(TagFields.TagGID, dgvAvailableTags.SelectedRows[0].Cells[TagFields.TagGID].Value.ToString());
                methparams.Add(FileFields.FileGID, _filegid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("FileTag.Assign.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                _httpClient,
                                                                _main.SvcUrl);

                if (methresult != null && methresult.RCode == APIResult.OK)
                {
                    // populate grids
                    PopulateGrids();
                }
                else
                {
                    MessageBox.Show("Unable to assign tag to the file.", "Assign Tag Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileTags.dgvAvailTags_CellDoubleClick", ex);
                MessageBox.Show(ex.Message, "frmFileTags:dgvAvailTags_CellDoubleClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// remove a method from the role
        /// </summary>
        private void dgvAssignedTags_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // remove selected method
                this.dgvAssignedTags.Rows[e.RowIndex].Selected = true;

                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(TagFields.FileTagGID, dgvAssignedTags.SelectedRows[0].Cells[TagFields.FileTagGID].Value.ToString());
                methparams.Add(TagFields.TagGID, dgvAssignedTags.SelectedRows[0].Cells[TagFields.TagGID].Value.ToString());
                methparams.Add(FileFields.FileGID, _filegid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("FileTag.Remove.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                _httpClient,
                                                                _main.SvcUrl);

                if (methresult != null && methresult.RCode == APIResult.OK)
                {
                    // populate grids
                    PopulateGrids();
                }
                else
                {
                    MessageBox.Show("Unable to remove the method from the role.", "Remove Method Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileTags.dgvAssignedMethods_CellDoubleClick", ex);
                MessageBox.Show(ex.Message, "RoleMethods.dgvAssignedMethods_CellDoubleClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
