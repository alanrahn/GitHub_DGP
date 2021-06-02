using System;
using System.Data.SqlClient;

using SqlSvrUtil;

namespace SysMetricsDB
{
    public class TestResults_ddl : ITbl
    {
        public TestResults_ddl()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string ScanTable(string DBName, string SvrConnStr)
        {
            string scanResult = "";
            SqlCommand sqlCmd = new SqlCommand();

            try
            {
                sqlCmd.CommandText = "USE [" + DBName + "];\n" +
                                    " SET ANSI_NULLS ON;\n" +
                                    " SET QUOTED_IDENTIFIER ON;\n" +
                                    // table
                                    " IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestResults]') AND type in (N'U'))\n" +
                                    " BEGIN\n" +
                                    " CREATE TABLE[dbo].[TestResults](\n" +

                                    " [row_id] [int] IDENTITY(1000,1) NOT NULL,\n" +
                                    " [row_gid] [varchar](50) NOT NULL,\n" +

                                    " [TestRun] [varchar](100) NOT NULL,\n" +
                                    " [Eval] [varchar](10) NOT NULL,\n" +
                                    " [EvalInfo] [varchar](5000) NOT NULL,\n" +
                                    " [APIMethod] [varchar](50) NOT NULL,\n" +
                                    " [AuthCode] [varchar](10) NOT NULL,\n" +
                                    " [AuthInfo] [varchar](500) NOT NULL,\n" +
                                    " [ExpAuthCode] [varchar](10) NOT NULL,\n" +
                                    " [ClientMS] [float] NOT NULL,\n" +
                                    " [NetworkMS] [float] NOT NULL,\n" +
                                    " [ServerMs] [float] NOT NULL,\n" +
                                    " [UserName] [varchar](50) NOT NULL,\n" +
                                    " [CompName] [varchar](50) NOT NULL,\n" +
                                    " [TimeStamp] [datetime2](7) NOT NULL,\n" +
                                    " [ReqSize] [varchar](10) NOT NULL,\n" +
                                    " [RespSize] [varchar](10) NOT NULL,\n" +
                                    " [SvcURL] [varchar](50) NOT NULL,\n" +
                                    " [FileName] [varchar](50) NOT NULL,\n" +
                                    " [ServerTime] [datetime2](7) NOT NULL,\n" +

                                    // PK index
                                    " CONSTRAINT[PK_TestResults] PRIMARY KEY CLUSTERED\n([row_id] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]\n" +
                                    " END;\n" +
                                    " SET ANSI_PADDING ON;\n" +

                                    // DGP index
                                    " IF NOT EXISTS(SELECT* FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TestResults]') AND name = N'rowgid_idx')\n" +
                                    " BEGIN\n" +
                                    " CREATE UNIQUE NONCLUSTERED INDEX[rowgid_idx] ON[dbo].[TestResults]([row_gid] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY];\n" +
                                    " END;\n" +

                                    // schema index
                                    " IF NOT EXISTS(SELECT* FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TestResults]') AND name = N'TestRun_idx')\n" +
                                    " BEGIN\n" +
                                    " CREATE NONCLUSTERED INDEX[TestRun_idx] ON[dbo].[TestResults]([TestRun] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY];\n" +
                                    " END;\n" +

                                    // schema index
                                    " IF NOT EXISTS(SELECT* FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TestResults]') AND name = N'Eval_idx')\n" +
                                    " BEGIN\n" +
                                    " CREATE NONCLUSTERED INDEX[Eval_idx] ON[dbo].[TestResults]([Eval] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY];\n" +
                                    " END;\n" +

                                    // schema index
                                    " IF NOT EXISTS(SELECT* FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[TestResults]') AND name = N'APIMethod_idx')\n" +
                                    " BEGIN\n" +
                                    " CREATE NONCLUSTERED INDEX[APIMethod_idx] ON[dbo].[TestResults]([APIMethod] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY];\n" +
                                    " END;\n" +

                                    // default
                                    " IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TestResults_ServerTime]') AND type = 'D')\n" +
                                    " BEGIN\n" +
                                    " ALTER TABLE[dbo].[TestResults] ADD CONSTRAINT[DF_TestResults_ServerTime]  DEFAULT(getutcdate()) FOR[ServerTime]\n" +
                                    " END;\n";

                // add new columns, indexes and defaults below (table schema treated as immutable, append-only):

                DBUtil dBUtil = new DBUtil();
                int rowsaff = dBUtil.Execute(sqlCmd, SvrConnStr);

                if (rowsaff == -1)
                {
                    // success
                    scanResult = "<p class=\"success\">TestResults Table Schema : OK</p>";
                }
                else
                {
                    // error
                    scanResult = "<p class=\"error\">TestResults Table Schema : ERROR : An unknown problem occurred creating or updating the table.</p>";
                }
            }
            catch (Exception ex)
            {
                scanResult = "<p class=\"error\">TestResults Table Schema : EXCEPTION : " + ex.Message + "</p>";
            }

            return scanResult;
        }
    }
}
