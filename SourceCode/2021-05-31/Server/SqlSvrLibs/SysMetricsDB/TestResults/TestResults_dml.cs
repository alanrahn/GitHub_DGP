using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using SqlSvrUtil;
using ApiUtil;

namespace SysMetricsDB
{
    public class TestResults_dml
    {
        string _connstr;

        public TestResults_dml(string connStr)
        {
            _connstr = connStr;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTable GetByID(string recGID)
        {
            DataTable dtresult = new DataTable();

            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = "select *" +
                                    " from TestResults" +
                                    " where row_gid = @row_gid;";

                sqlCmd.Parameters.Add("@row_gid", SqlDbType.VarChar, 50).Value = recGID;

                DBUtil dBUtil = new DBUtil();
                dtresult = dBUtil.Read(sqlCmd, _connstr);
            }

            return dtresult;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTable GetAll()
        {
            DataTable dtresult = new DataTable();

            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = "select top 10 *" +
                                    " from TestResults" +
                                    " order by ServerTime DESC;";

                DBUtil dBUtil = new DBUtil();
                dtresult = dBUtil.Read(sqlCmd, _connstr);
            }

            return dtresult;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetCount(string eval, string apiMethod)
        {
            string strresult = "";

            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = "Select count(*) AS Count" +
                                    " From TestResults";

                string linkcmd = " Where";
                // search filter values are optional
                if (eval != null && eval != "")
                {
                    sqlCmd.CommandText += linkcmd + " Eval Like @Eval";
                    sqlCmd.Parameters.Add("@Eval", SqlDbType.VarChar, 10).Value = eval + "%";
                    linkcmd = " And";
                }

                if (apiMethod != null && apiMethod != "")
                {
                    sqlCmd.CommandText += linkcmd + " APIMethod Like @APIMethod";
                    sqlCmd.Parameters.Add("@APIMethod", SqlDbType.VarChar, 50).Value = apiMethod + "%";
                    linkcmd = " And";
                }

                DBUtil dBUtil = new DBUtil();
                strresult = dBUtil.ReadValue(sqlCmd, _connstr);
            }

            return strresult;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTable GetSearch(string pageNum, string pageSize, string eval, string apiMethod)
        {
            DataTable dtresult = new DataTable();

            using (SqlCommand sqlCmd = new SqlCommand())
            {

                int _pagesize = Convert.ToInt32(pageSize);
                int _pagenum = Convert.ToInt32(pageNum);
                if (_pagenum < 0) _pagenum = 0;
                int _offset = _pagenum * _pagesize;

                sqlCmd.CommandText = "Select TestRun, Eval, APIMethod, AuthCode, AuthInfo, ExpAuthCode, ClientMS, NetworkMS, ServerMS, UserName, CompName, TimeStamp, ReqSize, RespSize, SvcURL, FileName, row_gid, row_id" +
                                    " From TestResults";

                // search filter values are optional
                string linkcmd = " Where";
                string orderby = " Order By TimeStamp DESC";

                // search filter values are optional
                if (eval != null && eval != "")
                {
                    sqlCmd.CommandText += linkcmd + " Eval Like @Eval";
                    sqlCmd.Parameters.Add("@Eval", SqlDbType.VarChar, 10).Value = eval + "%";
                    linkcmd = " And";
                    orderby += ", AppName ASC";
                }

                if (apiMethod != null && apiMethod != "")
                {
                    sqlCmd.CommandText += linkcmd + " APIMethod Like @APIMethod";
                    sqlCmd.Parameters.Add("@APIMethod", SqlDbType.VarChar, 50).Value = apiMethod + "%";
                    linkcmd = " And";
                    orderby += ", FormName ASC";
                }

                sqlCmd.CommandText += orderby;

                sqlCmd.CommandText += " Offset " + _offset.ToString() + " Rows" +
                                    " Fetch Next " + pageSize + " Rows Only";

                DBUtil dBUtil = new DBUtil();
                dtresult = dBUtil.Read(sqlCmd, _connstr);
            }

            return dtresult;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetEvalInfo(string row_gid)
        {
            string dtresult = "";

            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = "select EvalInfo" +
                                    " from TestResults" +
                                    " where row_gid = @rec_gid;";

                sqlCmd.Parameters.Add("@rec_gid", SqlDbType.VarChar, 50).Value = row_gid;

                DBUtil dBUtil = new DBUtil();
                dtresult = dBUtil.ReadValue(sqlCmd, _connstr);
            }

            return dtresult;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Write(string row_gid,
                                string testrun,
                                string eval,
                                string evalinfo,
                                string apimethod,
                                string authcode,
                                string authinfo,
                                string expauthcode,
                                string clientms,
                                string networkms,
                                string serverms,
                                string username,
                                string compname,
                                string timestamp,
                                string reqsize,
                                string respsize,
                                string svcurl,
                                string filename)
        {
            string strresult = "";

            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = "Insert TestResults (row_gid, TestRun, Eval, EvalInfo, APIMethod, AuthCode, AuthInfo, ExpAuthCode, ClientMS, NetworkMS, ServerMS, UserName, CompName, TimeStamp, ReqSize, RespSize, SvcURL, FileName)" +
                                    " Values (@row_gid, @TestRun, @Eval, @EvalInfo, @APIMethod, @AuthCode, @AuthInfo, @ExpAuthCode, @ClientMS, @NetworkMS, @ServerMS, @UserName, @CompName, @TimeStamp, @ReqSize, @RespSize, @SvcURL, @FileName);";

                sqlCmd.Parameters.Add("@row_gid", SqlDbType.VarChar, 50).Value = row_gid;
                sqlCmd.Parameters.Add("@TestRun", SqlDbType.VarChar, 100).Value = testrun;
                sqlCmd.Parameters.Add("@Eval", SqlDbType.VarChar, 10).Value = eval;
                sqlCmd.Parameters.Add("@EvalInfo", SqlDbType.VarChar, 5000).Value = evalinfo;
                sqlCmd.Parameters.Add("@APIMethod", SqlDbType.VarChar, 50).Value = apimethod;
                sqlCmd.Parameters.Add("@AuthCode", SqlDbType.VarChar, 10).Value = authcode;
                sqlCmd.Parameters.Add("@AuthInfo", SqlDbType.VarChar, 500).Value = authinfo;
                sqlCmd.Parameters.Add("@ExpAuthCode", SqlDbType.VarChar, 10).Value = expauthcode;
                sqlCmd.Parameters.Add("@ClientMS", SqlDbType.Float).Value = Convert.ToDouble(clientms);
                sqlCmd.Parameters.Add("@NetworkMS", SqlDbType.Float).Value = Convert.ToDouble(networkms);
                sqlCmd.Parameters.Add("@ServerMS", SqlDbType.Float).Value = Convert.ToDouble(serverms);
                sqlCmd.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = username;
                sqlCmd.Parameters.Add("@CompName", SqlDbType.VarChar, 50).Value = compname;
                sqlCmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime2).Value = Convert.ToDateTime(timestamp);
                sqlCmd.Parameters.Add("@ReqSize", SqlDbType.VarChar, 10).Value = reqsize;
                sqlCmd.Parameters.Add("@RespSize", SqlDbType.VarChar, 10).Value = respsize;
                sqlCmd.Parameters.Add("@SvcUrl", SqlDbType.VarChar, 50).Value = svcurl;
                sqlCmd.Parameters.Add("@FileName", SqlDbType.VarChar, 50).Value = filename;

                DBUtil dBUtil = new DBUtil();
                strresult = dBUtil.Write(sqlCmd, _connstr);
            }

            return strresult;
        }

        /// <summary>
        /// Hard delete
        /// </summary>
        public string Delete(string row_gid)
        {
            string strresult = "";

            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.CommandText = "Delete FROM TestResults" +
                                    " WHERE  row_gid = @row_gid;";

                sqlCmd.Parameters.Add("@row_gid", SqlDbType.VarChar, 50).Value = row_gid;

                DBUtil dBUtil = new DBUtil();
                strresult = dBUtil.Write(sqlCmd, _connstr);
            }

            return strresult;
        }
    }
}
