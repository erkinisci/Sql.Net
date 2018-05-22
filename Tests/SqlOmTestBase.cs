using System;
using System.Data;
using System.Data.Common;
using NUnit.Framework;
using Reeb.SqlOM;
using Reeb.SqlOM.Render;
using Reeb.Reporting;

namespace Tests
{
	public abstract class SqlOmTestBase
	{
		IDbConnection connection = null;

		protected abstract ISqlOmRenderer Renderer { get; }
		protected abstract IDbConnection CreateConnection();
		protected abstract IDbDataAdapter CreateDataAdapter();

		protected virtual void SetupPivot(PivotTable pivot)
		{
		}

		[Test]
		public virtual void Union()
		{
			SqlUnion union = new SqlUnion();
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn(SqlExpression.Raw("price * 10"), "priceX10"));
			query.FromClause.BaseTable = FromTerm.Table("products");
			
			union.Add(query);
			
			query = new SelectQuery();
			query.Columns.Add(new SelectColumn(SqlExpression.Field("price"), "priceX10"));
			query.FromClause.BaseTable = FromTerm.Table("products");

			union.Add(query, DistinctModifier.All);

			query = new SelectQuery();
			query.Columns.Add(new SelectColumn(SqlExpression.Field("price"), "priceX10"));
			query.FromClause.BaseTable = FromTerm.Table("products");

			union.Add(query, DistinctModifier.Distinct);

			string sql = Renderer.RenderUnion(union);
			Console.WriteLine(sql);
			RunSql(sql);
		}

