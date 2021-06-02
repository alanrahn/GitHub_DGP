﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Net.Http;

using ApiUtil;
using ApiUtil.DataClasses;
using System.Diagnostics;

namespace DGPLattice.Screens.Testing
{
    public partial class frmTestResultLog : Form
    {
        MainForm _main;
        HttpClient _httpClient;

        public decimal TotalRows { get; set; }
        public decimal PageSize { get; set; }
        public decimal TotalPages { get; set; }
        public int PageNum { get; set; }
        public string SearchState { get; set; }
        public string GID { get; set; }

        string _row_id;

        public frmTestResultLog(HttpClient httpClient, MainForm mainForm)
        {
            InitializeComponent();

            try
            {
                _main = mainForm;
                _httpClient = httpClient;
                //_main.SetMetrics(0, 0, 0, "", "");

                tscmbSearchCol.SelectedIndex = -1;

                SetPageSize();
                PageNum = 0;
                SetPageLabel(0, 0);
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog", ex);
                MessageBox.Show(ex.Message, "frmTestResultLog", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPageSize()
        {
            try
            {
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("TestResult.GetPageSize.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                _httpClient,
                                                                _main.SvcUrl);

                if (methresult != null && methresult.RCode == APIResult.OK)
                {
                    PageSize = Convert.ToDecimal(methresult.RVal);
                }
                else
                {
                    RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog.SetPageSize", "PageSize Error", "Error querying for the frmTestResultLog page size.");
                    MessageBox.Show("Error querying for the frmTestResultLog page size", "frmTestResultLog.SetPageSize", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog.SetPageSize", ex);
                MessageBox.Show(ex.Message, "frmTestResultLog.SetPageSize", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Search()
        {
            string countMethod = "TestResult.GetCount.base";
            string searchMethod = "TestResult.GetSearch.base";

            try
            {
                Dictionary<string, string> searchParams = new Dictionary<string, string>();
                searchParams.Add(CommonFields.PageNum, PageNum.ToString());
                searchParams.Add(CommonFields.rec_state, SearchState);
                searchParams.Add(CommonFields.SchemaFlag, BoolVal.TRUE);

                switch (tscmbSearchCol.Text)
                {
                    case LogFields.Eval:
                        searchParams.Add(LogFields.Eval, tstbxSearchVal.Text);
                        break;

                    case LogFields.APIMethod:
                        searchParams.Add(LogFields.APIMethod, tstbxSearchVal.Text);
                        break;
                }

                MsgUtil msgUtil = new MsgUtil();
                string methlist = msgUtil.CreateXMLMethod(countMethod, searchParams);
                methlist += msgUtil.CreateXMLMethod(searchMethod, searchParams);

                string reqMsg = msgUtil.CreateXMLRequest(_main.UserName, _main.Password, "", methlist);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                string respMsg = msgUtil.HttpPost(_httpClient, _main.SvcUrl, reqMsg);
                sw.Stop();

                RespInfo respinfo = new RespInfo();
                Dictionary<string, ResInfo> methresults = msgUtil.ReadResponseString(respinfo, respMsg);
                //_main.SetMetrics(2, sw.Elapsed.TotalMilliseconds, Convert.ToDouble(respinfo.SvrMS), "Lattice", "frmTestResultLog");

                if (respinfo.Auth.ToUpper() == AuthState.OK)
                {
                    ResInfo count = msgUtil.GetResult(countMethod + "_" + MethReturn.Default, methresults);

                    if (count != null && count.RCode.ToUpper() == APIResult.OK)
                    {
                        TotalRows = Convert.ToInt32(count.RVal);
                        SetTotalPages();

                        ResInfo search = msgUtil.GetResult(searchMethod + "_" + MethReturn.Default, methresults);

                        if (search.RCode.ToUpper() == APIResult.OK || search.RCode.ToUpper() == APIResult.Empty)
                        {
                            if (search.DType == APIData.DataTable)
                            {
                                // convert xml to datatable
                                CmnUtil cmnUtil = new CmnUtil();
                                DataTable methList = cmnUtil.XmlToTable(search.RVal);

                                if (methList.Rows.Count > 0)
                                {
                                    SetPageLabel(PageNum + 1, (int)TotalPages);

                                    dgvTestResults.DataSource = methList.DefaultView;
                                    //dgvTestResults.Columns[LogFields.LogTime].FillWeight = 25;
                                    //dgvTestResults.Columns[LogFields.LogTime].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss.fff";
                                }
                                else
                                {
                                    dgvTestResults.DataSource = null;
                                }
                            }
                            else
                            {
                                dgvTestResults.DataSource = null;
                            }
                        }
                        else
                        {
                            dgvTestResults.DataSource = null;
                            MessageBox.Show(search.RCode + " : " + search.RVal, "frmTestResultLog Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        dgvTestResults.DataSource = null;
                        MessageBox.Show(count.RCode + " : " + count.RVal, "frmTestResultLog Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // show error message
                    string errmsg = "The following error occurred: " + respinfo.Auth + " - " + respinfo.Info;
                    RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog.GetSearch", "Authentication error.", errmsg);
                    MessageBox.Show(errmsg, "API Request Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog.GetSearch", ex);
                MessageBox.Show(ex.Message, "frmTestResultLog.GetSearch", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetTotalPages()
        {
            decimal tmp = TotalRows / PageSize;
            TotalPages = Convert.ToInt32(Math.Ceiling(tmp));
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPageLabel(int pageNum, int totalPages)
        {
            tslblPages.Text = "Page " + pageNum.ToString() + " of " + totalPages.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearPageInfo()
        {
            TotalRows = 0;
            PageNum = 0;
            TotalPages = 0;
            SetPageLabel(0, 0);
            dgvTestResults.DataSource = null;
            //_main.SetMetrics(0, 0, 0, "", "");
        }

        // ************************************************************************************* //
        // Event Handlers ********************************************************************** //
        // ************************************************************************************* //

        private void tsbtnClear_Click(object sender, EventArgs e)
        {
            tscmbSearchCol.SelectedIndex = -1;
            tstbxSearchVal.Text = "";
            rtbxErrDetail.Text = "";
            ClearPageInfo();
        }

        private void tscmbSearchCol_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearPageInfo();
        }

        private void tscmbSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearPageInfo();
        }

        private void tsbtnSearch_Click(object sender, System.EventArgs e)
        {
            rtbxErrDetail.Text = "";
            Search();
        }

        private void tsbtnFirst_Click(object sender, EventArgs e)
        {
            PageNum = 0;
            rtbxErrDetail.Text = "";
            Search();
        }

        private void tsbtnPrev_Click(object sender, EventArgs e)
        {
            if (PageNum > 0) PageNum--;
            rtbxErrDetail.Text = "";
            Search();
        }

        private void tsbtnNext_Click(object sender, EventArgs e)
        {
            if (PageNum < TotalPages) PageNum++;
            rtbxErrDetail.Text = "";
            Search();
        }

        private void tsbtnLast_Click(object sender, EventArgs e)
        {
            PageNum = Convert.ToInt32(TotalPages - 1);
            rtbxErrDetail.Text = "";
            Search();
        }

        private void dgvTestResults_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.dgvTestResults.Rows[e.RowIndex].Selected = true;
                GID = dgvTestResults.SelectedRows[0].Cells[CommonFields.row_gid].Value.ToString();
                _row_id = dgvTestResults.SelectedRows[0].Cells[CommonFields.row_id].Value.ToString();
            }
        }

        private void dgvTestResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // populate detail text box
            this.dgvTestResults.Rows[e.RowIndex].Selected = true;
            GID = dgvTestResults.SelectedRows[0].Cells[CommonFields.row_gid].Value.ToString();

            try
            {
                // lookup method data using global id value
                Dictionary<string, string> methparams = new Dictionary<string, string>();
                methparams.Add(CommonFields.row_gid, GID);
                methparams.Add(CommonFields.SchemaFlag, BoolVal.TRUE);

                MsgUtil msgUtil = new MsgUtil();
                ResInfo methresult = msgUtil.ApiMethHelper("TestResult.GetEvalInfo.base",
                                                                _main.UserName,
                                                                _main.Password,
                                                                methparams,
                                                                _httpClient,
                                                                _main.SvcUrl);

                if (methresult != null && methresult.RCode == APIResult.OK)
                {
                    string xmlEvalInfo = "<?xml version=\"1.0\"?>\n" + methresult.RVal;
                    webbrsEvalInfo.DocumentText = xmlEvalInfo;
                }
                else
                {
                    RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog.dgvTestResults_CellClick", "ErrData Error", "Error querying for the frmTestResultLog ErrData content.");
                    MessageBox.Show("Error querying for the frmTestResultLog ErrData content", "frmTestResultLog.dgvTestResults_CellClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmTestResultLog.dgvTestResults_CellClick", ex);
                MessageBox.Show(ex.Message, "frmTestResultLog.dgvTestResults_CellClick", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
