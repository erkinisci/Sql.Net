using System.Configuration;
using System.Data;
using System.Data.OleDb;
using NUnit.Framework;
using Reeb.SqlOM;
using Reeb.SqlOM.Render;

namespace Tests
{
	[TestFixture]
	public class SqlOM_OracleOLEDB : SqlOmTestBase
	{
		ISqlOmRenderer renderer = new OracleRenderer();
		public SqlOM_OracleOLEDB()
		{
		}

		protected override IDbConnection CreateConnection()
		{
			return new OleDbConnection(ConfigurationSettings.AppSettings["OracleOLEDB"]);
		}

		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new OleDbDataAdapter();
		}

		protected override ISqlOmRenderer Renderer
		{
			get { return renderer; }
		}

		protected override IDataParameter CreateParameter(string name, object val)
		{
			return new OleDbParameter(name, val);
		}

		protected override string GetParameterName(string name)
		{
			return "?";
		}

	}
}
