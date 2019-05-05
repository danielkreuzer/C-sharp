using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Server.Interface {
    public interface IStationDataManager {
        Task<IEnumerable<Station>> GetAllStations();
        Task<IEnumerable<Station>> GetAllJoined();
        Task<IEnumerable<Station>> GetStationsByName(string name);
        Task<IEnumerable<Station>> GetStationsByTypeId(int stationTypeId);
        Task<Station> GetStationById(int id);
        Task<IEnumerable<Station>> GetStationByCommunityId(int communityId);
        Task<IEnumerable<Station>> GetStationByCreatorId(int creator);
        Task<bool> UpdateStation(Station station);
        Task<bool> AddStation(Station station);
        Task<bool> DeleteStation(Station station);
        Task<IEnumerable<StationType>> GetAllStationTypes();
        Task<IEnumerable<Community>> GetAllCommunities();
        Task<IEnumerable<District>> GetAllDistricts();
        Task<IEnumerable<Province>> GetAllProvinces();

        /*
         * Filtern nach Wetterstation bzw. Region: Eine Region ist durch einen Punkt und einen Radius
         * definiert, kann aber auch eine Gemeinde, ein Bezirk oder ein Bundesland sein.
         */

        Task<IEnumerable<Station>> FilterByRegion(GeoCoordinate geoCoordinate, double radius = 0.0);
        Task<IEnumerable<Station>> FilterByRegion(Community community);
        Task<IEnumerable<Station>> FilterByRegion(District district);
        Task<IEnumerable<Station>> FilterByRegion(Province province);
        
    }
}
