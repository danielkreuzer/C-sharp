using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Wetr.Domain;
using Wetr.Server.Factory;
using Wetr.Server.Interface;

namespace Wetr.WebService.REST.Controllers {

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class MeasurementsController : ApiController {
        private IMeasurementManager MeasurementManager { get; set; } = MeasurementManagerFactory.GetMeasurementManager();
        private IStationDataManager StationManager { get; set; } = StationDataManagerFactory.GetStationDataManager();
        private ITwitterManager twitterManager { get; set; } = TwitterManagerFactory.GetTwitterManager();

        [HttpPost]
        [Route("measurements")]
        public async Task<bool> Insert([FromBody] Measurement measurement) {
            await twitterManager.SendAlarmTweet(measurement);
            return await MeasurementManager.AddNewMeasurementDataPackage(measurement);
        }

        [HttpGet]
        [Route("measurements")]
        public async Task<IEnumerable<Measurement>> GetMeasurementsForStation([FromUri] string startDateTime, [FromUri] string endDateTime,
            [FromUri] int queryMode, [FromUri] int stationId) {
            try {
                return await MeasurementManager.GetMeasurementsForStation(stationId, DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode);
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("measurements/unit")]
        public async Task<IEnumerable<Unit>> GetAllUnits() {
            try {
                return await MeasurementManager.GetAllUnits();
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("measurements/last")]
        public async Task<IEnumerable<Measurement>> GetLastMeasurementsForStation([FromUri] int queryMode, [FromUri] int stationId, [FromUri] int limit = 10) {
            try {
                return await MeasurementManager.GetLatestMeasurementsForStation(stationId, queryMode, limit);
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("measurements/sum")]
        public async Task<IEnumerable<MeasurementAnalytic>> GetMeasurementsSum([FromUri] string startDateTime, [FromUri] string endDateTime,
            [FromUri] int queryMode, [FromUri] int groupByMode, [FromUri] int? stationId = null) {
            try {
                if (stationId == null) {
                    return await MeasurementManager.Sum(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode);
                } else {
                    Station station = await StationManager.GetStationById(Convert.ToInt32(stationId));
                    IEnumerable<Station> stations = new List<Station> { station };
                    return await MeasurementManager.Sum(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode, stations);
                }
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("measurements/min")]
        public async Task<IEnumerable<MeasurementAnalytic>> GetMeasurementsMin([FromUri] string startDateTime, [FromUri] string endDateTime,
            [FromUri] int queryMode, [FromUri] int groupByMode, [FromUri] int? stationId = null) {
            try {
                if (stationId == null) {
                    return await MeasurementManager.Min(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode);
                } else {
                    Station station = await StationManager.GetStationById(Convert.ToInt32(stationId));
                    IEnumerable<Station> stations = new List<Station> { station };
                    return await MeasurementManager.Min(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode, stations);
                }
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("measurements/max")]
        public async Task<IEnumerable<MeasurementAnalytic>> GetMeasurementsMax([FromUri] string startDateTime, [FromUri] string endDateTime,
            [FromUri] int queryMode, [FromUri] int groupByMode, [FromUri] int? stationId = null) {
            try {
                if (stationId == null) {
                    return await MeasurementManager.Max(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode);
                } else {
                    Station station = await StationManager.GetStationById(Convert.ToInt32(stationId));
                    IEnumerable<Station> stations = new List<Station> { station };
                    return await MeasurementManager.Max(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode, stations);
                }
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("measurements/avg")]
        public async Task<IEnumerable<MeasurementAnalytic>> GetMeasurementsAvg([FromUri] string startDateTime, [FromUri] string endDateTime,
            [FromUri] int queryMode, [FromUri] int groupByMode, [FromUri] int? stationId = null) {
            try {
                if (stationId == null) {
                    return await MeasurementManager.Avg(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode);
                } else {
                    Station station = await StationManager.GetStationById(Convert.ToInt32(stationId));
                    IEnumerable<Station> stations = new List<Station> { station };
                    return await MeasurementManager.Avg(DateTime.Parse(startDateTime), DateTime.Parse(endDateTime), queryMode, groupByMode, stations);
                }
            }
            catch {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}