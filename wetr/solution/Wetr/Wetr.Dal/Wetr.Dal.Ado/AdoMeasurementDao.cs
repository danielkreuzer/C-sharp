using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;
using Wetr.Server.Dal.Ado;

namespace Wetr.Dal.Ado {
    public class AdoMeasurementDao : IMeasurementDao {
        private static readonly RowMapper<Measurement> MeasurementMapper =
            row => new Measurement {
                Id = (int)row["id"],
                Value = (float)row["value"],
                TypeId = (int)row["type_id"],
                Timestamp = (DateTime)row["timestamp"],
                StationId = (int)row["station_id"],
                UnitId = (int)row["unit_id"],
                Unit = null,
                Station = null,
                MeasurementType = null
            };

        private static readonly RowMapper<Measurement> MeasurmentLatestMapper =
            row => new Measurement {
                Id = (int)row["measurementID"],
                Value = (float)row["measurementValue"],
                MeasurementType = new MeasurementType {
                    Id = (int)row["mstID"],
                    Name = (string)row["mstName"]
                },
                Timestamp = (DateTime)row["measurementTimestamp"],
                Station = null,
                Unit = new Unit {
                    Id = (int)row["unitID"],
                    ShortName = (string)row["unitShortName"],
                    LongName = (string)row["unitLongName"]
                }
            };

        private static readonly RowMapper<Measurement> MeasurmentJoinedMapper =
            row => new Measurement {
                Id = (int)row["id"],
                Value = (float)row["value"],
                MeasurementType = new MeasurementType {
                    Id = (int)row["mid"],
                    Name = (string)row["mname"]
                },
                Timestamp = (DateTime)row["timestamp"],
                Station = new Station {
                    Id = (int)row["sid"],
                    Name = (string)row["sname"],
                    TypeId = (int)row["type_id"],
                    Latitude = (float)row["latitude"],
                    Longitude = (float)row["longitude"],
                    CommunityId = (int)row["community_id"],
                    Creator = (int)row["creator"],
                    Community = null,
                    District = null,
                    Province = null,
                    User = null
                },
                Unit = new Unit {
                    Id = (int)row["uid"],
                    ShortName = (string)row["short_name"],
                    LongName = (string)row["long_name"]
                }
            };

        private static readonly RowMapper<DoubleCallback> DoubleMapper =
            row => new DoubleCallback {
                Value = (double)row["result"]
            };

        public class GroupByStringHandler {
            public string Select { get; set; }
            public string GroupByOrderBy { get; set; }

            public GroupByStringHandler(string @select, string groupByOrderBy) {
                Select = @select;
                GroupByOrderBy = groupByOrderBy;
            }
        }

        public string GetQueryString(int queryMode) {
            if (queryMode == QueryMode.temperature) {
                return "temperature";
            } else if (queryMode == QueryMode.air_pressure) {
                return "air_pressure";
            } else if (queryMode == QueryMode.rainfall) {
                return "rainfall";
            } else if (queryMode == QueryMode.humidity) {
                return "humidity";
            } else if (queryMode == QueryMode.wind_speed) {
                return "wind_speed";
            } else
                return "";
        }

