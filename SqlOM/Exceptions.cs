using System;

namespace Reeb.SqlOM
{
	/// <summary>
	/// InvalidQueryException exception can be thrown when the renderer decides that a query is invalid or incompatible with the target database.
	/// </summary>
	public class InvalidQueryException : ApplicationException
	{
		/// <summary>
		/// Creates a new InvalidQueryException
		/// </summary>
		/// <param name="text">Text of the exception</param>
		public InvalidQueryException(string text) : base(text) {}
	}
}
