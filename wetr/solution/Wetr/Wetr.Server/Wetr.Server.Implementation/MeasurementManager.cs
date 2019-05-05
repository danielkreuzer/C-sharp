using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Ado;
using Wetr.Dal.Interface;
using Wetr.Domain;
using Wetr.Server.Interface;

namespace Wetr.Server.Implementation {
    public class MeasurementManager : IMeasurementManager {
        private static IMeasurementDao _iMeasurementDao;
        private static IUnitDao _unitDao;
        private static string _connectionStringConfigName = "WetrDBConnection";

        private static IMeasurementDao GetIMeasurementDao() {
            return _iMeasurementDao ?? (_iMeasurementDao =
                       new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        private static IUnitDao GetIUnitDao() {
            return _unitDao ?? (_unitDao =
                       new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        public class EqualityFactory {
            private sealed class Impl<T> : IEqualityComparer<T> {
                private Func<T, T, bool> m_del;
                private IEqualityComparer<T> m_comp;
                public Impl(Func<T, T, bool> del) {
                    m_del = del;
                    m_comp = EqualityComparer<T>.Default;
                }
                public bool Equals(T left, T right) {
                    return m_del(left, right);
                }
                public int GetHashCode(T value) {
                    return m_comp.GetHashCode(value);
                }
            }
            public static IEqualityComparer<T> Create<T>(Func<T, T, bool> del) {
                return new Impl<T>(del);
            }
        }

        public async Task<bool> AddNewMeasurementDataPackage(Measurement measurement) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                if (measurement != null) {
                    return await measurementDao.AddMeasurementAsync(measurement);
                }

                return false;
            }
            catch (Exception) {
                return false;
            }
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsForStation(int stationId, DateTime start, DateTime end, int queryMode) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return await measurementDao.GetMeasurementsForStation(stationId, start, end, queryMode);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Measurement>> GetLatestMeasurementsForStation(int stationId, int queryMode, int limitation) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return await measurementDao.GetLastMeasurementsForStation(stationId, queryMode, limitation);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Unit>> GetAllUnits() {
            try {
                IUnitDao iUnitDao = GetIUnitDao();
                return await iUnitDao.FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Sum(DateTime start, DateTime end, int queryMode, int groupByMode) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetSum(start, end, queryMode, groupByMode)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Min(DateTime start, DateTime end, int queryMode, int groupByMode) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetMin(start, end, queryMode, groupByMode)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Max(DateTime start, DateTime end, int queryMode, int groupByMode) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetMax(start, end, queryMode, groupByMode)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Avg(DateTime start, DateTime end, int queryMode, int groupByMode) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetAvg(start, end, queryMode, groupByMode)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Sum(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetSum(start, end, queryMode, groupByMode, stations)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Min(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetMin(start, end, queryMode, groupByMode, stations)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Max(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetMax(start, end, queryMode, groupByMode, stations)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<MeasurementAnalytic>> Avg(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            try {
                IMeasurementDao measurementDao = GetIMeasurementDao();
                return (await measurementDao.GetAvg(start, end, queryMode, groupByMode, stations)).OrderBy(x => x.GetDateTime());
            }
            catch (Exception) {
                return null;
            }
        }

        //public async Task<double> SumTemperature(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.temperature);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinTemperature(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.temperature);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxTemperature(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.temperature);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgTemperature(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.temperature);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumTemperature(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.temperature, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinTemperature(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.temperature, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxTemperature(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.temperature, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgTemperature(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.temperature, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumTemperature(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.temperature, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinTemperature(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.temperature, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxTemperature(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.temperature, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgTemperature(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.temperature, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumAirPressure(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.air_pressure);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinAirPressure(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.air_pressure);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxAirPressure(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.air_pressure);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgAirPressure(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.air_pressure);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumAirPressure(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.air_pressure, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinAirPressure(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.air_pressure, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxAirPressure(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.air_pressure, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgAirPressure(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.air_pressure, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.air_pressure, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.air_pressure, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.air_pressure, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgAirPressure(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.air_pressure, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumRainfall(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.rainfall);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinRainfall(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.rainfall);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxRainfall(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.rainfall);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgRainfall(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.rainfall);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumRainfall(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.rainfall, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinRainfall(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.rainfall, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxRainfall(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.rainfall, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgRainfall(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.rainfall, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumRainfall(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.rainfall, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinRainfall(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.rainfall, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxRainfall(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.rainfall, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgRainfall(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.rainfall, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumHumidity(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.humidity);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinHumidity(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.humidity);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxHumidity(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.humidity);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgHumidity(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.humidity);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumHumidity(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.humidity, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinHumidity(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.humidity, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxHumidity(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.humidity, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgHumidity(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.humidity, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumHumidity(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.humidity, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinHumidity(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.humidity, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxHumidity(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.humidity, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgHumidity(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.humidity, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumWindSpeed(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.wind_speed);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinWindSpeed(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.wind_speed);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxWindSpeed(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.wind_speed);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgWindSpeed(DateTime start, DateTime end) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.wind_speed);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumWindSpeed(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.wind_speed, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinWindSpeed(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.wind_speed, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxWindSpeed(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.wind_speed, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgWindSpeed(DateTime start, DateTime end, Station station) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.wind_speed, station);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> SumWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetSum(start, end, QueryMode.wind_speed, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MinWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMin(start, end, QueryMode.wind_speed, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> MaxWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetMax(start, end, QueryMode.wind_speed, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}

        //public async Task<double> AvgWindSpeed(DateTime start, DateTime end, IEnumerable<Station> stations) {
        //    try {
        //        IMeasurementDao measurementDao = GetIMeasurementDao();
        //        return await measurementDao.GetAvg(start, end, QueryMode.wind_speed, stations);
        //    }
        //    catch (Exception) {
        //        return double.MinValue;
        //    }
        //}
    }
}
