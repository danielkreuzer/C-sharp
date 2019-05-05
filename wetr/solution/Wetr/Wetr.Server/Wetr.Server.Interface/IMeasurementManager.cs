using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Server.Interface {
    public static class QueryMode {
        public static int temperature = 0;
        public static int air_pressure = 1;
        public static int rainfall = 2;
        public static int humidity = 3;
        public static int wind_speed = 4;
    }

    public static class GroupByMode {
        public static int none = 0;
        public static int hour = 1;
        public static int day = 2;
        public static int week = 3;
        public static int month = 4;
        public static int year = 5;
    }
    public interface IMeasurementManager {
        Task<bool> AddNewMeasurementDataPackage(Measurement measurement);




        /*
         * Kumulation von Daten für bestimmte Zeitintervalle (Stunde, Tag, Woche, Monat): Datenkumulation
         * kann Minimum-, Maximum-, Durchschnittsbildung aber auch Aufsummieren
         * der Messwerte bedeuten.
         *
            * Was war die Durchschnitts-/Minimal-/Maximal-Temperatur bei einer bestimmten Wetterstation/
            * in einer Region gruppiert nach Tagen/Wochen/Monaten?
            */

        /*
         *Was war die tage-/wochen-/monatsweise kumulierte Niederschlagsmenge bei einer bestimmten
         * Wetterstation?
         */

        /*
         *Was war die durchschnittliche Niederschlagsmenge in einer bestimmten Region gruppiert
         * nach Tagen/Wochen/Monaten?
         */

        // WHY NOT ALL FUNCTION WRAPPED TO ONE WITH OPTIONAL PARAMETERS?
        // OVERLOADS GUARANTEE THE RIGHT COMBINATION OF THE ENTERED DATA!

        Task<IEnumerable<Measurement>> GetMeasurementsForStation(int stationId, DateTime start, DateTime end, int queryMode);
        Task<IEnumerable<Measurement>> GetLatestMeasurementsForStation(int stationId, int queryMode, int limitation);

        Task<IEnumerable<Unit>> GetAllUnits();

        Task<IEnumerable<MeasurementAnalytic>> Sum(DateTime start, DateTime end, int queryMode, int groupByMode);
        Task<IEnumerable<MeasurementAnalytic>> Min(DateTime start, DateTime end, int queryMode, int groupByMode);
        Task<IEnumerable<MeasurementAnalytic>> Max(DateTime start, DateTime end, int queryMode, int groupByMode);
        Task<IEnumerable<MeasurementAnalytic>> Avg(DateTime start, DateTime end, int queryMode, int groupByMode);

        Task<IEnumerable<MeasurementAnalytic>> Sum(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);
        Task<IEnumerable<MeasurementAnalytic>> Min(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);
        Task<IEnumerable<MeasurementAnalytic>> Max(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);
        Task<IEnumerable<MeasurementAnalytic>> Avg(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);

        //Task<double> SumTemperature(DateTime start, DateTime end);
        //Task<double> MinTemperature(DateTime start, DateTime end);
        //Task<double> MaxTemperature(DateTime start, DateTime end);
        //Task<double> AvgTemperature(DateTime start, DateTime end);

        //Task<double> SumTemperature(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MinTemperature(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MaxTemperature(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> AvgTemperature(DateTime start, DateTime end, IEnumerable<Station> stations);

        //Task<double> SumAirPressure(DateTime start, DateTime end);
        //Task<double> MinAirPressure(DateTime start, DateTime end);
        //Task<double> MaxAirPressure(DateTime start, DateTime end);
        //Task<double> AvgAirPressure(DateTime start, DateTime end);

        //Task<double> SumAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MinAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MaxAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> AvgAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations);

        //Task<double> SumRainfall(DateTime start, DateTime end);
        //Task<double> MinRainfall(DateTime start, DateTime end);
        //Task<double> MaxRainfall(DateTime start, DateTime end);
        //Task<double> AvgRainfall(DateTime start, DateTime end);

        //Task<double> SumRainfall(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MinRainfall(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MaxRainfall(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> AvgRainfall(DateTime start, DateTime end, IEnumerable<Station> stations);

        //Task<double> SumHumidity(DateTime start, DateTime end);
        //Task<double> MinHumidity(DateTime start, DateTime end);
        //Task<double> MaxHumidity(DateTime start, DateTime end);
        //Task<double> AvgHumidity(DateTime start, DateTime end);

        //Task<double> SumHumidity(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MinHumidity(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MaxHumidity(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> AvgHumidity(DateTime start, DateTime end, IEnumerable<Station> stations);

        //Task<double> SumWindSpeed(DateTime start, DateTime end);
        //Task<double> MinWindSpeed(DateTime start, DateTime end);
        //Task<double> MaxWindSpeed(DateTime start, DateTime end);
        //Task<double> AvgWindSpeed(DateTime start, DateTime end);

        //Task<double> SumWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MinWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> MaxWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations);
        //Task<double> AvgWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations);
    }
}
