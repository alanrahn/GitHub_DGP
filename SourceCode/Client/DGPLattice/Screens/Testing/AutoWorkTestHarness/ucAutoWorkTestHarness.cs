
using System;
using System.Windows.Forms;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.Testing
{
    public partial class ucAutoWorkTestHarness : UserControl
    {
        MainForm _main;

        string _claimToken = "";
        string _autoworklog = "";
        int _claimcount;

        string _runmode = "ReplicaWork";
        string _claimMethod = "ReplicaWork.ClaimRecords.base";
        string _releaseMethod = "ReplicaWork.ReleaseRecord.base";

        bool _polling;

        public ucAutoWorkTestHarness(MainForm main)
        {
            InitializeComponent();

            try
            {
                _main = main;
                _main.SetMetrics(0, 0, 0, "", "");

                _claimToken = _main.UserName + "_DGPLattice";
                _autoworklog = ConfigurationManager.AppSettings["AutoWorkLogging"].ToString();

                tscmbInterval.SelectedIndex = 0;
                tscmbRunMode.SelectedIndex = 0;

                tslblSelWorkType.Text = "ReplicaWork";

                PopulateGrid();
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "ucAutoWorkTestHarness", "ucAutoWorkTestHarness", ex);
                MessageBox.Show(ex.Message, "ucAutoWorkTestHarness", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartPolling()
        {
            _polling = true;
            tsbtnPolling.Text = "Stop Polling";
            SetInterval();
            autoTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopPolling()
        {
            _polling = false;
            tsbtnPolling.Text = "Start Polling";
            autoTimer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetInterval()
        {
            int interval = 5000;

            if (tscmbInterval.Text != null && tscmbInterval.Text != "")
            {
                interval = Convert.ToInt32(tscmbInterval.Text) * 1000;
            }

            autoTimer.Interval = interval;
        }

        private void PopulateGrid()
        {
            Dictionary<string, string> autoparams = new Dictionary<string, string>();
            autoparams.Add(CommonFields.SchemaFlag, BoolVal.TRUE);

            MsgUtil msgUtil = new MsgUtil();
            CmnUtil cmnUtil = new CmnUtil();

            string reqid = cmnUtil.GetNewGID();

            ResInfo workresult = msgUtil.ApiMethHelper("AutoWorkLog.GetAll.base",
                                                            _main.UserName,
                                                            _main.Password,
                                                            autoparams,
                                                            _main.SvcUrl);

            if (workresult != null && (workresult.RCode == APIResult.OK && workresult.DType == APIData.DataTable))
            {
                DataTable workrecs = cmnUtil.XmlToTable(workresult.RVal);

                dgvAutoWorkLog.DataSource = workrecs;
            }
        }


        // ************************************************************************************* //
        // Event Handlers ********************************************************************** //
        // ************************************************************************************* //

        /// <summary>
        /// 
        /// </summary>
        private void tsbtnPolling_Click(object sender, EventArgs e)
        {
            if (_polling == true)
            {
                StopPolling();
            }
            else
            {
                StartPolling();
            }
        }

        /// <summary>
        /// the logic works for both LatestWork and VerifyWork processes
        /// </summary>
        private void AutoTimer_Tick(object sender, EventArgs e)
        {
            autoTimer.Stop();
            Stopwatch claimed = new Stopwatch();
            claimed.Start();
            DateTime startTime = DateTime.UtcNow;

            string _runstate = "";
            string _statemsg = "";
            string _procsteps = "";

            string _network = ConfigurationManager.AppSettings["Network"].ToString();
            long _maxdurms = Convert.ToInt64(ConfigurationManager.AppSettings["AutoWorkMaxDurMS"]);

            if (_autoworklog == "ON")
            {
                _procsteps += "StartTime: " + startTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + ";\n";
                _procsteps += "ClaimedBy: " + _claimToken + ";\n";
                _procsteps += "RunMode: " + _runmode + ";\n";
            }

            Dictionary<string, string> claimparams = new Dictionary<string, string>();
            claimparams.Add(CommonFields.SchemaFlag, BoolVal.TRUE);
            claimparams.Add(WorkFields.Network, _network);
            claimparams.Add(WorkFields.ClaimedBy, _claimToken);

            MsgUtil msgUtil = new MsgUtil();
            CmnUtil cmnUtil = new CmnUtil();

            string reqid = cmnUtil.GetNewGID();

            ResInfo claimresult = msgUtil.ApiMethHelper(_claimMethod,
                                                            _main.UserName,
                                                            _main.Password,
                                                            claimparams,
                                                            _main.SvcUrl);

            if (claimresult != null && (claimresult.RCode == APIResult.OK || claimresult.RCode == APIResult.Empty))
            {
                if (claimresult.RCode == APIResult.OK)
                {
                    if (claimresult.DType == APIData.DataTable)
                    {
                        DataTable _claimedRecs = cmnUtil.XmlToTable(claimresult.RVal);

                        if (_claimedRecs.Rows.Count > 0)
                        {
                            _claimcount = _claimedRecs.Rows.Count;

                            // run each batch of claimed records as a standalone fire-and-forget process
                            if (_runmode == "RepWork")
                            {
                                Task.Run(() =>
                                Parallel.ForEach(_claimedRecs.AsEnumerable(), (drow) =>
                                {
                                    ReplicaWorkProc.Replicate(_main.SvcUrl, _releaseMethod, _main.UserName, _main.Password, drow, _main.MaxSegSize);
                                }));
                            }
                            else if (_runmode == "GenWork")
                            {
                                Task.Run(() =>
                                Parallel.ForEach(_claimedRecs.AsEnumerable(), (drow) =>
                                {
                                    GeneralWorkProc.DoWork(_main.SvcUrl, _releaseMethod, _main.UserName, _main.Password, drow);
                                }));
                            }
                        }
                        else
                        {
                            _claimcount = 0;
                        }
                    }
                }
            }
            else
            {
                string errmsg = "The following error occurred: " + claimresult.RCode + " - " + claimresult.RVal;
                RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "ucAutoWorkTestHarness", "AutoTimer_Tick", "Error claiming work record.", errmsg);
            }

            claimed.Stop();
            long duration = claimed.ElapsedMilliseconds;

            // check if duration exceeds max limit
            if (_maxdurms !=  0 && duration > _maxdurms)
            {
                // log error
                RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "ucAutoWorkTestHarness", _runmode, "Process duration error.", "Process duration = " + duration.ToString() + " MS : Max duration = " + _maxdurms.ToString() + " MS");
            }

            if (_autoworklog == "ON")
            {
                _procsteps += "ClaimedRecs: " + _claimcount.ToString() + ";\n";
                _procsteps += "EndTime: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + ";\n";

                // call AutoWorkLog API
                Dictionary<string, string> autoparams = new Dictionary<string, string>();
                autoparams.Add(WorkFields.WorkType, _runmode);
                autoparams.Add(LogFields.CompName, Environment.MachineName);
                autoparams.Add(WorkFields.ClaimedBy, _claimToken);
                autoparams.Add(LogFields.DurationMS, duration.ToString());
                autoparams.Add(WorkFields.MaxDurMS, _maxdurms.ToString());
                autoparams.Add(WorkFields.RunState, _runstate);
                autoparams.Add(WorkFields.StateMsg, _statemsg);
                autoparams.Add(WorkFields.ProcState, ProcState.Working);
                autoparams.Add(WorkFields.ProcSteps, _procsteps);

                ResInfo autologresult = msgUtil.ApiMethHelper("AutoWorkLog.New.base",
                                                            _main.UserName,
                                                            _main.Password,
                                                            autoparams,
                                                            _main.SvcUrl);

                if (autologresult.RCode == null || autologresult.RCode != APIResult.OK)
                {
                    // log error
                    RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "ucAutoWorkTestHarness", _runmode, "Error creating ReplicaWorkLog record: " + autologresult.RVal, _procsteps);
                }

                PopulateGrid();
            }

            if (tscmbRunMode.Text == "REPEAT")
            {
                autoTimer.Start();
            }
            else
            {
                StopPolling();
            }
        }

        private void tscmbInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPolling();
            SetInterval();
        }

        private void tscmbRunMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPolling();
        }

        private void tsbtnRefresh_Click(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void replicaWorkMenuItem_Click(object sender, EventArgs e)
        {
            _runmode = "RepWork";
            _claimMethod = "ReplicaWork.ClaimRecords.base";
            _releaseMethod = "ReplicaWork.ReleaseRecord.base";

            tslblSelWorkType.Text = "ReplicaWork";
            dgvAutoWorkLog.DataSource = null;
        }

        private void generalWorkMenuItem_Click(object sender, EventArgs e)
        {
            _runmode = "GenWork";
            _claimMethod = "GeneralWork.ClaimRecords.base";
            _releaseMethod = "GeneralWork.ReleaseRecord.base";

            tslblSelWorkType.Text = "GeneralWork";
            dgvAutoWorkLog.DataSource = null;
        }

        private void dgvAutoWorkLog_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // populate detail text box
            this.dgvAutoWorkLog.Rows[e.RowIndex].Selected = true;
        }

        private void viewMenuItem_Click(object sender, EventArgs e)
        {
            // populate detail text box
            string procdetail = dgvAutoWorkLog.SelectedRows[0].Cells["ProcSteps"].Value.ToString();
            frmProcDetail procDetail = new frmProcDetail(procdetail);
            procDetail.ShowDialog();
        }

        private void dgvAutoWorkLog_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dgvAutoWorkLog.ClearSelection();
                this.dgvAutoWorkLog.Rows[e.RowIndex].Selected = true;
            }
        }
    }
}
