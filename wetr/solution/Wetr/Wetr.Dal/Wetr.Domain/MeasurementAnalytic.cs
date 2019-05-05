using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class MeasurementAnalytic {
        public int Hour { get; set; }
        public int Day { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public double Value { get; set; }

        public MeasurementAnalytic(double value, int hour = 1, int day = 1, int week = 1, int month = 1, int year = 1) {
            Hour = hour;
            Day = day;
            Week = week;
            Month = month;
            Year = year;
            Value = value;
        }

        public MeasurementAnalytic() {
            
        }

        public DateTime GetDateTime() {
            return new DateTime(Year, Month, Day, Hour, 0, 0);
        }
    }
}
