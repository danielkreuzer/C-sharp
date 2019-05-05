using System;
using System.Collections;
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

    public class StationDataManager : IStationDataManager {
        private static IStationDao _iStationDao = null;
        private static IStationTypeDao _iStationTypeDao = null;
        private static ICommunityDao _iCommunityDao = null;
        private static IProvinceDao _iProvinceDao = null;
        private static IDistrictDao _iDistrictDao = null;
        private static string _connectionStringConfigName = "WetrDBConnection";

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K') {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit) {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        private static IStationDao GetIStationDao() {
            return _iStationDao ?? (_iStationDao =
                       new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        private static IStationTypeDao GetIStationTypeDao() {
            return _iStationTypeDao ?? (_iStationTypeDao =
                       new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }
        private static ICommunityDao GetICommunityDao() {
            return _iCommunityDao ?? (_iCommunityDao =
                       new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }
        private static IDistrictDao GetIDistrictDao() {
            return _iDistrictDao ?? (_iDistrictDao =
                       new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }
        private static IProvinceDao GetIProvinceDao() {
            return _iProvinceDao ?? (_iProvinceDao =
                       new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        public async Task<IEnumerable<Station>> GetAllStations() {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> GetAllJoined() {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindJoinedAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> GetStationsByName(string name) {
            try {
                if (!name.Equals("")) {
                    IStationDao stationDao = GetIStationDao();
                    return await stationDao.FindByNameAsync(name);
                }

                return null;
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> GetStationsByTypeId(int stationTypeId) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindByStationTypeIdAsync(stationTypeId);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<Station> GetStationById(int id) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindJoinedByIdAsync(id);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> GetStationByCommunityId(int communityId) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindByCommunityIdAsync(communityId);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> GetStationByCreatorId(int creator) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindByCreatorIdAsync(creator);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<bool> UpdateStation(Station station) {
            try {
                if (station != null) {
                    IStationDao stationDao = GetIStationDao();

                    if (station.CommunityId == 0 && station.Community != null) {
                        station.CommunityId = station.Community.Id;
                    }

                    if (station.TypeId == 0 && station.StationType != null) {
                        station.TypeId = station.StationType.Id;
                    }

                    if (station.Creator == 0 && station.User != null) {
                        station.Creator = station.User.Id;
                    }

                    return await stationDao.UpdateAllAsync(station);
                }

                return false;
            }
            catch (Exception) {
                return false;
            }
        }

        public async Task<bool> AddStation(Station station) {
            try {
                if (station != null) {
                    IStationDao stationDao = GetIStationDao();

                    if (station.CommunityId == 0 && station.Community != null) {
                        station.CommunityId = station.Community.Id;
                    }

                    if (station.TypeId == 0 && station.StationType != null) {
                        station.TypeId = station.StationType.Id;
                    }

                    if (station.Creator == 0 && station.User != null) {
                        station.Creator = station.User.Id;
                    }

                    return await stationDao.AddStationAsync(station);
                }

                return false;
            }
            catch (Exception) {
                return false;
            }
        }

        public async Task<bool> DeleteStation(Station station) {
            // Catch exeption faster than check if measurements are in db!!!
            try {
                if (station != null) {
                    IStationDao stationDao = GetIStationDao();
                    return await stationDao.DeleteStationAsync(station);
                }

                return false;
            }
            catch (Exception) {
                return false;
            }
        }

        public async Task<IEnumerable<StationType>> GetAllStationTypes() {
            try {
                IStationTypeDao stationDao = GetIStationTypeDao();
                return await stationDao.FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Community>> GetAllCommunities() {
            try {
                ICommunityDao stationDao = GetICommunityDao();
                return await stationDao.FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<District>> GetAllDistricts() {
            try {
                IDistrictDao stationDao = GetIDistrictDao();
                return await stationDao.FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Province>> GetAllProvinces() {
            try {
                IProvinceDao stationDao = GetIProvinceDao();
                return await stationDao.FindAllAsync();
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> FilterByRegion(GeoCoordinate geoCoordinate, double radius = 0) {
            try {
                IStationDao stationDao = GetIStationDao();
                IEnumerable<Station> stations = await stationDao.FindAllAsync();
                List<Station> returnStations = new List<Station>();
                foreach (Station station in stations) {
                    if (DistanceTo(geoCoordinate.Latitude, geoCoordinate.Longitude, station.Latitude,
                            station.Longitude) <= radius) {
                        returnStations.Add(station);
                    }
                }

                return returnStations;
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> FilterByRegion(Community community) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindByCommunityIdAsync(community.Id);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> FilterByRegion(District district) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindByDistrictIdAsync(district.Id);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<IEnumerable<Station>> FilterByRegion(Province province) {
            try {
                IStationDao stationDao = GetIStationDao();
                return await stationDao.FindByProvinceIdAsync(province.Id);
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
