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
    public class CommunityManager : ICommunityManager {
        private static ICommunityDao _communityDao;

        private static string _connectionStringConfigName = "WetrDBConnection";

        private static ICommunityDao GetCommunityDao() {
            return _communityDao ?? (_communityDao =
                       new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        public async Task<IEnumerable<Community>> GetAllCommunities() {
            try {
                return await GetCommunityDao().FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<Community> FindCommunitiesById(int id) {
            try {
                return await GetCommunityDao().FindByIdAsync(id);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> FindCommunitiesByZipCode(int zipCode) {
            try {
                return await GetCommunityDao().FindByZipCodeAsync(zipCode);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> FindCommunitiesByName(string name) {
            try {
                return await GetCommunityDao().FindByNameAsync(name);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> FindCommunitiesByDistrictId(int districtId) {
            try {
                return await GetCommunityDao().FindByDistrictIdAsync(districtId);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> FindCommunitiesBySearchString(string searchString) {
            try {
                var communities = await GetAllCommunities();

                if (searchString != null) {
                    return communities.Where(e => e.ZipName.ToLower().Contains(searchString.ToLower()));
                }

                return communities;
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> FindCommunitiesWithStations() {
            try {
                return await GetCommunityDao().FindAllWithStationsAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> FindCommunitiesWithStationsBySearchString(string searchString) {
            try {
                var communities = await FindCommunitiesWithStations();

                if (searchString != null) {
                    return communities.Where(e => e.ZipName.ToLower().Contains(searchString.ToLower()));
                }

                return communities;
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
