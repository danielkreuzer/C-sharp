using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Wetr.Domain;
using Wetr.Server.Factory;

namespace Wetr.WebService.REST.Controllers {

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class CommunitiesController : ApiController {

        [HttpGet]
        [Route("communities")]
        public async Task<IEnumerable<Community>> GetAll() {
            var manager = CommunityManagerFactory.GetCommunityManager();

            return await manager.GetAllCommunities();
        }

        [HttpGet]
        [Route("communities/name")]
        public async Task<IEnumerable<Community>> GetByName([FromUri] string name) {
            var manager = CommunityManagerFactory.GetCommunityManager();

            return await manager.FindCommunitiesByName(name);
        }

        [HttpGet]
        [Route("communities/stations")]
        public async Task<IEnumerable<Community>> GetStations([FromUri] string q = null) {
            var manager = CommunityManagerFactory.GetCommunityManager();

            if (q != null) {
                return await manager.FindCommunitiesWithStationsBySearchString(q);
            }

            return await manager.FindCommunitiesWithStations();
        }
    }
}