        public GroupByStringHandler GetGroupByString(int groupByMode) {
            if (groupByMode == GroupByMode.none) {
                return new GroupByStringHandler("", "");
            } else if (groupByMode == GroupByMode.hour) {
                return new GroupByStringHandler(" HOUR(timestamp) AS hour, DAY(timestamp) AS day, WEEK(timestamp) AS week, MONTH(timestamp) AS month, YEAR(timestamp) AS year,",
                    " GROUP BY HOUR(timestamp), DAY(timestamp), WEEK(timestamp), MONTH(timestamp), YEAR(timestamp) ORDER BY DAY(timestamp), MONTH(timestamp), YEAR(timestamp), HOUR(timestamp)");
            } else if (groupByMode == GroupByMode.day) {
                return new GroupByStringHandler(" DAY(timestamp) AS day, WEEK(timestamp) AS week, MONTH(timestamp) AS month, YEAR(timestamp) AS year,",
                    " GROUP BY DAY(timestamp), WEEK(timestamp), MONTH(timestamp), YEAR(timestamp) ORDER BY DAY(timestamp), MONTH(timestamp), YEAR(timestamp)");
            } else if (groupByMode == GroupByMode.week) {
                return new GroupByStringHandler(" WEEK(timestamp) AS week, MONTH(timestamp) AS month, YEAR(timestamp) AS year,",
                    " GROUP BY WEEK(timestamp), MONTH(timestamp), YEAR(timestamp) ORDER BY MONTH(timestamp), YEAR(timestamp)");
            } else if (groupByMode == GroupByMode.month) {
                return new GroupByStringHandler(" MONTH(timestamp) AS month, YEAR(timestamp) AS year,",
                    " GROUP BY MONTH(timestamp), YEAR(timestamp) ORDER BY MONTH(timestamp), YEAR(timestamp)");
            } else if (groupByMode == GroupByMode.year) {
                return new GroupByStringHandler(" YEAR(timestamp) AS year,",
                    " GROUP BY YEAR(timestamp) ORDER BY YEAR(timestamp)");
            }

            return new GroupByStringHandler("", "");
        }

        private string GetStationsString(IEnumerable<Station> stations) {
            string stationsString = "";
            int i = 1;
            int max = stations.Count();
            foreach (Station station in stations) {
                if (i < max) {
                    stationsString += station.Id.ToString() + ", ";
                } else {
                    stationsString += station.Id.ToString();
                }

                i++;
            }

            return stationsString;
        }

        private readonly AdoTemplate _template;

        public AdoMeasurementDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<IEnumerable<Measurement>> FindAllAsync() {
            return await _template.QueryAsync("select * from measurement", MeasurementMapper);
        }

        public async Task<Measurement> FindByIdAsync(int id) {
            return (await _template.QueryAsync("select * from measurement where id=@id",
                new[] { new QueryParameter("@id", id) },
                MeasurementMapper
            )).FirstOrDefault();
        }

        public async Task<Measurement> FindJoinedByIdAsync(int id) {
            return (await _template.QueryAsync("SELECT measurement.id, measurement.value, m.id AS \"mid\", m.name AS \"mname\", measurement.timestamp,\r\n       s.id AS \"sid\", s.name AS \"sname\", s.type_id, s.longitude, s.latitude,\r\n       s.community_id, s.creator, u.id AS \"uid\", u.short_name, u.long_name\r\n  FROM measurement\r\n    JOIN measurement_type m on measurement.type_id = m.id\r\n    JOIN station s on measurement.station_id = s.id\r\n    JOIN unit u on measurement.unit_id = u.id\r\n  WHERE measurement.id = @id",
                new[] { new QueryParameter("@id", id) },
                MeasurmentJoinedMapper
            )).FirstOrDefault();
        }

        public async Task<IEnumerable<Measurement>> FindByStationIdAsync(int stationId) {
            return await _template.QueryAsync("select * from measurement where station_id=@station_id",
                new[] { new QueryParameter("@station_id", stationId) },
                MeasurementMapper
            );
        }

        public async Task<IEnumerable<Measurement>> FindByDateRangeAsync(DateTime start, DateTime end) {
            // not necessary, converted automatically
            //string startString = start.ToString("yyyy-MM-dd HH-mm-ss");
            //string endString = end.ToString("yyyy-MM-dd HH-mm-ss");

            return await _template.QueryAsync("select * from measurement where timestamp between @start and @end",
                new[] { new QueryParameter("@start", start), new QueryParameter("@end", end) },
                MeasurementMapper
            );
        }

        public async Task<IEnumerable<Measurement>> FindByDateRangeAndStationAsync(DateTime start, DateTime end, int stationId) {
            return await _template.QueryAsync("select * from measurement where timestamp between @start and @end and station_id = @station_id",
                new[] { new QueryParameter("@start", start), new QueryParameter("@end", end), new QueryParameter("@station_id", stationId) },
                MeasurementMapper
            );
        }

