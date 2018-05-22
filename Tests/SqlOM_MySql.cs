using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Reeb.SqlOM;
using Reeb.SqlOM.Render;

namespace Tests
{
	[TestFixture]
	public class SqlOM_MySql : SqlOmTestBase 
	{
		ISqlOmRenderer renderer = new MySqlRenderer();

		protected override IDbConnection CreateConnection()
		{
			return new MySqlConnection(ConfigurationSettings.AppSettings["MySql"]);
		}

		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new MySqlDataAdapter();
		}

		protected override ISqlOmRenderer Renderer
		{
			get { return renderer; }
		}

		public override void Init()
		{
			base.Init();
		}

		[Test] 
		//[ExpectedException(typeof(InvalidQueryException))]
		public override void GroupByWithCube()
		{
		    Assert.Throws<InvalidQueryException>(() => base.GroupByWithCube());
			//base.GroupByWithCube ();
		}

		protected override IDataParameter CreateParameter(string name, object val)
		{
			return new MySqlParameter(name, val);
		}

		protected override string GetParameterName(string name)
		{
			return "@" + name;
		}

		protected override void SetupPivot(Reeb.Reporting.PivotTable pivot)
		{
			pivot.WithIsTotal = false;
		}
	}
}
