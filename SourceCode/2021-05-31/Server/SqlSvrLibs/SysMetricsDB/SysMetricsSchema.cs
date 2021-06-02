using SqlSvrUtil;

namespace SysMetricsDB
{
    public static class SysMetricsMaster
    {
        public const string SourceDB = "dgp_master_sysmetrics";
    }

    public class SysMetricsSchema : IDB
    {
        public SysMetricsSchema()
        {

        }

        public string ScanDB(string dbName, string connStr)
        {
            string scanSummary = "";

            TestResults_ddl testResults_Ddl = new TestResults_ddl();
            scanSummary += testResults_Ddl.ScanTable(dbName, connStr);

            LatticeMetrics_ddl latticeMetrics_Ddl = new LatticeMetrics_ddl();
            scanSummary += latticeMetrics_Ddl.ScanTable(dbName, connStr);

            DGPErrors_ddl dgpErrors_Ddl = new DGPErrors_ddl();
            scanSummary += dgpErrors_Ddl.ScanTable(dbName, connStr);

            AutoWorkLog_ddl autoWorkLog_Ddl = new AutoWorkLog_ddl();
            scanSummary += autoWorkLog_Ddl.ScanTable(dbName, connStr);

            ServerMetrics_ddl serverMetrics_Ddl = new ServerMetrics_ddl();
            scanSummary += serverMetrics_Ddl.ScanTable(dbName, connStr);

            return scanSummary;
        }
    }
}