        public async Task<IEnumerable<Measurement>> FindByMeasurementTypeAsync(int measurementTypeId) {
            return await _template.QueryAsync("select * from measurement where type_id=@measurementTypeId",
                new[] { new QueryParameter("@measurementTypeId", measurementTypeId) },
                MeasurementMapper
            );
        }

        public async Task<bool> AddMeasurementAsync(Measurement measurement) {
            return await _template.ExecuteAsync(
                       "INSERT INTO measurement (value, type_id, timestamp, station_id, unit_id) VALUES " +
                       "(@value, @type_id, @timestamp, @station_id, @unit_id)",
                       new[] {
                           new QueryParameter("@value", measurement.Value),
                           new QueryParameter("@type_id", measurement.TypeId),
                           new QueryParameter("@timestamp", measurement.Timestamp),
                           new QueryParameter("@station_id", measurement.StationId),
                           new QueryParameter("@unit_id", measurement.UnitId)
                       }
                   ) == 1;
        }

        public async Task<bool> UpdateMeasurementAsync(Measurement measurement) {
            return await _template.ExecuteAsync(
                       "UPDATE measurement SET value = @value, type_id = @type_id, timestamp = @timestamp, station_id = @station_id, unit_id=@unit_id WHERE id = @id ",
                       new[] {
                           new QueryParameter("@value", measurement.Value),
                           new QueryParameter("@type_id", measurement.TypeId),
                           new QueryParameter("@timestamp", measurement.Timestamp),
                           new QueryParameter("@station_id", measurement.StationId),
                           new QueryParameter("@unit_id", measurement.UnitId),
                           new QueryParameter("@id", measurement.Id)
                       }
                   ) == 1;
        }

        public async Task<bool> DeleteMeasurementAsync(Measurement measurement) {
            return await _template.ExecuteAsync(
                       "delete from measurement where id = @id",
                       new[] { new QueryParameter("@id", measurement.Id) }
                   ) == 1;
        }

