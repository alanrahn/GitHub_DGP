using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using ApiUtil.DataClasses;

namespace ApiUtil
{
    /// <summary>
    /// Run on the CLIENT
    /// </summary>
    public class GeneralWorkProc
    {
        public GeneralWorkProc()
        {

        }

        /// <summary>
        /// RUNS ON THE CLIENT
        /// the process will call either 1 or 2 API methods, which is why it must be run in the DMZ and not from the internal network;
        /// workURL and workMethod are used to update/release the claimed GeneralWork record
        /// </summary>
        public static void DoWork(string releaseURL,
                                    string releaseMethod,
                                    string workAcct,
                                    string WorkPword,
                                    DataRow dataRow)
        {
            Stopwatch process = new Stopwatch();
            process.Start();
            DateTime startTime = DateTime.UtcNow;
            MsgUtil msgUtil = new MsgUtil();
            long starttime = msgUtil.UnixTimeLong();

            // read work values from DataRow (not all columns are used for each work type
            string _rec_gid = dataRow[CommonFields.rec_gid].ToString();
            string _worktype = dataRow[WorkFields.WorkType].ToString();
            string _srcdbname = dataRow[WorkFields.SrcDBName].ToString();
            string _srcurl = dataRow[WorkFields.SrcURL].ToString();
            string _srcmethod = dataRow[WorkFields.SrcMethod].ToString();
            string _desturl = dataRow[WorkFields.DestURL].ToString();
            string _destmethod = dataRow[WorkFields.DestMethod].ToString();
            string _claimedby = dataRow[WorkFields.ClaimedBy].ToString();
            string _logging = dataRow[WorkFields.Logging].ToString();
            string _startid = dataRow[WorkFields.StartID].ToString();
            string _intervaltype = dataRow[WorkFields.IntervalType].ToString();
            string _intervalVal = dataRow[WorkFields.IntervalVal].ToString();
            string _resetflag = dataRow[WorkFields.ResetFlag].ToString();
            long _maxdurms = Convert.ToInt64(dataRow[WorkFields.MaxDurMS]);
            long _nextrun = Convert.ToInt64(dataRow[WorkFields.NextRun]);

            long runlagms = starttime - _nextrun;

            string _runstate = "";
            string _statemsg = "";
            string _procsteps = "";

            if (_logging == "ON")
            {
                _procsteps += "StartTime: " + startTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + ";\n";
                _procsteps += "ThreadID: " + Thread.CurrentThread.ManagedThreadId.ToString() + ";\n";
                _procsteps += "RunLagMS: " + runlagms.ToString() + ";\n";
            }

            try
            {
                if (_worktype == WorkType.DupeCheck)
                {
                    // dupecheck only uses the source url and method
                    Dictionary<string, string> srcparams = new Dictionary<string, string>();
                    srcparams.Add(CountFields.SrcTblName, "");

                    ResInfo srcresult = msgUtil.ApiMethHelper(_srcmethod,
                                                                workAcct,
                                                                WorkPword,
                                                                srcparams,
                                                                _srcurl);

                   

                    if (srcresult.RCode == APIResult.OK || srcresult.RCode == APIResult.Empty)
                    {
                        _procsteps += "DupeCheck: " + srcresult.RVal + ";\n";
                    }
                    else
                    {
                        _procsteps += "DupeCheck: " + srcresult.RCode +  " : " + srcresult.RVal + ";\n";

                        // error with dupecheck method
                        if (_resetflag == BoolVal.TRUE)
                        {
                            _runstate = RunState.Disabled;
                        }

                        _statemsg = srcresult.RVal;
                        RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "Error in GeneralWork DupeCheck " + _srcmethod + " method.", srcresult.RVal);
                    }
                }
                else if (_worktype == WorkType.CountCheck)
                {
                    string startID = "0";
                    // first get counts from the destination for the source database records
                    Dictionary<string, string> destparams = new Dictionary<string, string>();
                    destparams.Add(WorkFields.SrcDBName, _srcdbname);
                    if (_startid != null && _startid != "") startID = _startid;
                    destparams.Add(WorkFields.StartID, startID);

                    ResInfo destresult = msgUtil.ApiMethHelper(_destmethod,
                                                                workAcct,
                                                                WorkPword,
                                                                destparams,
                                                                _desturl);

                    if (destresult.RCode == APIResult.OK)
                    {
                        CountInfo countInfo = new CountInfo();

                        // store returned destination values
                        string[] destvalues = destresult.RVal.Split(',');
                        countInfo.maxDestSrcID = Convert.ToInt64(destvalues[0]);
                        countInfo.DestSrcCount = Convert.ToInt32(destvalues[1]);

                        _procsteps += "DestCounts: " + countInfo.maxDestSrcID.ToString() + " : " + countInfo.DestSrcCount.ToString() + ";\n";

                        // then get counts from the source that are the equivalent of those from the destination
                        Dictionary<string, string> srcparams = new Dictionary<string, string>();
                        destparams.Add(WorkFields.SrcDBName, _srcdbname);
                        destparams.Add(WorkFields.EndID, countInfo.maxDestSrcID.ToString());

                        ResInfo srcresult = msgUtil.ApiMethHelper(_srcmethod,
                                                                    workAcct,
                                                                    WorkPword,
                                                                    srcparams,
                                                                    _srcurl);

                        if (srcresult.RCode == APIResult.OK)
                        {
                            // store returned destination values
                            string[] srcvalues = srcresult.RVal.Split(',');
                            countInfo.SrcTotalCount = Convert.ToInt32(srcvalues[1]);
                            countInfo.SrcDestCount = Convert.ToInt32(srcvalues[2]);

                            _procsteps += "SrcCounts: " + countInfo.SrcTotalCount.ToString() + " : " + countInfo.SrcDestCount.ToString() + ";\n";

                            long replicalag = countInfo.SrcTotalCount - countInfo.DestSrcCount;
                            RemoteErrLog.LogInfo(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "ReplicationLag: " + _srcdbname + " to " + _desturl + " : " + _destmethod, "Replication Lag = " + replicalag.ToString());

                            _procsteps += "ReplicaLag: " + replicalag.ToString() + ";\n";

                            // compare the source and destination counts
                            if (countInfo.DestSrcCount < countInfo.SrcDestCount)
                            {
                                // gap
                                long gapdiff = countInfo.SrcDestCount - countInfo.DestSrcCount;
                                RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "GAP: " + _srcdbname + " to " + _desturl + " : " + _destmethod, gapdiff.ToString() + " source records are missing from the destination.");

                                _procsteps += "CountCheckGap: " + gapdiff.ToString() + ";\n";
                            } 
                            else if (countInfo.DestSrcCount > countInfo.SrcDestCount)
                            {
                                // extra record(s)
                                long extradiff = countInfo.DestSrcCount - countInfo.SrcDestCount ;
                                RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "EXTRA: " + _srcdbname + " to " + _desturl + " : " + _destmethod, extradiff.ToString() + " extra destination records compared to the source.");

                                _procsteps += "CountCheckExtra: " + extradiff.ToString() + ";\n";
                            }
                            else
                            {
                                _procsteps += "CountCheck: OK;\n";
                            }
                        }
                        else
                        {
                            // error querying for source counts
                            if (_resetflag == BoolVal.TRUE)
                            {
                                _runstate = RunState.Disabled;
                            }

                            _statemsg = srcresult.RVal;
                            _resetflag = BoolVal.TRUE;
                            RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "Error in GeneralWork CountCheck " + _srcmethod + " method.", srcresult.RVal);
                        }
                    }
                    else
                    {
                        // error querying for destination counts
                        if (_resetflag == BoolVal.TRUE)
                        {
                            _runstate = RunState.Disabled;
                        }

                        _statemsg = destresult.RVal;
                        _resetflag = BoolVal.TRUE;
                        RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "Error in GeneralWork CountCheck " + _destmethod + " method.", destresult.RVal);
                    }
                }
            }
            catch (Exception ex)
            {
                // log exception info
                if (_resetflag == BoolVal.TRUE)
                {
                    _runstate = RunState.Disabled;
                }

                _statemsg = ex.Message;
                RemoteErrLog.LogException(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, ex);
            }
            finally
            {
                string _procstate = "";
                long nextrun = msgUtil.NextUnixTime(_intervaltype, _intervalVal);

                if (_runstate == RunState.Ready)
                {
                    _procstate = ProcState.Complete;
                }
                else
                {
                    _procstate = ProcState.Error;
                    _resetflag = BoolVal.TRUE;
                }

                Dictionary<string, string> workparams = new Dictionary<string, string>();
                workparams.Add(WorkFields.RunState, _runstate);
                workparams.Add(WorkFields.StateMsg, _statemsg);
                workparams.Add(WorkFields.ClaimedBy, _claimedby);
                workparams.Add(CommonFields.rec_gid, _rec_gid);
                workparams.Add(WorkFields.NextRun, nextrun.ToString());
                workparams.Add(WorkFields.ProcState, _procstate);
                workparams.Add(WorkFields.ResetFlag, _resetflag);

                ResInfo releaseresult = msgUtil.ApiMethHelper(releaseMethod,
                                                            workAcct,
                                                            WorkPword,
                                                            workparams,
                                                            releaseURL);

                process.Stop();
                long duration = process.ElapsedMilliseconds;

                if (releaseresult.RCode == null || releaseresult.RCode != APIResult.OK)
                {
                    // log error
                    RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "Error releasing GeneralWork record.", releaseresult.RVal);
                }

                // check if duration exceeds max limit
                if (_maxdurms != 0 && duration > _maxdurms)
                {
                    // log error
                    RemoteErrLog.LogError(workAcct, WorkPword, _srcurl, "GeneralWorkProc.DoWork", "GeneralWorkProc.DoWork", "Process duration error.", "Process duration = " + duration.ToString() + " MS : Max duration = " + _maxdurms.ToString() + " MS");
                }

                if (_logging == "ON")
                {
                    _procsteps += "EndTime: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + ";\n";

                    // call AutoWorkLog API
                    Dictionary<string, string> autoparams = new Dictionary<string, string>();
                    autoparams.Add(WorkFields.WorkType, _worktype);
                    autoparams.Add(LogFields.CompName, Environment.MachineName);
                    autoparams.Add(WorkFields.ClaimedBy, _claimedby);
                    autoparams.Add(LogFields.DurationMS, duration.ToString());
                    autoparams.Add(WorkFields.MaxDurMS, _maxdurms.ToString());
                    autoparams.Add(WorkFields.RunState, _runstate);
                    autoparams.Add(WorkFields.StateMsg, _statemsg);
                    autoparams.Add(WorkFields.ProcState, _procstate);
                    autoparams.Add(WorkFields.ProcSteps, _procsteps);

                    ResInfo autologresult = msgUtil.ApiMethHelper("AutoWorkLog.New.base",
                                                                workAcct,
                                                                WorkPword,
                                                                autoparams,
                                                                releaseURL);

                    if (autologresult.RCode == null || autologresult.RCode != APIResult.OK)
                    {
                        // log error
                        RemoteErrLog.LogInfo(workAcct, WorkPword, _srcurl, "GeneralWorkProc", "DoWork." + _worktype, "Error creating AutoWorkLog record: " + autologresult.RVal, _procsteps);
                    }
                    
                }
            }
        }

    }

}
