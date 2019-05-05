using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
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
    public interface IMeasurementDao {
        

        Task<IEnumerable<Measurement>> FindAllAsync();
        Task<Measurement> FindByIdAsync(int id);
        Task<Measurement> FindJoinedByIdAsync(int id);
        Task<IEnumerable<Measurement>> FindByStationIdAsync(int stationId);
        Task<IEnumerable<Measurement>> FindByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<Measurement>> FindByDateRangeAndStationAsync(DateTime start, DateTime end, int stationId);
        Task<IEnumerable<Measurement>> FindByMeasurementTypeAsync(int measurementTypeId);
        Task<bool> AddMeasurementAsync(Measurement measurement);
        Task<bool> UpdateMeasurementAsync(Measurement measurement);
        Task<bool> DeleteMeasurementAsync(Measurement measurement);

        Task<double> GetSum(int queryMode);
        Task<double> GetMin(int queryMode);
        Task<double> GetMax(int queryMode);
        Task<double> GetAvg(int queryMode);

        Task<IEnumerable<Measurement>> GetMeasurementsForStation(int stationId, DateTime start, DateTime end, int queryMode);
        Task<IEnumerable<Measurement>> GetLastMeasurementsForStation(int stationId, int queryMode, int limitation);

        Task<IEnumerable<MeasurementAnalytic>> GetSum(DateTime start, DateTime end, int queryMode, int groupByMode);
        Task<IEnumerable<MeasurementAnalytic>> GetMin(DateTime start, DateTime end, int queryMode, int groupByMode);
        Task<IEnumerable<MeasurementAnalytic>> GetMax(DateTime start, DateTime end, int queryMode, int groupByMode);
        Task<IEnumerable<MeasurementAnalytic>> GetAvg(DateTime start, DateTime end, int queryMode, int groupByMode);

        Task<IEnumerable<MeasurementAnalytic>> GetSum(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);
        Task<IEnumerable<MeasurementAnalytic>> GetMin(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);
        Task<IEnumerable<MeasurementAnalytic>> GetMax(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);
        Task<IEnumerable<MeasurementAnalytic>> GetAvg(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations);

    }
}
