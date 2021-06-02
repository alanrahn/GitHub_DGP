using System;
using System.Collections.Generic;
using System.Configuration;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPMetricsAPI.TestResults
{
    public class TestResults_switch : IMethSwitch
    {
        string _connstr;

        public TestResults_switch()
        {
            _connstr = ConfigurationManager.AppSettings["SysMetrics"].ToString();
        }

        public string CallMethod(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string methXml = "";
            TestResults_mapper testResults_Mapper = new TestResults_mapper(_connstr);

            switch (methodname)
            {
                case "TestResult.GetPageSize.base":
                    methXml = testResults_Mapper.GetPageSize(userinfo, methodname, methparams);
                    break;

                case "TestResult.GetByID.base":
                    methXml = testResults_Mapper.GetByID(userinfo, methodname, methparams);
                    break;

                case "TestResult.GetCount.base":
                    methXml = testResults_Mapper.GetCount(userinfo, methodname, methparams);
                    break;

                case "TestResult.GetSearch.base":
                    methXml = testResults_Mapper.GetSearch(userinfo, methodname, methparams);
                    break;

                case "TestResult.GetEvalInfo.base":
                    methXml = testResults_Mapper.GetEvalInfo(userinfo, methodname, methparams);
                    break;

                case "TestResult.GetAll.base":
                    methXml = testResults_Mapper.GetAll(userinfo, methodname, methparams);
                    break;

                case "TestResult.New.base":
                    methXml = testResults_Mapper.AddTestResult(userinfo, methodname, methparams);
                    break;

                case "TestResult.Delete.base":
                    methXml = testResults_Mapper.Delete(userinfo, methodname, methparams);
                    break;

                default:
                    throw new Exception("Error: Method [ " + methodname + " ] was not found in the TestResult API.");
            }

            return methXml;
        }
    }
}
