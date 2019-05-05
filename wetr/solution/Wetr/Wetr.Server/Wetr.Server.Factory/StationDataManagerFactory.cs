using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Server.Interface;
using Wetr.Server.Implementation;

namespace Wetr.Server.Factory {
    public static class StationDataManagerFactory {
        private static IStationDataManager _stationDataManager;

        public static IStationDataManager GetStationDataManager() {
            return _stationDataManager ?? (_stationDataManager = new StationDataManager());
        }
    }
}
