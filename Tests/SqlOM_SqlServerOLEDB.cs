using System.Configuration;
using System.Data;
using System.Data.OleDb;
using NUnit.Framework;
using Reeb.SqlOM;
using Reeb.SqlOM.Render;

namespace Tests
{
	[TestFixture]
	public class SqlOM_SqlServerOLEDB : SqlOmTestBase
	{
		ISqlOmRenderer renderer = new SqlServerRenderer();

		protected override IDbConnection CreateConnection()
		{
			return new OleDbConnection(ConfigurationSettings.AppSettings["SqlServerOLEDB"]);
		}

		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new OleDbDataAdapter();
		}

		protected override ISqlOmRenderer Renderer
		{
			get { return renderer; }
		}

		[Test] 
		//[ExpectedException(typeof(InvalidQueryException))]
		public void PagingNoOrder()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			
			;
		    Assert.Throws<InvalidQueryException>(() => RenderPage(0, 2, 3, query));
        }

		public override void Init()
		{
			base.Init();
		}

		protected override IDataParameter CreateParameter(string name, object val)
		{
			return new OleDbParameter(name, val);
		}

		protected override string GetParameterName(string name)
		{
			return "?";
		}

		[Test] 
		public virtual void TableSpace()
		{
			SelectQuery query = new SelectQuery();
			query.TableSpace = "sqlom.dbo";
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers", "t");
			
			RenderSelect(query);
		}
	}
}
