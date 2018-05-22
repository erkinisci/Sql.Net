using System;

namespace Reeb.SqlOM
{
	/// <summary>
	/// Encapsulates SQL CASE clause
	/// </summary>
	public class CaseClause
	{
		CaseTermCollection terms = new CaseTermCollection();
		SqlExpression elseVal = null;
		
		/// <summary>
		/// Creates a new CaseClause
		/// </summary>
		public CaseClause()
		{
		}

		/// <summary>
		/// Gets the <see cref="CaseTerm"/> collection for this CaseClause
		/// </summary>
		public CaseTermCollection Terms
		{
			get { return this.terms; }
		}

		/// <summary>
		/// Gets or sets the value CASE default value
		/// </summary>
		public SqlExpression ElseValue
		{
			get { return this.elseVal; }
			set { this.elseVal = value; }
		}
	}
}
