using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Server.Interface {
    public interface ICommunityManager {
        Task<IEnumerable<Community>> GetAllCommunities();
        Task<Community> FindCommunitiesById(int id);
        Task<IEnumerable<Community>> FindCommunitiesByZipCode(int zipCode);
        Task<IEnumerable<Community>> FindCommunitiesByName(string name);
        Task<IEnumerable<Community>> FindCommunitiesByDistrictId(int districtId);
        Task<IEnumerable<Community>> FindCommunitiesBySearchString(string search);
        Task<IEnumerable<Community>> FindCommunitiesWithStations();
        Task<IEnumerable<Community>> FindCommunitiesWithStationsBySearchString(string search);
    }
}
