using System.Configuration;
using System.Data;
using NUnit.Framework;
using Oracle.ManagedDataAccess.Client;
using Reeb.SqlOM;
using Reeb.SqlOM.Render;

namespace Tests
{
	[TestFixture]
	public class SqlOM_OracleODP : SqlOmTestBase
	{
		ISqlOmRenderer renderer = new OracleRenderer();
		public SqlOM_OracleODP()
		{
		}

		protected override IDbConnection CreateConnection()
		{
			return new OracleConnection(ConfigurationSettings.AppSettings["OracleODP"]);
		}

		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new OracleDataAdapter();
		}

		protected override ISqlOmRenderer Renderer
		{
			get { return renderer; }
		}

		protected override IDataParameter CreateParameter(string name, object val)
		{
			return new OracleParameter(name, val);
		}

		protected override string GetParameterName(string name)
		{
			return ":1";
		}
	}
}
