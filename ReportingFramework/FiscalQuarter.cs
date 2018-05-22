using System;

namespace Reeb.Reporting
{
	/// <summary>
	/// Encapsulates alternative quarter definition
	/// </summary>
	/// <remarks>
	/// Some enterprises define their fiscal quarters differently then calendar quarters.
	/// Use the <see cref="StartDay"/> and <see cref="StartMonth"/> static properties to set the begining of the fiscal quarter.
	/// </remarks>
	public class FiscalQuarter
	{
		DateTime[] quarters = new DateTime[4];
		int year;

		static int startDay = 1, startMonth = 1;

		/// <summary>
		/// Creates a new FiscalQuarter
		/// </summary>
		public FiscalQuarter()
		{
			DateTime today = DateTime.Today;
			int fiscalQuarterMonth = startMonth;
			int fiscalQuarterDay = startDay;
			year = today.Year;
			quarters[0] = new DateTime(year, fiscalQuarterMonth, fiscalQuarterDay);
			quarters[1] = quarters[0].AddMonths(3);
			quarters[2] = quarters[0].AddMonths(6);
			quarters[3] = quarters[0].AddMonths(9);
		}

		/// <summary>
		/// Returns the current fiscal quarter index
		/// </summary>
		/// <returns>Current fiscal quarter index</returns>
		public int GetCurrentQuarter()
		{
			return GetQuarterForDate(DateTime.Today);
		}
		
		/// <summary>
		/// Returns fiscal quarter index which the specified date falls into
		/// </summary>
		/// <param name="date">The date</param>
		/// <returns>Fiscal quarter index</returns>
		public int GetQuarterForDate(DateTime date)
		{
			DateTime thisYearDate = new DateTime(year, date.Month, date.Day);
			for (int i = 0; i < 3; i++)
			{
				if (quarters[i] <= thisYearDate && thisYearDate < quarters[i+1])
					return i + 1;
			}
			return 4;
		}

		/// <summary>
		/// Gets the start date of a specific fiscal quarter
		/// </summary>
		/// <param name="quarter">Quarter index in year</param>
		/// <param name="year">Year</param>
		/// <returns>The start date of the specified fiscal quarter</returns>
		public DateTime GetStartDate(int quarter, int year)
		{
			DateTime date = quarters[quarter - 1];
			return new DateTime(year, date.Month, date.Day);
		}

		/// <summary>
		/// Gets or sets the day of month in which fiscal quarter begins
		/// </summary>
		public static int StartDay
		{
			get { return startDay; }
			set { startDay = value; }
		}

		/// <summary>
		/// Gets or sets the month in which fiscal quarter begins
		/// </summary>
		public static int StartMonth
		{
			get { return startMonth; }
			set { startMonth = value; }
		}
	}
}
