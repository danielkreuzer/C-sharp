using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Ado;
using Wetr.Dal.Interface;
using Wetr.Domain;
using Wetr.Server.Interface;

namespace Wetr.Server.Implementation {
    public class MeasurementTypeManager : IMeasurementTypeManager {
        private static IMeasurementTypeDao _measurementTypeDao;
        private static string _connectionStringConfigName = "WetrDBConnection";

        private static IMeasurementTypeDao GetMeasurementTypeDao() {
            return _measurementTypeDao ?? (_measurementTypeDao =
                       new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        public async Task<IEnumerable<MeasurementType>> GetAllMeasurementTypes() {
            try {
                return await GetMeasurementTypeDao().FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<MeasurementType> FindMeasurementTypesById(int id) {
            try {
                return await GetMeasurementTypeDao().FindByIdAsync(id);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<MeasurementType> FindMeasurementTypesByName(string name) {
            try {
                return await GetMeasurementTypeDao().FindByNameAsync(name);
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
