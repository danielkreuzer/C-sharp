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
    public class MeasurementTypesController : ApiController {

        [HttpGet]
        [Route("measurementtypes")]
        public async Task<IEnumerable<MeasurementType>> GetAll() {
            var manager = MeasurementTypeManagerFactory.GetMeasurementTypeManager();

            return await manager.GetAllMeasurementTypes();
        }
    }
}