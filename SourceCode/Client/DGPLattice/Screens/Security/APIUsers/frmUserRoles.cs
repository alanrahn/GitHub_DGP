﻿using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Security
{
    public partial class frmUserRoles : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucApiUsers _parent;
        string _usergid;
        string _username;

        public frmUserRoles(HttpClient httpClient, MainForm mainForm, ucApiUsers parentForm, string userGID, string userName)
        {
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _usergid = userGID;
                _username = userName;

                if (_usergid != null && _usergid != "")
                {
                    _usergid = userGID;
                    _username = userName;
                    this.Text = "User Roles: " + _username;
                    PopulateGrids();
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmUserRoles", ex);
                MessageBox.Show(ex.Message, "frmUserRoles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateGrids()
        {
            try
            {
                dgvAssignedRoles.DataSource = null;
                dgvAvailableRoles.DataSource = null;

                Dictionary<string, string> methsearchparams = new Dictionary<string, string>();
                methsearchparams.Add("UserGID", _usergid);
                methsearchparams.Add("SchemaFlag", "true");

                MsgUtil msgUtil = new MsgUtil();
                string methlist = msgUtil.CreateXMLMethod("RoleUser.GetAssigned.base", methsearchparams);
                methlist += msgUtil.CreateXMLMethod("RoleUser.GetAvailable.base", methsearchparams);

                string reqMsg = msgUtil.CreateXMLRequest(_main.UserName, _main.Password, "", methlist);

                string respMsg = msgUtil.HttpPost(_httpClient, _main.SvcUrl, reqMsg);

                // read response message results
                RespInfo respinfo = new RespInfo();
                Dictionary<string, ResInfo> methresults = msgUtil.ReadResponseString(respinfo, respMsg);

                if (respinfo.Auth.ToUpper() == AuthState.OK)
                {
                    CmnUtil cmnUtil = new CmnUtil();

                    ResInfo assignedresult = msgUtil.GetResult("RoleUser.GetAssigned.base_DEFAULT", methresults);
                    if (assignedresult != null && assignedresult.RCode == APIResult.OK && assignedresult.DType == APIData.DataTable)
                    {
                        DataTable assignedTbl = cmnUtil.XmlToTable(assignedresult.RVal);
                        if (assignedTbl.Rows.Count > 0)
                        {
                            dgvAssignedRoles.DataSource = assignedTbl;
                        }
                    }

                    ResInfo availableresult = msgUtil.GetResult("RoleUser.GetAvailable.base_DEFAULT", methresults);
                    if (availableresult != null && availableresult.RCode == APIResult.OK && availableresult.DType == APIData.DataTable)
                    {
                        DataTable availableTbl = cmnUtil.XmlToTable(availableresult.RVal);
                        if (availableTbl.Rows.Count > 0)
                        {
                            dgvAvailableRoles.DataSource = availableTbl;
                        }
                    }
                }
                else
                {
                    RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmUserRoles.PopulateGrids", "Authentication error.", "Authentication error = " + respinfo.Auth);
                    MessageBox.Show(respinfo.Info, "frmUserRoles.PopulateGrids", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmUserRoles.PopulateGrids", ex);
                MessageBox.Show(ex.Message, "frmUserRoles.PopulateGrids", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// add a method to the role
        /// </summary>
        private void dgvAvailableRoles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // add selected method
                this.dgvAvailableRoles.Rows[e.RowIndex].Selected = true;

                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add("RoleGID", dgvAvailableRoles.SelectedRows[0].Cells["RoleGID"].Value.ToString());
                methparams.Add("UserGID", _usergid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("RoleUser.Assign.base",
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
                    MessageBox.Show("Unable to assign role to the user.", "Assign Role Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmUserRoles.dgvAvailableRoles_CellContentDoubleClick", ex);
                MessageBox.Show(ex.Message, "frmUserRoles.dgvAvailableRoles_CellContentDoubleClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// remove a method from the role
        /// </summary>
        private void dgvAssignedRoles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // remove selected method
                this.dgvAssignedRoles.Rows[e.RowIndex].Selected = true;

                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add("RoleUserGID", dgvAssignedRoles.SelectedRows[0].Cells["RoleUserGID"].Value.ToString());
                methparams.Add("RoleGID", dgvAssignedRoles.SelectedRows[0].Cells["RoleGID"].Value.ToString());
                methparams.Add("UserGID", _usergid);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("RoleUser.Remove.base",
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
                    MessageBox.Show("Unable to remove the role from the user.", "Remove Role Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmUserRoles.dgvAssignedRoles_CellContentDoubleClick", ex);
                MessageBox.Show(ex.Message, "frmUserRoles.dgvAssignedRoles_CellContentDoubleClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