		[Test] 
		public virtual void RawSql()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn(SqlExpression.Raw("price * 10"), "priceX10"));
			query.FromClause.BaseTable = FromTerm.Table("products");
			RenderSelect(query);
		}

		[Test] 
		public virtual void Pivot1()
		{
			SelectQuery baseQuery = new SelectQuery();
			baseQuery.Columns.Add(new SelectColumn("*"));
			baseQuery.FromClause.BaseTable = FromTerm.Table("orders");

			PivotTable pivot = new PivotTable();
			SetupPivot(pivot);
			pivot.BaseQuery = baseQuery;
			pivot.Function = SqlAggregationFunction.Sum;
			pivot.ValueField = "quantaty";
			pivot.RowField = "customerId";
			
			PivotColumn pivotCol = new PivotColumn("date", SqlDataType.Date);
			TimePeriod currentYear = TimePeriod.FromToday(TimePeriodType.Year);
			pivotCol.Values.Add(PivotColumnValue.CreateRange("PreviousYears", new Range(null, currentYear.Add(-1).PeriodStartDate)));
			pivotCol.Values.Add(PivotColumnValue.CreateRange("LastYear", new Range(currentYear.Add(-1).PeriodStartDate, currentYear.PeriodStartDate)));
			pivotCol.Values.Add(PivotColumnValue.CreateRange("FollowingYears", new Range(currentYear.PeriodStartDate, null)));
			pivot.Columns.Add(pivotCol);

			pivotCol = new PivotColumn("productId", SqlDataType.Number);
			pivotCol.Values.Add(PivotColumnValue.CreateScalar("product1", 1));
			pivotCol.Values.Add(PivotColumnValue.CreateRange("product2", new Range(2,3)));
			pivot.Columns.Add(pivotCol);

			SelectQuery pivotQuery = pivot.BuildPivotSql();
			DataTable data = Fill(pivotQuery);
			if (data == null)
				return;
			WriteTable(data);

			Console.WriteLine("Drill down");
			SelectQuery drillDownQuery = pivot.BuildDrillDownSql(SqlConstant.Number(1), "LastYear");
			data = Fill(drillDownQuery);
			if (data == null)
				return;
			WriteTable(data);

			Console.WriteLine("Drill down");
			drillDownQuery = pivot.BuildDrillDownSql(null, "LastYear");
			data = Fill(drillDownQuery);
			if (data == null)
				return;
			WriteTable(data);
		}

		[Test] 
		public virtual void Pivot2()
		{
			PivotTable pivot = new PivotTable();
			SetupPivot(pivot);
			pivot.BaseSql = "select o.*, c.name, p.name productName from orders o inner join customers c on o.customerId = c.customerId inner join products p on o.productId = p.productId";
			pivot.Function = SqlAggregationFunction.Count;
			pivot.ValueField = "productName";
			pivot.RowField = "productName";

			PivotColumn pivotCol = new PivotColumn("name", SqlDataType.String);
			pivotCol.Values.Add(PivotColumnValue.CreateScalar("c1", "John"));
			pivotCol.Values.Add(PivotColumnValue.CreateScalar("c2", "Lucinda"));
			pivotCol.Values.Add(PivotColumnValue.CreateScalar("c3", "Mark"));
			pivot.Columns.Add(pivotCol);
			pivot.WithTotal = false;

			SelectQuery pivotQuery = pivot.BuildPivotSql();
			DataTable data = Fill(pivotQuery);
			if (data == null)
				return;
			WriteTable(data);
		}

		[Test] 
		//[ExpectedException(typeof(InvalidQueryException))]
		public virtual void InvalidSelect1()
		{
			SelectQuery query = new SelectQuery();
			RenderSelect(query);
		}

		[Test]
		//[ExpectedException(typeof(InvalidQueryException))]
		public virtual void InvalidSelect2()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("*"));
			
			RenderSelect(query);		
		}

		[Test]
		//[ExpectedException(typeof(InvalidQueryException))]
		public void InvalidUpdate()
		{
			UpdateQuery query = new UpdateQuery();
			//RenderUpdate(query);
		    Assert.Throws<InvalidQueryException>(() => RenderUpdate(query));
        }

		[Test]
		public void Update()
		{
			UpdateQuery query = new UpdateQuery("products");
			query.Terms.Add(new UpdateTerm("price", SqlExpression.Number(12.1)));
			query.Terms.Add(new UpdateTerm("quantaty", SqlExpression.Field("quantaty")));
			query.WhereClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("productId"), SqlExpression.Number(1), CompareOperator.Equal) );
			RenderUpdate(query);
		}

		[Test]
		//[ExpectedException(typeof(InvalidQueryException))]
		public void InvalidInsert1()
		{
			InsertQuery query = new InsertQuery();
			RenderInsert(query);
		}

		[Test]
		//[ExpectedException(typeof(InvalidQueryException))]
		public void InvalidInsert2()
		{
			InsertQuery query = new InsertQuery("products");
			//RenderInsert(query);
		    Assert.Throws<InvalidQueryException>(() => RenderInsert(query));
        }

		[Test]
		public void Insert()
		{
			InsertQuery query = new InsertQuery("products");
			query.Terms.Add(new UpdateTerm("productId", SqlExpression.Number(999)));
			query.Terms.Add(new UpdateTerm("name", SqlExpression.String("Temporary Test Product")));
			query.Terms.Add(new UpdateTerm("price", SqlExpression.Number(123.45)));
			query.Terms.Add(new UpdateTerm("quantaty", SqlExpression.Number(97)));
			RenderInsert(query);
		}

		[Test]
		//[ExpectedException(typeof(InvalidQueryException))]
		public void InvalidDelete1()
		{
			DeleteQuery query = new DeleteQuery("_dummy_table_");
			//RenderDelete(query);
		    Assert.Throws<InvalidQueryException>(() => RenderDelete(query));
        }

		[Test]
		//[ExpectedException(typeof(InvalidQueryException))]
		public void InvalidDelete2()
		{
			DeleteQuery query = new DeleteQuery();
		    Assert.Throws<InvalidQueryException>(() => RenderDelete(query));
		}

		[Test]
		public void Delete()
		{
			DeleteQuery query = new DeleteQuery("products");
			query.WhereClause.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("productId"), SqlExpression.Number(999), CompareOperator.Equal));
			RenderDelete(query);
		}

		[Test] 
		public virtual void RowCount()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			
			RenderRowCount(query);
		}

		[Test] 
		public virtual void Paging1()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			query.OrderByTerms.Add(new OrderByTerm("name", null, OrderByDirection.Descending));
			query.OrderByTerms.Add(new OrderByTerm("birthDate", null, OrderByDirection.Ascending));

			int count = RenderRowCount(query);
			
			RenderPage(0, 2, count, query);
		}

		[Test] 
		public virtual void Paging2()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			query.OrderByTerms.Add(new OrderByTerm("name", null, OrderByDirection.Descending));
			query.OrderByTerms.Add(new OrderByTerm("birthDate", null, OrderByDirection.Ascending));
			
			int count = RenderRowCount(query);
			RenderPage(1, 2, count, query);
		}

		[Test] 
		public virtual void Paging3()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			query.OrderByTerms.Add(new OrderByTerm("name", null, OrderByDirection.Descending));
			query.OrderByTerms.Add(new OrderByTerm("birthDate", null, OrderByDirection.Ascending));
			query.GroupByTerms.Add(new GroupByTerm("name"));
			query.GroupByTerms.Add(new GroupByTerm("birthDate"));
			
			int count = RenderRowCount(query);
			RenderPage(1, 2, count, query);
		}

		[Test] 
		public virtual void Paging4()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers, "customerName"));
			query.Columns.Add(new SelectColumn("name", tProducts, "productName"));
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Inner, query.FromClause.BaseTable, tOrders, "customerId", "customerId");
			query.FromClause.Join(JoinType.Inner, tOrders, tProducts, "productId", "productId");

			query.OrderByTerms.Add(new OrderByTerm("name", tCustomers, OrderByDirection.Descending));

			int count = RenderRowCount(query);
			
			RenderPage(0, 2, count, query);
		}
		
		[Test] 
		public virtual void SimpleSelect()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			
			RenderSelect(query);
		}

		[Test] 
		public virtual void Join1()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn("name", tProducts));
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Inner, query.FromClause.BaseTable, tOrders, "customerId", "customerId");
			query.FromClause.Join(JoinType.Inner, tOrders, tProducts, "productId", "productId");
			
			RenderSelect(query);
		}
		
		[Test] 
		public virtual void Join2()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn("name", tProducts));
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Left, tCustomers, tOrders, new JoinCondition("customerId"));
			query.FromClause.Join(JoinType.Left, tOrders, tProducts, new JoinCondition("productId", "productId"));
			
			RenderSelect(query);
		}

		[Test] 
		public virtual void Join3()
		{
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tProducts));
			query.Columns.Add(new SelectColumn("date", tOrders));
			query.FromClause.BaseTable = tOrders;
			query.FromClause.Join(JoinType.Right, query.FromClause.BaseTable, tProducts, "productId", "productId");
			
			RenderSelect(query);
		}

		[Test] 
		public virtual void Join4()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("*"));
			query.FromClause.BaseTable = FromTerm.Table("customers");
			query.FromClause.Join(JoinType.Cross, query.FromClause.BaseTable, FromTerm.Table("products"));
			
			RenderSelect(query);
		}

		[Test] 
		public virtual void Join5()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn("name", tProducts));
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Left, tCustomers, tOrders, new JoinCondition("customerId"), new JoinCondition("customerId"));
			WhereClause condition = new WhereClause(WhereClauseRelationship.Or);
			condition.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("productId", tOrders), SqlExpression.Field("productId", tProducts), CompareOperator.Equal));
			condition.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("orderId", tOrders), SqlExpression.Field("productId", tProducts), CompareOperator.Equal));
			query.FromClause.Join(JoinType.Left, tOrders, tProducts, condition);
			
			RenderSelect(query);
		}

		[Test] 
		public virtual void Having()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn("price", tProducts, "sum", SqlAggregationFunction.Sum));
			
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Inner, query.FromClause.BaseTable, tOrders, "customerId", "customerId");
			query.FromClause.Join(JoinType.Inner, tOrders, tProducts, "productId", "productId");

			query.GroupByTerms.Add(new GroupByTerm("name", tCustomers));

			query.HavingPhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("name", tCustomers), SqlExpression.String("John"), CompareOperator.Equal) );

			RenderSelect(query);
		}

		[Test] 
		public virtual void GroupByWithRollup()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn("price", tProducts, "sum", SqlAggregationFunction.Sum));
			
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Inner, query.FromClause.BaseTable, tOrders, "customerId", "customerId");
			query.FromClause.Join(JoinType.Inner, tOrders, tProducts, "productId", "productId");

			query.GroupByTerms.Add(new GroupByTerm("name", tCustomers));
			
			query.GroupByWithRollup = true;

			RenderSelect(query);
		}
		
		[Test] 
		public virtual void GroupByWithCube()
		{
			FromTerm tCustomers = FromTerm.Table("customers");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn("price", tProducts, "sum", SqlAggregationFunction.Sum));
			
			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Inner, query.FromClause.BaseTable, tOrders, "customerId", "customerId");
			query.FromClause.Join(JoinType.Inner, tOrders, tProducts, "productId", "productId");

			query.GroupByTerms.Add(new GroupByTerm("name", tCustomers));
			
			query.GroupByWithCube = true;

			RenderSelect(query);
		}
		
		
		[Test] 
		public virtual void ComplicatedQuery()
		{
			FromTerm tCustomers = FromTerm.Table("customers", "c");
			FromTerm tProducts = FromTerm.Table("products", "p");
			FromTerm tOrders = FromTerm.Table("orders", "o");

			SelectQuery query = new SelectQuery();

			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn(SqlExpression.IfNull(SqlExpression.Field("name", tCustomers), SqlExpression.Constant(SqlConstant.String("name"))), "notNull"));
			query.Columns.Add(new SelectColumn(SqlExpression.Null(), "nullValue"));
			query.Columns.Add(new SelectColumn("name", tProducts, "productName", SqlAggregationFunction.None));
			query.Columns.Add(new SelectColumn("price", tProducts));

			query.FromClause.BaseTable = tCustomers;
			query.FromClause.Join(JoinType.Left, tCustomers, tOrders, "customerId", "customerId");
			query.FromClause.Join(JoinType.Inner, tOrders, tProducts, "productId", "productId");
			
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("name", tCustomers), SqlExpression.String("John"), CompareOperator.Equal));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.String("Dohe"), SqlExpression.Field("name", tCustomers), CompareOperator.NotEqual));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("name", tCustomers), SqlExpression.String("J%"), CompareOperator.Like));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Date(DateTime.Now), SqlExpression.Field("date", tOrders), CompareOperator.Greater));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(10), SqlExpression.Number(9), CompareOperator.Greater));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(10), SqlExpression.Number(9), CompareOperator.GreaterOrEqual));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(10), SqlExpression.Number(11.5), CompareOperator.LessOrEqual));
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Number(1), SqlExpression.Number(1), CompareOperator.BitwiseAnd));
			
			WhereClause group = new WhereClause(WhereClauseRelationship.Or);
			
			group.Terms.Add(WhereTerm.CreateBetween(SqlExpression.Field("price", tProducts), SqlExpression.Number(1), SqlExpression.Number(10)));
			group.Terms.Add(WhereTerm.CreateIn(SqlExpression.Field("name", tProducts), SqlConstantCollection.FromList(new string[] {"Nail", "Hamer", "Skrewdriver"})));
			group.Terms.Add(WhereTerm.CreateIn(SqlExpression.Field("name", tProducts), "select name from products"));
			group.Terms.Add(WhereTerm.CreateNotIn(SqlExpression.Field("name", tProducts), SqlConstantCollection.FromList(new string[] {"Unkown"})));
			group.Terms.Add(WhereTerm.CreateNotIn(SqlExpression.Field("name", tProducts), "select name from products"));
			group.Terms.Add(WhereTerm.CreateIsNull(SqlExpression.Field("name", tProducts)));
			group.Terms.Add(WhereTerm.CreateIsNotNull(SqlExpression.Field("name", tProducts)));
			group.Terms.Add(WhereTerm.CreateExists("select productId from products"));
			group.Terms.Add(WhereTerm.CreateNotExists("select productId from products"));
			
			query.WherePhrase.SubClauses.Add(group);
			
			query.OrderByTerms.Add(new OrderByTerm("name", tCustomers, OrderByDirection.Descending));
			query.OrderByTerms.Add(new OrderByTerm("price", OrderByDirection.Ascending));
			
			query.Distinct = true;
			query.Top = 10;

			RenderSelect(query);
		}

		[Test] 
		public virtual void SubQuery()
		{
			FromTerm tCustomers = FromTerm.Table("customers");

			SelectQuery subQuery = new SelectQuery();
			subQuery.Top = 1;
			subQuery.Columns.Add(new SelectColumn("name", tCustomers));
			subQuery.FromClause.BaseTable = FromTerm.Table("customers");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.Columns.Add(new SelectColumn(SqlExpression.SubQuery("select count(*) from customers"), "cnt"));
			query.Columns.Add(new SelectColumn(SqlExpression.SubQuery(subQuery), "subq"));
			query.FromClause.BaseTable = tCustomers;
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Field("customerId", tCustomers), SqlExpression.SubQuery("select customerId from customers where name='John'"), CompareOperator.Equal ) );

			RenderSelect(query);
		}

		[Test] 
		public virtual void Parameter()
		{
			FromTerm tCustomers = FromTerm.Table("customers");

			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("name", tCustomers));
			query.FromClause.BaseTable = tCustomers;
			query.WherePhrase.Terms.Add(WhereTerm.CreateCompare(SqlExpression.Parameter(GetParameterName("pName")), SqlExpression.Field("name", tCustomers), CompareOperator.Equal ) );

			string sql = Renderer.RenderSelect(query);
			Console.WriteLine(sql);
			if (connection != null)
			{
				IDbCommand command = connection.CreateCommand();
				command.CommandText = sql;
				command.Parameters.Add(CreateParameter("@pName", "John"));
				command.ExecuteNonQuery();
			}
		}

		protected abstract string GetParameterName(string name);
		protected abstract IDataParameter CreateParameter(string name, object val);

