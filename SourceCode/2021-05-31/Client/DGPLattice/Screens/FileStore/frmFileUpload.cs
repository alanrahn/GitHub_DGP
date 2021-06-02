﻿using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Net.Http;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Diagnostics;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPLattice.Screens.FileStore
{
    public partial class frmFileUpload : Form
    {
        HttpClient _httpClient;
        MainForm _main;
        ucFileStore _parent;
        LatticeNode _folder;
        FileData _filedata;
        CommonOpenFileDialog locdialog;
        BackgroundWorker bw;
        ResultHTML _resultHTML;

        string _action;
        string _rec_gid;
        string _row_ms;
        string _version;
        string _folderpath;
        string _procresult;

        string _htmlResultBase;
        string _htmlTempResult;

        int _totalSeg;
        int _segSize;

        public frmFileUpload(HttpClient httpClient, MainForm mainForm, ucFileStore parentForm, LatticeNode selNode, string folderPath, string action, string rec_gid, string filename, string filedescrip, string row_ms, string version)
        { 
            InitializeComponent();

            try
            {
                _httpClient = httpClient;
                _main = mainForm;
                _parent = parentForm;
                _folder = selNode;
                _rec_gid = rec_gid;
                
                _row_ms = row_ms;
                _version = version;
                _action = action;
                _folderpath = folderPath;

                tbxSelectedFolder.Text = folderPath;

                tbxFileName.Text = filename;
                tbxFileDescrip.Text = filedescrip;

                Environment.CurrentDirectory = _parent.UploadDir;

                locdialog = new CommonOpenFileDialog();
                locdialog.Title = "Select File to Upload";
                //locdialog.Filters.Add(new CommonFileDialogFilter("Content Files", "*.*"));
                locdialog.EnsurePathExists = true;
                locdialog.EnsureValidNames = true;
                locdialog.IsFolderPicker = false;
                locdialog.Multiselect = false;

                btnUpload.Enabled = false;

                bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork2);
                bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.WorkerReportsProgress = true;

                _segSize = Convert.ToInt32(_main.MaxSegSize);

                if (_action == ReplicaAction.Insert)
                {
                    // new file with new global ID and version
                    this.Text = "Upload New File";
                    _rec_gid = "";
                    tbxGlobalID.Text = "";
                    _version = "1";
                }
                else
                {
                    // new version of an existing file
                    this.Text = "Upload New File Version";
                    tbxGlobalID.Text = _rec_gid;
                }

                ClearResults();

                // initialize HTML progress message
                _resultHTML = new ResultHTML();
                _htmlResultBase = _resultHTML.HTMLStart() + "<div class=\"titlediv\">Upload Process:</div><div class=\"innerdiv\">";
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileUpload", ex);
                MessageBox.Show(ex.Message, "frmFileUpload", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // browse for file to upload
            try
            {
                if (_parent.UploadDir != null && _parent.UploadDir != "")
                {
                    locdialog.InitialDirectory = _parent.UploadDir;
                }
                else
                {
                    locdialog.InitialDirectory = Environment.CurrentDirectory;
                }

                if (locdialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    tbxFilePath.Text = locdialog.FileName;

                    if (File.Exists(tbxFilePath.Text))
                    {
                        btnUpload.Enabled = true;
                        _parent.UploadDir = Path.GetDirectoryName(locdialog.FileName);

                        FileUtil fileUtil = new FileUtil();
                        _filedata = fileUtil.GetFileData(tbxFilePath.Text);

                        if (tbxFileName.Text == "") tbxFileName.Text = _filedata.FileName + _filedata.FileExt;

                        _htmlResultBase += "<p class=\"success\">File Size: " + _filedata.FileSize.ToString() + " Bytes</p>";

                        _htmlTempResult += "</div>" + _resultHTML.HTMLEnd();

                        RefreshResults(_htmlResultBase + _htmlTempResult);
                        Application.DoEvents();
                    }

                    btnUpload.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileUpload.btnBrowse_Click", ex);
                MessageBox.Show(ex.Message, "frmFileUpload.btnBrowse_Click", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
                btnUpload.Enabled = false;
                btnBrowse.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            string procmsg = "";
            string procresult = "";
            long totalMS = 0;

            BackgroundWorker worker = (BackgroundWorker)sender;
            Stopwatch sw = new Stopwatch();

            try
            {
                sw.Start();

                if (_filedata.FileSize > 0 && _filedata.FileSize <= Convert.ToInt64(_main.MaxFileSize))
                {
                    long offset = 0;
                    string shardname = "";
                    string _filems = "";

                    MsgUtil msgUtil = new MsgUtil();

                    // query to get the shard name of a writable shard
                    Dictionary<string, string> shardparams = new Dictionary<string, string>();
                    ResInfo shardresult = msgUtil.ApiMethHelper("FileShard.GetShardName.base",
                                                                        _main.UserName,
                                                                        _main.Password,
                                                                        shardparams,
                                                                        _httpClient,
                                                                        _main.SvcUrl);

                    if (shardresult.RCode.ToUpper() == APIResult.OK)
                    {
                        sw.Stop();
                        // get the name of the writable shard to be used
                        shardname = shardresult.RVal;

                        totalMS += sw.ElapsedMilliseconds;
                        procmsg = "<p class=\"success\">ShardName: " + shardname + " retrieved from server: " + sw.ElapsedMilliseconds.ToString() + " MS</p>";
                        sw.Restart();

                        if (_action == ReplicaAction.Update)
                        {
                            // file update increments version number
                            int priorversion = Convert.ToInt32(_version);
                            priorversion++;
                            _version = priorversion.ToString();
                        }

                        // calculate total segments for file
                        FileUtil fileUtil = new FileUtil();
                        _totalSeg = fileUtil.GetFileSegCount(_filedata.FileSize, _segSize);

                        // create or update parent file metadata record; get new global ID in return
                        Dictionary<string, string> fileparams = new Dictionary<string, string>();
                        fileparams.Add(CommonFields.Action, _action);
                        fileparams.Add(CommonFields.rec_gid, _rec_gid);
                        fileparams.Add(CommonFields.row_ms, _row_ms);
                        fileparams.Add(FolderFields.FolderGID, _folder.FolderGID);
                        fileparams.Add(FolderFields.GroupGID, _folder.GroupGID);
                        fileparams.Add(FolderFields.FolderPath, _folderpath);
                        fileparams.Add(FileFields.FileName, _filedata.FileName);
                        fileparams.Add(FileFields.FileDescrip, tbxFileDescrip.Text);
                        fileparams.Add(FileFields.FileExt, _filedata.FileExt);
                        fileparams.Add(FileFields.FileSize, _filedata.FileSize.ToString());
                        fileparams.Add(FileFields.FileDate, _filedata.FileDate.ToString());
                        fileparams.Add(FileFields.FileHash, _filedata.FileHash);
                        fileparams.Add(FileFields.FileVersion, _version);
                        fileparams.Add(FileFields.SegSize, _segSize.ToString());
                        fileparams.Add(FileFields.TotalSeg, _totalSeg.ToString());
                        fileparams.Add(FileFields.ShardName, shardname);

                        string filemethod = "File.Save.base";
                        if (_action == ReplicaAction.Insert) filemethod = "File.New.base";

                        string methlist = msgUtil.CreateXMLMethod(filemethod, fileparams);
                        string reqmsg = msgUtil.CreateXMLRequest(_main.UserName, _main.Password, "", methlist);
                        string respmsg = msgUtil.HttpPost(_httpClient, _main.SvcUrl, reqmsg);

                        RespInfo respinfo = new RespInfo();
                        Dictionary<string, ResInfo> methresults = msgUtil.ReadResponseString(respinfo, respmsg);

                        ResInfo defres = msgUtil.GetResult(filemethod + "_" + MethReturn.Default, methresults);
                        ResInfo msres = msgUtil.GetResult(filemethod + "_" + "RowMS", methresults);

                        sw.Stop();
                        totalMS += sw.ElapsedMilliseconds;

                        if (defres.RCode.ToUpper() == APIResult.OK)
                        {
                            _rec_gid = defres.RVal;
                            _filems = msres.RVal;
                            procresult = APIResult.OK;
                            procmsg = "<p class=\"success\">File Record created/updated: " + sw.ElapsedMilliseconds.ToString() + " MS</p>";

                            sw.Restart();

                            // parse file into segments and save to a file shard table
                            bool segupload = true;
                            for (int x = 1; x <= _totalSeg; x++)
                            {
                                byte[] tmparray = fileUtil.ReadFileSegment(tbxFilePath.Text, offset);

                                if (tmparray.Length > 0)
                                {
                                    string segdata = Convert.ToBase64String(tmparray);

                                    Dictionary<string, string> segparams = new Dictionary<string, string>();
                                    segparams.Add(FolderFields.GroupGID, _folder.GroupGID);
                                    segparams.Add(FileFields.ShardName, shardname);
                                    segparams.Add(FileFields.FileGID, _rec_gid);
                                    segparams.Add(FileFields.FileVersion, _version);
                                    segparams.Add(FileFields.TotalSeg, _totalSeg.ToString());
                                    segparams.Add(FileFields.SegNum, x.ToString());
                                    segparams.Add(FileFields.SegSize, segdata.Length.ToString());
                                    segparams.Add(FileFields.SegData, segdata);

                                    ResInfo appendresult = msgUtil.ApiMethHelper("FileShard.New.base",
                                                                                _main.UserName,
                                                                                _main.Password,
                                                                                segparams,
                                                                                _httpClient,
                                                                                _main.SvcUrl);

                                    if (appendresult.RCode.ToUpper() == APIResult.OK)
                                    {
                                        offset += _segSize;
                                        decimal percent = ((decimal)x / (decimal)_totalSeg) * 100;
                                        int progress = Convert.ToInt32(percent);
                                        worker.ReportProgress(progress, "");
                                    }
                                    else
                                    {
                                        // error message
                                        sw.Stop();
                                        procresult = APIResult.Error;
                                        procmsg = "<p class=\"error\">File Segment Process Error: " + appendresult.RCode + " : " + appendresult.RVal + "</p>";
                                        worker.ReportProgress(100, procmsg);
                                        segupload = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    // error message
                                    sw.Stop();
                                    procresult = APIResult.Error;
                                    procmsg = "<p class=\"error\">File Segment Process Error: the segment size was zero bytes</p>";
                                    worker.ReportProgress(100, procmsg);
                                    segupload = false;
                                    break;
                                }
                            }

                            sw.Stop();
                            totalMS += sw.ElapsedMilliseconds;

                            // all file segments uploaded successfully
                            if (segupload)
                            {
                                procmsg = "<p class=\"success\">" + _totalSeg.ToString() + " segments uploaded: " + sw.ElapsedMilliseconds.ToString() + " MS</p>";
                                double KB = (_totalSeg * _segSize) / (double)1024;
                                double Sec = sw.ElapsedMilliseconds / (double)1000;
                                procmsg += "<p class=\"success\">" + Math.Round(KB / Sec, 2).ToString() + " KB/Sec</p>";
                                

                            }
                            else
                            {
                                // rollback process by removing the parent file record
                                Dictionary<string, string> methparams = new Dictionary<string, string>();
                                methparams.Add(CommonFields.rec_gid, _rec_gid);
                                methparams.Add(FileFields.FileName, tbxFileName.Text);
                                methparams.Add(FileFields.FileDescrip, "File removed due to problems with the upload of file segments");
                                methparams.Add(CommonFields.row_ms, _filems);

                                ResInfo methresult = msgUtil.ApiMethHelper("File.Remove.base",
                                                                            _main.UserName,
                                                                            _main.Password,
                                                                            methparams,
                                                                            _httpClient,
                                                                            _main.SvcUrl);

                                procresult = APIResult.Error;
                                procmsg = "<p class=\"error\">File Record Error: File removed due to problems with the upload of file segments.</p>";
                                worker.ReportProgress(100, procmsg);
                            }

                            worker.ReportProgress(100, procmsg);
                            sw.Restart();
                        }
                        else
                        {
                            procresult = APIResult.Error;
                            procmsg = "<p class=\"error\">File Record Error: " + defres.RCode + " : " + defres.RVal + "</p>";
                        }

                        sw.Restart();
                    }
                    else
                    {
                        // error: no writable shardname returned
                        sw.Stop();
                        procresult = APIResult.Error;
                        procmsg = "<p class=\"error\">Writable ShardName Error: " + shardresult.RCode + " : " + shardresult.RVal + "</p>";
                        worker.ReportProgress(100, procmsg);
                    }
                }
                else
                {
                    // error message
                    sw.Stop();
                    procresult = APIResult.Error;
                    procmsg = "<p class=\"error\">File Upload Process Error: the file is larger than the maximum file size limit.</p>";
                    worker.ReportProgress(0, procmsg);
                }
            }
            catch (Exception ex)
            {
                RemoteErrLog.LogException(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileUpload.btnUpload_Click", ex);
                MessageBox.Show(ex.Message, "btnUpload_Click", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            sw.Stop();
            totalMS += sw.ElapsedMilliseconds;

            if (procresult == APIResult.OK)
            {
                procmsg += "<p class=\"success\">Total Process Time: " + totalMS.ToString() + " MS</p>";
            }
            else
            {
                procmsg = "<p class=\"error\">File Upload Process Error: the file is larger than the maximum file size limit.</p>";
                RemoteErrLog.LogError(_main.UserName, _main.Password, _main.SvcUrl, "DGP Lattice", "frmFileUpload.btnUpload_Click", "File Upload Process Error", procmsg);
            }

            e.Result = procresult + "|" + procmsg;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();

            string progmsg = e.UserState.ToString();
            _htmlResultBase += progmsg;

            _htmlTempResult += "</div>" + _resultHTML.HTMLEnd();

            RefreshResults(_htmlResultBase + _htmlTempResult);
            Application.DoEvents();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;
            progressBar1.Update();

            string[] results = e.Result.ToString().Split('|');

            _procresult = results[0];
            _htmlResultBase += results[1];
            _htmlTempResult += "</div>" + _resultHTML.HTMLEnd();

            RefreshResults(_htmlResultBase + _htmlTempResult);

            // update contents of file grid
            _parent.FileSearch();

            btnBrowse.Enabled = false;
            btnUpload.Enabled = false;
            btnCancel.Enabled = false;

            if (_procresult == APIResult.OK && ckbxAutoClose.Checked) this.Close();
        }

        private void ClearForm()
        {
            tbxGlobalID.Text = "";
            tbxFileName.Text = "";
            tbxFileDescrip.Text = "";
            btnUpload.Enabled = false;

            _filedata = null;

            progressBar1.Value = 0;

            _htmlResultBase = _resultHTML.HTMLStart() + "<div class=\"titlediv\">Upload Process:</div><div class=\"innerdiv\">";
            ClearResults();
        }

        private void RefreshResults(string htmlResult)
        {
            brwsResults.DocumentText = "0";
            brwsResults.Document.OpenNew(true);
            brwsResults.Document.Write(htmlResult);
            brwsResults.Refresh();
        }

        private void ClearResults()
        {
            // clear results
            brwsResults.DocumentText = "0";
            brwsResults.Document.OpenNew(true);
            ResultHTML resultHTML = new ResultHTML();
            string resHTML = resultHTML.HTMLStart() + resultHTML.HTMLEnd();
            brwsResults.Document.Write(resHTML);
            brwsResults.Refresh();
        }

    }
}