        public async Task<double> GetSum(int queryMode) {
            return (await _template.QueryAsync(
                "SELECT SUM(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type",
                new[] { new QueryParameter("@type", GetQueryString(queryMode)) }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMin(int queryMode) {
            return (await _template.QueryAsync(
                "SELECT MIN(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type",
                new[] { new QueryParameter("@type", GetQueryString(queryMode)) }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMax(int queryMode) {
            return (await _template.QueryAsync(
                "SELECT MAX(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type",
                new[] { new QueryParameter("@type", GetQueryString(queryMode)) }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetAvg(int queryMode) {
            return (await _template.QueryAsync(
                "SELECT AVG(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type",
                new[] { new QueryParameter("@type", GetQueryString(queryMode)) }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsForStation(int stationId, DateTime start, DateTime end, int queryMode) {
            return (await _template.QueryAsync(
                "SELECT measurement_type.id AS mstID, measurement_type.name as mstName, unit.id AS unitID, short_name AS unitShortName, long_name AS unitLongName, measurementID, measurementValue, measurementTimestamp, measurementStationID, measurementUnitId FROM unit JOIN (SELECT id AS measurementID, value AS measurementValue, timestamp as measurementTimestamp, station_id as measurementStationID, type_id AS measurementTypeId, unit_id as measurementUnitId FROM measurement WHERE station_id = @stationId AND type_id = @queryMode AND timestamp BETWEEN @start AND @end ORDER BY timestamp DESC) AS selctedItems ON measurementUnitId = id JOIN measurement_type ON measurementTypeId = measurement_type.id",
                new[] {
                    new QueryParameter("@stationId", stationId),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@queryMode", queryMode+1),
                }, MeasurmentLatestMapper));
        }

        public async Task<IEnumerable<Measurement>> GetLastMeasurementsForStation(int stationId, int queryMode, int limitation) {
            return (await _template.QueryAsync(
                    "SELECT measurement_type.id AS mstID, measurement_type.name as mstName, unit.id AS unitID, short_name AS unitShortName, long_name AS unitLongName, measurementID, measurementValue, measurementTimestamp, measurementStationID, measurementUnitId FROM unit JOIN (SELECT id AS measurementID, value AS measurementValue, timestamp as measurementTimestamp, station_id as measurementStationID, type_id AS measurementTypeId, unit_id as measurementUnitId FROM measurement WHERE station_id = @stationId AND type_id = @queryMode ORDER BY timestamp DESC LIMIT @limitation) AS selctedItems ON measurementUnitId = id JOIN measurement_type ON measurementTypeId = measurement_type.id ORDER BY measurementTimestamp ASC",
                    new[] {
                        new QueryParameter("@queryMode", queryMode+1),
                        new QueryParameter("@stationId", stationId),
                        new QueryParameter("@limitation", limitation)
                    }, MeasurmentLatestMapper));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetSum(DateTime start, DateTime end, int queryMode, int groupByMode) {
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " SUM(value) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                        new QueryParameter("@type", GetQueryString(queryMode)),
                        new QueryParameter("@start", start),
                        new QueryParameter("@end", end)
                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetMin(DateTime start, DateTime end, int queryMode, int groupByMode) {
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " CAST(MIN(value) AS char) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)

                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetMax(DateTime start, DateTime end, int queryMode, int groupByMode) {
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " CAST(MAX(value) AS char) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)

                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetAvg(DateTime start, DateTime end, int queryMode, int groupByMode) {
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " AVG(value) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)

                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetSum(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " SUM(value) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetMin(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " CAST(MIN(value) AS char) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetMax(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " CAST(MAX(value) AS char) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }

        public async Task<IEnumerable<MeasurementAnalytic>> GetAvg(DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            GroupByStringHandler groupByStringHandler = GetGroupByString(groupByMode);
            return (await _template.QueryAsync(
                "SELECT " + groupByStringHandler.Select + " AVG(value) AS value FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end " + groupByStringHandler.GroupByOrderBy,
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, AdoMeasurementRowMapperDao.GetMapperForMode(groupByMode)));
        }



        public async Task<double> GetSum(DateTime start, DateTime end, int queryMode) {
            return (await _template.QueryAsync(
                "SELECT SUM(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMin(DateTime start, DateTime end, int queryMode) {
            return (await _template.QueryAsync(
                "SELECT MIN(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMax(DateTime start, DateTime end, int queryMode) {
            return (await _template.QueryAsync(
                "SELECT MAX(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetAvg(DateTime start, DateTime end, int queryMode) {
            return (await _template.QueryAsync(
                "SELECT AVG(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetSum(DateTime start, DateTime end, int queryMode, Station station) {
            return (await _template.QueryAsync(
                "SELECT SUM(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id = @station AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@station", station.Id)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMin(DateTime start, DateTime end, int queryMode, Station station) {
            return (await _template.QueryAsync(
                "SELECT MIN(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id = @station AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@station", station.Id)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMax(DateTime start, DateTime end, int queryMode, Station station) {
            return (await _template.QueryAsync(
                "SELECT MAX(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id = @station AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@station", station.Id)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetAvg(DateTime start, DateTime end, int queryMode, Station station) {
            return (await _template.QueryAsync(
                "SELECT AVG(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id = @station AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@station", station.Id)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetSum(DateTime start, DateTime end, int queryMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            return (await _template.QueryAsync(
                "SELECT SUM(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMin(DateTime start, DateTime end, int queryMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            return (await _template.QueryAsync(
                "SELECT MIN(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetMax(DateTime start, DateTime end, int queryMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            return (await _template.QueryAsync(
                "SELECT MAX(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, DoubleMapper)).FirstOrDefault().Value;
        }

        public async Task<double> GetAvg(DateTime start, DateTime end, int queryMode, IEnumerable<Station> stations) {
            string stationsString = GetStationsString(stations);
            return (await _template.QueryAsync(
                "SELECT AVG(value) AS result FROM measurement JOIN measurement_type m on measurement.type_id = m.id WHERE name = @type AND station_id IN (@stations) AND timestamp BETWEEN @start AND @end",
                new[] {
                    new QueryParameter("@type", GetQueryString(queryMode)),
                    new QueryParameter("@start", start),
                    new QueryParameter("@end", end),
                    new QueryParameter("@stations", stationsString)
                }, DoubleMapper)).FirstOrDefault().Value;
        }
    }
}
