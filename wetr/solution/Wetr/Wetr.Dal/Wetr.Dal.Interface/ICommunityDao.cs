using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface ICommunityDao {
        Task<IEnumerable<Community>> FindAllAsync();
        Task<IEnumerable<Community>> FindAllWithStationsAsync();
        Task<Community> FindByIdAsync(int id);
        Task<IEnumerable<Community>> FindByZipCodeAsync(int zipCode);
        Task<IEnumerable<Community>> FindByNameAsync(string name);
        Task<IEnumerable<Community>> FindByDistrictIdAsync(int districtId);
        Task<bool> AddCommunityAsync(Community community);
        Task<bool> UpdateCommunityAsync(Community community);
        Task<bool> DeleteCommunityAsync(Community community);
    }
}
