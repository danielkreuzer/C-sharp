using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IProvinceDao {
        Task<IEnumerable<Province>> FindAllAsync();
        Task<Province> FindByIdAsync(int id);
        Task<IEnumerable<Province>> FindByNameAsync(string name);
        Task<bool> AddProvinceAsync(Province province);
        Task<bool> UpdateProvinceAsync(Province province);
        Task<bool> DeleteProvinceAsync(Province province);
    }
}
