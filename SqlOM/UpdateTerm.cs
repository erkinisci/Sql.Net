using System;

namespace Reeb.SqlOM
{
	/// <summary>
	/// Encapsulates a column-value pair for UPDATE and INSERT statements
	/// </summary>
	public class UpdateTerm
	{
		string fieldName;
		SqlExpression val;

		/// <summary>
		/// Creates an UpdateTerm
		/// </summary>
		/// <param name="fieldName">The name of the field to be updated</param>
		/// <param name="val">New field value</param>
		public UpdateTerm(string fieldName, SqlExpression val)
		{
			this.fieldName = fieldName;
			this.val = val;
		}

		/// <summary>
		/// Gets the name of the field which is to be updated
		/// </summary>
		public string FieldName
		{
			get { return this.fieldName; }
		}

		/// <summary>
		/// Gets the value the field will be updated to
		/// </summary>
		public SqlExpression Value
		{
			get { return this.val; }
		}
	}
}