/*		[Test]
		[Ignore("")]
		public void Performace1()
		{
			SelectQuery query = new SelectQuery();
			query.Columns.Add(new SelectColumn("indexed"));
			query.FromClause.BaseTable = FromTerm.Table("LargeTable");
			query.OrderByTerms.Add(new OrderByTerm("indexed", null, OrderByDirection.Ascending));

			const int iterations = 10, pageSize = 10, maxPages = 100;
			int count = RenderRowCount(query);
			Random random = new Random(10);
			DateTime startTime = DateTime.Now;
			for (int i = 0; i < iterations; i++)
			{
				int pageIndex = random.Next(maxPages);
				string sql = Renderer.RenderPage(pageIndex, pageSize, count, query);
				RunSql(sql);
			}

			TimeSpan span = DateTime.Now - startTime;
			Console.WriteLine("Paging elapsed time: {0} ms.", span.TotalMilliseconds);

			random = new Random(10);
			startTime = DateTime.Now;
			for (int i = 0; i < iterations; i++)
			{
				int pageIndex = random.Next(maxPages);
				query.Top = (pageIndex + 1) * pageSize;
				string sql = Renderer.RenderSelect(query);
				RunSql(sql);
			}
			span = DateTime.Now - startTime;
			Console.WriteLine("Sorting elapsed time: {0} ms.", span.TotalMilliseconds);
		}
*/
		protected void RenderUpdate(UpdateQuery query)
		{
			string sql = Renderer.RenderUpdate(query);
			Console.WriteLine(sql);
			RunSql(sql);
		}

		protected void RenderInsert(InsertQuery query)
		{
			string sql = Renderer.RenderInsert(query);
			Console.WriteLine(sql);
			RunSql(sql);
		}

		protected void RenderDelete(DeleteQuery query)
		{
			string sql = Renderer.RenderDelete(query);
			Console.WriteLine(sql);
			RunSql(sql);
		}
		
		protected void RenderSelect(SelectQuery query)
		{
			string sql = Renderer.RenderSelect(query);
			Console.WriteLine(sql);
			RunSql(sql);
		}

		protected int RenderRowCount(SelectQuery query)
		{
			string sql = Renderer.RenderRowCount(query);
			Console.WriteLine(sql);

			if (connection == null)
				return 0;
			
			IDbCommand command = connection.CreateCommand();
			command.CommandText = sql;
			return Convert.ToInt32(command.ExecuteScalar());
		}

		protected void RenderPage(int pageIndex, int pageSize, int totalRows, SelectQuery query)
		{
			string sql = Renderer.RenderPage(pageIndex, pageSize, totalRows, query);
			Console.WriteLine(sql);
			RunSql(sql);
		}

		protected virtual void RunSql(string sql)
		{
			if (connection == null)
				return;
			
			IDbCommand command = connection.CreateCommand();
			command.CommandText = sql;
			command.ExecuteNonQuery();
			//IDataReader reader = command.ExecuteReader();
			//reader.Close();
		}

		DataTable Fill(SelectQuery query)
		{
			string sql = Renderer.RenderSelect(query);
			Console.WriteLine(sql);

			if (connection == null)
				return null;
			
			IDbCommand command = connection.CreateCommand();
			command.CommandText = sql;
			IDbDataAdapter da = CreateDataAdapter();
			DataSet ds = new DataSet();
			da.SelectCommand = command;
			da.Fill(ds);

            return ds.Tables[0];
		}

		void WriteTable(DataTable data)
		{
			foreach(DataColumn col in data.Columns)
				Console.Write("{0}\t", col.ColumnName);
			Console.WriteLine();
			foreach(DataRow row in data.Rows)
			{
				foreach(DataColumn col in data.Columns)
					Console.Write("{0}\t", row[col]);
				Console.WriteLine();
			}
		}

		[SetUp]
		public virtual void Init()
		{
			connection = CreateConnection();
			if (connection != null)
			{
				try { connection.Open(); }
				catch(Exception e)
				{
					Console.Out.WriteLine("Connection to database could not be established. Exception: {0}", e);
					connection = null;
				}
			}
		}
		
		[TearDown]
		public virtual void Dispose()
		{
		}
	}
}
