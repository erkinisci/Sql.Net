using System;
using Reeb.SqlOM;

namespace Reeb.Reporting
{
	/// <summary>
	/// Encapsulates a pivot ColumnField
	/// </summary>
	public class PivotColumn
	{
		SqlDataType dataType;
		string columnField;
		PivotColumnValueCollection values = new PivotColumnValueCollection();
		
		/// <summary>
		/// Creates a new PivotTableItem
		/// </summary>
		public PivotColumn()
		{
		}

		/// <summary>
		/// Creates a new PivotTableItem
		/// </summary>
		/// <param name="columnField">Name of the ColumnField</param>
		/// <param name="dataType">DataType of the pivot column</param>
		public PivotColumn(string columnField, SqlDataType dataType)
		{
			this.columnField = columnField;
			this.dataType = dataType;
		}

		/// <summary>
		/// Gets or sets the ColumnField
		/// </summary>
		public string ColumnField 
		{ 
			get { return columnField; } 
			set { columnField = value; } 
		}

		/// <summary>
		/// Gets the values of this PivotColumn
		/// </summary>
		public PivotColumnValueCollection Values
		{ 
			get { return values; } 
		}

		/// <summary>
		/// Gets or sets the data type of the pivot column
		/// </summary>
		public SqlDataType DataType
		{
			get { return dataType; }
			set { dataType = value; }
		}
	}
}
