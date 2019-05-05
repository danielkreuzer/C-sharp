using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IStationDao {
        Task<Station> FindByIdAsync(int id);
        Task<Station> FindJoinedByIdAsync(int id);
        Task<IEnumerable<Station>> FindJoinedAsync();
        Task<IEnumerable<Station>> FindAllAsync();
        Task<IEnumerable<Station>> FindByNameAsync(string name);
        Task<IEnumerable<Station>> FindByStationTypeIdAsync(int stationTypeId);
        Task<IEnumerable<Station>> FindByCommunityIdAsync(int communityId);
        Task<IEnumerable<Station>> FindByDistrictIdAsync(int districtId);
        Task<IEnumerable<Station>> FindByProvinceIdAsync(int provinceId);
        Task<IEnumerable<Station>> FindByCreatorIdAsync(int creatorId);
        Task<bool> UpdateAllAsync(Station station);
        Task<bool> AddStationAsync(Station station);
        Task<bool> DeleteStationAsync(Station station);
    }
}
