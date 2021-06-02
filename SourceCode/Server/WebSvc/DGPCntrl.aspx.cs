using System;
using System.Text;
using System.Web;
using System.Configuration;
using System.Diagnostics;

using ApiUtil;
using ApiUtil.DataClasses;
using SysInfoDB;
using SysMetricsDB;

namespace DGPWebSvc
{
    public partial class DGPCntrl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch servertime = new Stopwatch();
            servertime.Start();

            MsgUtil msgutil = new MsgUtil();
            ReqInfo reqInfo = new ReqInfo();
            UserInfo userInfo = new UserInfo();

            string respMsg = string.Empty;
            string _appname = ConfigurationManager.AppSettings["WebSvcName"].ToString();

            try
            {
                if (ConfigurationManager.AppSettings["LocState"].ToString() == LocState.Online)
                {
                    // check request message content type
                    if (HttpContext.Current.Request.ContentType == "text/xml")
                    {
                        long reqMsgSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxReqMsgKB"].ToString()) * 1024;

                        // check request message size
                        if (HttpContext.Current.Request.ContentLength <= reqMsgSize)
                        {
                            string ttlcheckflag = ConfigurationManager.AppSettings["TTLCheckFlag"].ToString();
                            int ttlsec = Convert.ToInt32(ConfigurationManager.AppSettings["TTLMS"]);
                            string usercacheflag = ConfigurationManager.AppSettings["UserCacheFlag"].ToString();
                            string ratelimitflag = ConfigurationManager.AppSettings["RateLimitFlag"].ToString();
                            int maxmethbatch = Convert.ToInt32(ConfigurationManager.AppSettings["MaxMethBatch"]);
                            string maxrespmsgkb = ConfigurationManager.AppSettings["MaxRespMsgKB"].ToString();
                            int maxfailedlogin = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedLogin"]);
                            string sysinfoconnstr = ConfigurationManager.AppSettings["SysInfo"].ToString();
                            string svckeyversion = ConfigurationManager.AppSettings["SvcKeyVersion"].ToString();
                            string svckey = ConfigurationManager.AppSettings[svckeyversion].ToString();

                            // max request message size is also used in the XML Reader
                            msgutil.ReadRequestStream(reqInfo, Request.InputStream, reqMsgSize);
                            reqInfo.ClientIP = msgutil.GetClientIPAddress();

                            MsgPipeline msgpipe = new MsgPipeline(sysinfoconnstr);

                            string resultMsg = msgpipe.ReadReqMsg(reqInfo, ref userInfo, HttpContext.Current, new DGPAPISwitch(), ttlcheckflag, ttlsec, usercacheflag, ratelimitflag, maxmethbatch, maxfailedlogin, svckey);

                            // test size of response message compared to the max limit
                            int maxRespBytes = Convert.ToInt32(maxrespmsgkb) * 1024;
                            if (resultMsg.Length <= maxRespBytes)
                            {
                                servertime.Stop();
                                respMsg = msgutil.CreateXMLResponse(reqInfo.UserName, reqInfo.ID, userInfo.AuthState, LocState.Online, servertime.Elapsed.TotalMilliseconds.ToString(), resultMsg);
                            }
                            else
                            {
                                servertime.Stop();
                                string errMsg = msgutil.CreateXMLResult(MethReturn.MethodError, MethReturn.Default, APIResult.Error, APIData.Text, "The size of the response message exceeded the maximum " + maxrespmsgkb + " KB.");
                                respMsg = msgutil.CreateXMLResponse(reqInfo.UserName, reqInfo.ID, AuthState.Exceeded, LocState.Online, servertime.Elapsed.TotalMilliseconds.ToString(), errMsg);
                            }
                        }
                        else
                        {
                            servertime.Stop();
                            string errMsg = msgutil.CreateXMLResult(MethReturn.MethodError, MethReturn.Default, APIResult.Error, APIData.Text, "The size of the request message exceeded the maximum " + reqMsgSize.ToString() + " Bytes.");
                            respMsg = msgutil.CreateXMLResponse(reqInfo.UserName, reqInfo.ID, AuthState.Content, "The size of the request message exceeded the maximum.", "0", errMsg);
                        }
                    }
                    else
                    { 
                        servertime.Stop();
                        string errMsg = msgutil.CreateXMLResult(MethReturn.MethodError, MethReturn.Default, APIResult.Error, APIData.Text, "Only text/xml request content type is allowed.");
                        respMsg = msgutil.CreateXMLResponse(reqInfo.UserName, reqInfo.ID, AuthState.Content, "Only text/xml request content type is allowed.", "0", errMsg);
                    }
                }
                else
                {
                    servertime.Stop();
                    string errMsg = msgutil.CreateXMLResult(MethReturn.MethodError, MethReturn.Default, APIResult.Error, APIData.Text, "The web service is currently offline.");
                    respMsg = msgutil.CreateXMLResponse(reqInfo.UserName, reqInfo.ID, LocState.Offline, "The web service is currently offline.", "0", errMsg);
                }
            }
            catch (Exception ex)
            {
                servertime.Stop();
                ServerErrLog.LogException(reqInfo.UserName, _appname, "DGPCntrl.Page_Load", ex);
                string errMsg = msgutil.CreateXMLResult(MethReturn.MethodError, MethReturn.Default, APIResult.Exception, APIData.Text, ex.Message);
                respMsg = msgutil.CreateXMLResponse(reqInfo.UserName, reqInfo.ID, AuthState.Error, "DGPCtrl.Page_Load exception." + ex.Message, "0", errMsg);
            }
            finally
            {
                Response.ContentType = "text/xml";
                Response.AppendHeader("Content-Disposition", "filename=respmsg.xml");
                Response.AppendHeader("Content-Length", Encoding.UTF8.GetBytes(respMsg).Length.ToString());
                Response.Charset = "utf-8";
                Response.Write(respMsg);

                try
                {
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest(); // https://support.microsoft.com/en-us/kb/312629
                }
                catch { }
            }
        }
    }
}