namespace Reeb.SqlOM
{
	/// <summary>
	/// Specifies how a result set should be ordered.
	/// </summary>
	public enum OrderByDirection 
	{ 
		/// <summary>Ascending Order</summary>
		Ascending, 
		/// <summary>Descending Order</summary>
		Descending 
	}

	/// <summary>
	/// Represents one term in an ORDER BY clause
	/// </summary>
	/// <remarks>
	/// Use OrderByTerm to specify how a result-set should be ordered.
	/// </remarks>
	public class OrderByTerm
	{
		string field;
		FromTerm table;
		OrderByDirection direction;
		
		/// <summary>
		/// Creates an ORDER BY term with field name and table alias
		/// </summary>
		/// <param name="field">Name of a field to order by</param>
		/// <param name="table">The table this field belongs to</param>
		/// <param name="dir">Order by direction</param>
		public OrderByTerm(string field, FromTerm table, OrderByDirection dir)
		{
			this.field = field;
			this.table = table;
			this.direction = dir;
		}
		
		/// <summary>
		/// Creates an ORDER BY term with field name and no table alias
		/// </summary>
		/// <param name="field">Name of a field to order by</param>
		/// <param name="dir">Order by direction</param>
		public OrderByTerm(string field, OrderByDirection dir) : this(field, null, dir)
		{
		}

		/// <summary>
		/// Gets the direction for this OrderByTerm
		/// </summary>
		public OrderByDirection Direction
		{
			get { return this.direction; }
		}

		/// <summary>
		/// Gets the name of a field to order by
		/// </summary>
		public string Field
		{
			get { return this.field; }
		}

		/// <summary>
		/// Gets the table alias for this OrderByTerm
		/// </summary>
		/// <remarks>
		/// Gets the name of a FromTerm the field specified by <see cref="OrderByTerm.Field">Field</see> property.
		/// </remarks>
		public string TableAlias
		{
			get { return (table == null) ? null : table.RefName; }
		}

		/// <summary>
		/// Returns the FromTerm associated with this OrderByTerm
		/// </summary>
		public FromTerm Table
		{
			get { return table; }
		}
	}
}
