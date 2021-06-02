using System;
using System.Collections.Generic;
using System.Configuration;

using ApiUtil;
using ApiUtil.DataClasses;

namespace DGPMetricsAPI.LatticeMetrics
{
    public class LatticeMetrics_switch : IMethSwitch
    {
        string _connstr;

        public LatticeMetrics_switch()
        {
            _connstr = ConfigurationManager.AppSettings["SysMetrics"].ToString();
        }

        public string CallMethod(UserInfo userinfo, string methodname, Dictionary<string, string> methparams)
        {
            string methXml = "";
            LatticeMetrics_mapper latticeMetrics_Mapper = new LatticeMetrics_mapper(_connstr);

            switch (methodname)
            {
                case "LatticeMetrics.GetPageSize.base":
                    methXml = latticeMetrics_Mapper.GetPageSize(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.GetByID.base":
                    methXml = latticeMetrics_Mapper.GetByID(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.GetCount.base":
                    methXml = latticeMetrics_Mapper.GetCount(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.GetSearch.base":
                    methXml = latticeMetrics_Mapper.GetSearch(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.GetAll.base":
                    methXml = latticeMetrics_Mapper.GetAll(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.New.base":
                    methXml = latticeMetrics_Mapper.AddLatticeMetric(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.Delete.base":
                    methXml = latticeMetrics_Mapper.Delete(userinfo, methodname, methparams);
                    break;

                case "LatticeMetrics.Server.base":
                    methXml = latticeMetrics_Mapper.AddServerMetric(userinfo, methodname, methparams);
                    break;

                default:
                    throw new Exception("Error: Method [ " + methodname + " ] was not found in the LatticeMetrics API.");
            }

            return methXml;
        }
    }
}
