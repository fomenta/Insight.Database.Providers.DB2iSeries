using IBM.Data.DB2.iSeries;
using Insight.Database;
using Insight.Database.Providers.DB2iSeries;
using Insight.Tests.DB2iSeries.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Insight.Tests
{
    [TestFixture]
    public class DB2iSeriesTests
    {
        private iDB2Connection Database;
        private string ConnectionString;

        [OneTimeSetUp]
        public void SetUp()
        {
            DB2InsightDbProvider.RegisterProvider();
            ConnectionString = @"Driver={SQL Server};Server=.\SQLEXPRESS2016;Database=Demo;Trusted_Connection=Yes";
            var filePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Downloads\TestInsightDatabase\Demo.dsn");
            Database = new iDB2Connection($"FileDSN={filePath}");
        }


        [Test]
        public void Query_QuerySql()
        {
            IList<Beer> beers = Database.QuerySql<Beer>("SELECT * FROM Beer WHERE Country = @Country", new { Country = "CO" });
            Assert.AreEqual(1, beers.Count, "beer.Count");
            var beer = beers.First();
            Assert.AreEqual(1, beer.ID, "beer.ID");
            Assert.AreEqual("Club colombia", beer.Name, "beer.Name");
        }


        [Test]
        public void Query_QuerySql_NoInsight()
        {
            string queryString = "SELECT * FROM Beer WHERE Country = ?";
            iDB2Command command = new iDB2Command(queryString, Database);
            command.Parameters.Add("Country", iDB2DbType.iDB2VarChar, 3).Value = "CO";
            Database.Open();
            using (iDB2DataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                DataTable dt = new DataTable("Beer");
                dt.Load(reader);
            }
        }


        [Test]
        public void NamedParametersAreConvertedToPositionalParameters()
        {
            var c = new System.Data.Odbc.OdbcConnection(ConnectionString);
            dynamic i = c.QuerySql("SELECT p=@p, q=@q, r=@p", new { p = 5, q = 9 }).First();
            Assert.AreEqual(5, i.p);
            Assert.AreEqual(9, i.q);
            Assert.AreEqual(5, i.r);
        }
    }
}
