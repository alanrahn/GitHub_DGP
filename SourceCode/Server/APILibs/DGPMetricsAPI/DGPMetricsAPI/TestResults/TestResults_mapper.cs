using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using ApiUtil;
using ApiUtil.DataClasses;
using SysMetricsDB;

namespace DGPMetricsAPI.TestResults
{
    public class TestResults_mapper
    {
        string _connstr;
        string _appname;
        string _pagesize = "50";

        public TestResults_mapper(string connStr)
        {
            _connstr = connStr;
            _appname = ConfigurationManager.AppSettings["WebSvcName"].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetPageSize(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.Text, _pagesize);
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetByID(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            DataTable results = new DataTable();
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                string SchemaFlag = msgUtil.GetParamValue(CommonFields.SchemaFlag, methparams);
                string row_gid = msgUtil.GetParamValue(CommonFields.row_gid, methparams);

                // test for required input parameters
                bool reqFields = true;
                string reqFieldNames = "";

                if (SchemaFlag == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing SchemaFlag; ";
                }
                else
                {
                    var isBoolean = bool.TryParse(SchemaFlag, out bool n);
                    if (!isBoolean)
                    {
                        reqFields = false;
                        reqFieldNames += "SchemaFlag must be a bool; ";
                    }
                }
                if (row_gid == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing row_gid; ";
                }

                if (reqFields)
                {
                    bool schemaflag = Convert.ToBoolean(SchemaFlag);
                    TestResults_dml testResults_Dml = new TestResults_dml(_connstr);
                    results = testResults_Dml.GetByID(row_gid);

                    if (results.Rows.Count > 0)
                    {
                        CmnUtil cmnUtil = new CmnUtil();
                        string tblxml = cmnUtil.TableToXml(results, schemaflag);
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.DataTable, tblxml);
                    }
                    else
                    {
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Empty, APIData.Text, "");
                    }
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + ": " + reqFieldNames);
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetCount(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                string eval = msgUtil.GetParamValue(LogFields.Eval, methparams);
                string apimethod = msgUtil.GetParamValue(LogFields.APIMethod, methparams);

                TestResults_dml testResults = new TestResults_dml(_connstr);
                string rowcount = testResults.GetCount(eval, apimethod);

                if (rowcount != null && rowcount != "")
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.Text, rowcount);
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Empty, APIData.Text, "");
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetSearch(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            DataTable results = new DataTable();
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                // pagination values
                string SchemaFlag = msgUtil.GetParamValue(CommonFields.SchemaFlag, methparams);
                string pagenum = msgUtil.GetParamValue(CommonFields.PageNum, methparams);

                // search values (optional)
                string eval = msgUtil.GetParamValue(LogFields.Eval, methparams);
                string apimethod = msgUtil.GetParamValue(LogFields.APIMethod, methparams);

                // test for required input parameters
                bool reqFields = true;
                string reqFieldNames = "";

                if (SchemaFlag == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing SchemaFlag; ";
                }
                else
                {
                    var isBoolean = bool.TryParse(SchemaFlag, out bool n);
                    if (!isBoolean)
                    {
                        reqFields = false;
                        reqFieldNames += "SchemaFlag must be a bool; ";
                    }
                }
                if (pagenum == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing PageNum; ";
                }
                else
                {
                    var isNumeric = int.TryParse(pagenum, out int n);
                    if (!isNumeric)
                    {
                        reqFields = false;
                        reqFieldNames += "PageNum must be an int; ";
                    }
                }

                if (reqFields)
                {
                    bool schemaflag = Convert.ToBoolean(SchemaFlag);
                    TestResults_dml testResults = new TestResults_dml(_connstr);
                    results = testResults.GetSearch(pagenum, _pagesize, eval, apimethod);

                    if (results.Rows.Count > 0)
                    {
                        CmnUtil cmnUtil = new CmnUtil();
                        string tblxml = cmnUtil.TableToXml(results, schemaflag);
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.DataTable, tblxml);
                    }
                    else
                    {
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Empty, APIData.Text, "");
                    }
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + ": " + reqFieldNames);
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetEvalInfo(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            string results = "";
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                string row_gid = msgUtil.GetParamValue(CommonFields.row_gid, methparams);

                bool reqFields = true;
                string reqFieldNames = "";

                if (row_gid == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing row_gid;";
                }

                if (reqFields)
                {
                    TestResults_dml testResults = new TestResults_dml(_connstr);
                    results = testResults.GetEvalInfo(row_gid);

                    if (results != null && results != "")
                    {
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.Text, results);
                    }
                    else
                    {
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, "Problem querying for EvalInfo data.");
                    }
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + ": " + reqFieldNames);
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetAll(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            DataTable results = new DataTable();
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                string SchemaFlag = msgUtil.GetParamValue(CommonFields.SchemaFlag, methparams);

                // test for required input parameters
                bool reqFields = true;
                string reqFieldNames = "";

                if (SchemaFlag == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing SchemaFlag; ";
                }
                else
                {
                    var isBoolean = bool.TryParse(SchemaFlag, out bool n);
                    if (!isBoolean)
                    {
                        reqFields = false;
                        reqFieldNames += "SchemaFlag must be a bool; ";
                    }
                }

                if (reqFields)
                {
                    bool schemaflag = Convert.ToBoolean(SchemaFlag);
                    TestResults_dml testResults_Dml = new TestResults_dml(_connstr);
                    results = testResults_Dml.GetAll();

                    if (results.Rows.Count > 0)
                    {
                        CmnUtil cmnUtil = new CmnUtil();
                        string tblxml = cmnUtil.TableToXml(results, schemaflag);
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.DataTable, tblxml);
                    }
                    else
                    {
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Empty, APIData.Text, "");
                    }
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + ": " + reqFieldNames);
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /* --------------------------------------------------------------------------------------------- */
        /* --------------------------------------------------------------------------------------------- */
        /* --------------------------------------------------------------------------------------------- */

        /// <summary>
        /// 
        /// </summary>
        public string AddTestResult(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                string resrecs = msgUtil.GetParamValue("ResultRecords", methparams);
                
                bool reqFields = true;
                string reqFieldNames = "";

                if (resrecs == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing resrecs;";
                }

                if (reqFields)
                {
                    CmnUtil cmnUtil = new CmnUtil();
                    DataTable resRecs = cmnUtil.XmlToTable(resrecs);
                    int rowCount = 0;
                    if (resRecs.Rows.Count > 0)
                    {
                        rowCount = resRecs.Rows.Count;
                        int addCount = 0;
                        TestResults_dml testResults_Dml = new TestResults_dml(_connstr);

                        foreach (DataRow dr in resRecs.Rows)
                        {
                            string row_gid = cmnUtil.GetNewGID();
                            string TestRun = dr[TestResultsFields.TestRun].ToString();
                            string Eval = dr[TestResultsFields.Eval].ToString();
                            string EvalInfo = dr[TestResultsFields.EvalInfo].ToString();
                            string APIMethod = dr[TestResultsFields.APIMethod].ToString();
                            string AuthCode = dr[TestResultsFields.AuthCode].ToString();
                            string AuthInfo = dr[TestResultsFields.AuthInfo].ToString();
                            string ExpAuthCode = dr[TestResultsFields.ExpAuthCode].ToString();
                            string ClientMS = dr[TestResultsFields.ClientMS].ToString();
                            string NetworkMS = dr[TestResultsFields.NewtorkMS].ToString();
                            string ServerMS = dr[TestResultsFields.ServerMS].ToString();
                            string UserName = dr[TestResultsFields.UserName].ToString();
                            string CompName = dr[TestResultsFields.CompName].ToString();
                            string TimeStamp = dr[TestResultsFields.TimeStamp].ToString();
                            string ReqSize = dr[TestResultsFields.ReqSize].ToString();
                            string RespSize = dr[TestResultsFields.RespSize].ToString();
                            string SvcURL = dr[TestResultsFields.SvcURL].ToString();
                            string FileName = dr[TestResultsFields.FileName].ToString();

                            string tmpcode = testResults_Dml.Write(row_gid,
                                                                    TestRun, 
                                                                    Eval, 
                                                                    EvalInfo, 
                                                                    APIMethod, 
                                                                    AuthCode, 
                                                                    AuthInfo, 
                                                                    ExpAuthCode, 
                                                                    ClientMS, 
                                                                    NetworkMS, 
                                                                    ServerMS,
                                                                    UserName,
                                                                    CompName,
                                                                    TimeStamp,
                                                                    ReqSize,
                                                                    RespSize,
                                                                    SvcURL,
                                                                    FileName);

                            if (tmpcode == APIResult.OK)
                            {
                                addCount++;
                            }
                        }

                        string procresult = "";
                        if (addCount == rowCount)
                        {
                            procresult = APIResult.OK;
                        }
                        else
                        {
                            procresult = APIResult.Error + ": added " + addCount.ToString() + " out of " + rowCount.ToString() + " records";
                        }

                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.Text, procresult);
                    }
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + ": " + reqFieldNames);
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Delete(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string resultxml = "";
            MsgUtil msgUtil = new MsgUtil();

            try
            {
                string row_gid = msgUtil.GetParamValue(CommonFields.row_gid, methparams);

                bool reqFields = true;
                string reqFieldNames = "";

                if (row_gid == "")
                {
                    reqFields = false;
                    reqFieldNames += "Missing row_gid;";
                }

                if (reqFields)
                {
                    TestResults_dml results_Dml = new TestResults_dml(_connstr);
                    string tmpcode = results_Dml.Delete(row_gid);

                    if (tmpcode == APIResult.OK)
                    {
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.OK, APIData.Text, tmpcode);
                    }
                    else
                    {
                        string errmsg = "A problem occurred deleteing the TestResult record.";
                        resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, errmsg);
                        ServerErrLog.LogError(userinfo.UserName, _appname, methodname, "Log delete error.", errmsg);
                    }
                }
                else
                {
                    resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + ": " + reqFieldNames);
                }
            }
            catch (Exception ex)
            {
                resultxml = msgUtil.CreateXMLResult(methodname, MethReturn.Default, APIResult.Error, APIData.Text, methodname + " : " + ex.Message);
                ServerErrLog.LogException(userinfo.UserName, _appname, methodname, ex);
            }

            return resultxml;
        }

    }
}
