using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Reeb.SqlOM;
using Reeb.SqlOM.Render;

namespace Tests
{
	[TestFixture]
	public class SqlOM_SqlServerNative : SqlOmTestBase
	{
		ISqlOmRenderer renderer = new SqlServerRenderer();

		protected override IDbConnection CreateConnection()
		{
			return new SqlConnection(ConfigurationSettings.AppSettings["SqlServerNative"]);
		}

		protected override IDbDataAdapter CreateDataAdapter()
		{
			return new SqlDataAdapter();
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

		    Assert.Throws<InvalidQueryException>(() => RenderPage(0, 2, 3, query));
		}

		public override void Init()
		{
			base.Init();
		}

		protected override IDataParameter CreateParameter(string name, object val)
		{
			return new SqlParameter(name, val);
		}

		protected override string GetParameterName(string name)
		{
			return "@" + name;
		}

		[Test] 
		public virtual void TableSpace1()
		{
			SelectQuery query = new SelectQuery();
			query.TableSpace = "sqlom.dbo";
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers", "t");
			
			RenderSelect(query);
		}

		[Test] 
		public virtual void TableSpace2()
		{
			SelectQuery query = new SelectQuery();
			query.TableSpace = "foo.bar";
			query.Columns.Add(new SelectColumn("*"));
			query.FromClause.BaseTable = FromTerm.Table("customers", "t1", "sqlom", "dbo");
			query.FromClause.Join(JoinType.Inner, query.FromClause.BaseTable, FromTerm.Table("customers", "t2", "dbo"), "customerId", "customerId");
			
			RenderSelect(query);
		}
	}
}
