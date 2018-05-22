using System;
using Reeb.SqlOM;

namespace Reeb.Reporting
{
	internal enum PivotColumnValueType {Scalar, Range}

	/// <summary>
	/// Encapsulates a value of a Pivot Column
	/// </summary>
	public class PivotColumnValue
	{
		string crossTabColumnName;
		object val = null;
		PivotColumnValueType valueType;
		Range range;
		
		/// <summary>
		/// Creates a new PivotColumnValue
		/// </summary>
		public PivotColumnValue()
		{
		}

		/// <summary>
		/// Creates a new PivotColumnValue
		/// </summary>
		public PivotColumnValue(string crossTabColumnName)
		{
			this.crossTabColumnName = crossTabColumnName;
		}

		/// <summary>
		/// Creates a new PivotColumnValue with a scalar value
		/// </summary>
		public static PivotColumnValue CreateScalar(string crossTabColumnName, object val)
		{
			PivotColumnValue columnValue = new PivotColumnValue(crossTabColumnName);
			columnValue.SetScalarValue(val);
			return columnValue;
		}

		/// <summary>
		/// Creates a new PivotColumnValue with a range value
		/// </summary>
		public static PivotColumnValue CreateRange(string crossTabColumnName, Range range)
		{
			PivotColumnValue columnValue = new PivotColumnValue(crossTabColumnName);
			columnValue.SetRangeValue(range);
			return columnValue;
		}

		/// <summary>
		/// Gets or sets the name of the column in the resulting CrossTab
		/// </summary>
		public string CrossTabColumnName
		{
			get { return this.crossTabColumnName; }
			set { this.crossTabColumnName = value; }
		}

		/// <summary>
		/// Sets a scalar value
		/// </summary>
		/// <param name="val">A value which matches PivotColumn's data type</param>
		public void SetScalarValue(object val)
		{
			this.val = val;
			this.valueType = PivotColumnValueType.Scalar;
		}

		/// <summary>
		/// Sets a range value
		/// </summary>
		/// <param name="range"></param>
		public void SetRangeValue(Range range)
		{
			this.range = range;
			this.valueType = PivotColumnValueType.Range;
		}

		internal object Value
		{
			get { return this.val; }
		}

		internal PivotColumnValueType ValueType
		{
			get { return this.valueType; }
		}

		internal Range Range
		{
			get { return this.range; }
		}
	}
}
