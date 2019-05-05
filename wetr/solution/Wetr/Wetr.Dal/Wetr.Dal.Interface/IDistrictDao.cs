using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IDistrictDao {
        Task<IEnumerable<District>> FindAllAsync();
        Task<District> FindByIdAsync(int id);
        Task<IEnumerable<District>> FindByNameAsync(string name);
        Task<IEnumerable<District>> FindByProvinceIdAsync(int provinceId);
        Task<bool> AddDistrictAsync(District district);
        Task<bool> UpdateDistrictAsync(District district);
        Task<bool> DeleteDistrictAsync(District district);
    }
}
