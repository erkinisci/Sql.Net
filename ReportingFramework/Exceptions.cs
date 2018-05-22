using System;

namespace Reeb.Reporting
{
	/// <summary>
	/// General exception for pivot table processing
	/// </summary>
	public class PivotTableException : ApplicationException
	{
		/// <summary>
		/// Creates a new PivotTableException
		/// </summary>
		/// <param name="text"></param>
		public PivotTableException(string text) : base(text) {}
	}

	/// <summary>
	/// General exception for pivot table processing
	/// </summary>
	public class PivotTransformException : ApplicationException 
	{
		/// <summary>
		/// Creates a new PivotTransformException
		/// </summary>
		/// <param name="text"></param>
		public PivotTransformException(string text) : base(text) {}
	}
}
