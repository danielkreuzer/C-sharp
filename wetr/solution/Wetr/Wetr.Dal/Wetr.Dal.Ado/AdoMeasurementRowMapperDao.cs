using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Server.Dal.Ado {
    public static class AdoMeasurementRowMapperDao {

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear) {
            //DateTime jan1 = new DateTime(year, 1, 1);
            //int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            //// Use first Thursday in January to get first week of the year as
            //// it will never be in Week 52/53
            //DateTime firstThursday = jan1.AddDays(daysOffset);
            //var cal = CultureInfo.CurrentCulture.Calendar;
            //int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            //var weekNum = weekOfYear;
            //// As we're adding days to a date in Week 1,
            //// we need to subtract 1 in order to get the right date for week #1
            //if (firstWeek == 1) {
            //    weekNum -= 1;
            //}

            //// Using the first Thursday as starting week ensures that we are starting in the right year
            //// then we add number of weeks multiplied with days
            //var result = firstThursday.AddDays(weekNum * 7);

            //// Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            //// Debug.WriteLine(result.AddDays(-3).Day.ToString() + " " + result.AddDays(-3).Month.ToString());
            //return result.AddDays(-3);

            DateTime startOfYear = new DateTime(year, 1, 1);

            // The +7 and %7 stuff is to avoid negative numbers etc.
            int daysToFirstCorrectDay = ((1 - (int)startOfYear.DayOfWeek) + 7) % 7;

            return startOfYear.AddDays(7 * (weekOfYear - 1) + daysToFirstCorrectDay);
        }

        public static RowMapper<MeasurementAnalytic> MeasurementAnalytic0 =
            row => new MeasurementAnalytic {
                Value = DoubleParser(row),
                Hour = 1,
                Day = 1,
                Week = 1,
                Month = 1,
                Year = 2000
            };

        public static RowMapper<MeasurementAnalytic> MeasurementAnalytic1 =
            row => new MeasurementAnalytic {
                Value = DoubleParser(row),
                Hour = 1,
                Day = 1,
                Week = 1,
                Month = 1,
                Year = (int)row["year"]
            };

        public static RowMapper<MeasurementAnalytic> MeasurementAnalytic2 =
            row => new MeasurementAnalytic {
                Value = DoubleParser(row),
                Hour = 1,
                Day = 1,
                Week = 1,
                Month = (int)row["month"],
                Year = (int)row["year"]
            };

        public static RowMapper<MeasurementAnalytic> MeasurementAnalytic3 =
            row => new MeasurementAnalytic {
                Value = DoubleParser(row),
                Hour = 1,
                Day = FirstDateOfWeekISO8601((int)row["year"], (int)row["week"]).Day,
                Week = (int)row["week"],
                Month = FirstDateOfWeekISO8601((int)row["year"], (int)row["week"]).Month,
                Year = (int)row["year"]
            };

        public static RowMapper<MeasurementAnalytic> MeasurementAnalytic4 =
            row => new MeasurementAnalytic {
                Value = DoubleParser(row),
                Hour = 1,
                Day = (int)row["day"],
                Week = (int)row["week"],
                Month = (int)row["month"],
                Year = (int)row["year"]
            };

        public static RowMapper<MeasurementAnalytic> MeasurementAnalytic5 =
            row => new MeasurementAnalytic {
                Value = DoubleParser(row),
                Hour = (int)row["hour"],
                Day = (int)row["day"],
                Week = (int)row["week"],
                Month = (int)row["month"],
                Year = (int)row["year"]
            };

        public static double DoubleParser(IDataRecord record) {
            try {
                return (double)record["value"];
            }
            catch {
                string toParse = ((string)record["value"]).Replace(".", ",");
                double.TryParse(toParse, out var ret);
                return ret;
            }
        }

        public static RowMapper<MeasurementAnalytic> GetMapperForMode(int mode) {
            if (mode == GroupByMode.none) {
                return MeasurementAnalytic0;
            } else if (mode == GroupByMode.year) {
                return MeasurementAnalytic1;
            } else if (mode == GroupByMode.month) {
                return MeasurementAnalytic2;
            } else if (mode == GroupByMode.week) {
                return MeasurementAnalytic3;
            } else if (mode == GroupByMode.day) {
                return MeasurementAnalytic4;
            }
            return MeasurementAnalytic5;
        }
    }
}
