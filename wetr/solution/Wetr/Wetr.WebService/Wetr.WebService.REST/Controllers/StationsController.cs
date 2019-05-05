using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Wetr.Domain;
using Wetr.Server.Factory;

namespace Wetr.WebService.REST.Controllers {

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class StationsController : ApiController {

        [HttpGet]
        [Route("stations")]
        public async Task<IEnumerable<Station>> GetAll() {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.GetAllJoined();
        }

        [HttpGet]
        [Route("stations/types")]
        public async Task<IEnumerable<StationType>> GetAllTypes() {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.GetAllStationTypes();
        }

        [HttpPost]
        [Route("stations")]
        public async Task<bool> AddStation([FromBody]Station station) {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.AddStation(station);
        }

        [HttpGet]
        [Route("stations/{id}")]
        public async Task<Station> GetById(int id) {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.GetStationById(id);
        }

        [HttpGet]
        [Route("stations/community/{id}")]
        public async Task<IEnumerable<Station>> GetByCommunityId(int id) {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.GetStationByCommunityId(id);
        }

        [HttpPut]
        [Route("stations/{id}")]
        public async Task<bool> UpdateStation(int id, [FromBody]Station station) {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.UpdateStation(station);
        }

        [HttpDelete]
        [Route("stations/{id}")]
        public async Task<bool> DeleteStation(int id, [FromBody]Station station) {
            var manager = StationDataManagerFactory.GetStationDataManager();

            return await manager.DeleteStation(station);
        }
    }
}