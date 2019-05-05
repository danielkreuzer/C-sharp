using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wetr.Dal.Interface;
using Wetr.Dal.Ado;
using Wetr.Domain;

namespace Wetr.Test {
    [TestClass]
    public class StationDaoTest {
        private string _connectionStringConfigName = "WetrDBConnection";

        [TestMethod]
        public async Task FindAll() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            IEnumerable<Station> stations = await stationDao.FindAllAsync();
            int count = stations.Count();

            Assert.IsTrue(count > 1);
        }

        [TestMethod]
        public async Task FindById() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = await stationDao.FindByIdAsync(1);

            Assert.IsTrue(station != null && station.Id == 1);
        }

        [TestMethod]
        public async Task FindByJoinedId() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = await stationDao.FindJoinedByIdAsync(1);

            Assert.IsTrue(station != null && station.Id == 1);
        }

        [TestMethod]
        public async Task FindByName() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = (await stationDao.FindByNameAsync("ANDAU")).FirstOrDefault();

            Assert.IsTrue(station != null && station.Name == "ANDAU");
        }

        [TestMethod]
        public async Task FindByStationTypeId() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = (await stationDao.FindByStationTypeIdAsync(1)).FirstOrDefault();

            Assert.IsTrue(station != null && station.TypeId == 1);
        }

        [TestMethod]
        public async Task FindByCommunityId() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = (await stationDao.FindByCommunityIdAsync(85)).FirstOrDefault();

            Assert.IsTrue(station != null && station.CommunityId == 85);
        }

        [TestMethod]
        public async Task FindByDistrictId() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = (await stationDao.FindByDistrictIdAsync(107)).FirstOrDefault();

            Assert.IsTrue(station != null && station.CommunityId == 85);
        }

        [TestMethod]
        public async Task FindByProvinceId() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = (await stationDao.FindByProvinceIdAsync(1)).FirstOrDefault();

            Assert.IsTrue(station != null && station.CommunityId == 85);
        }

        [TestMethod]
        public async Task FindByCreator() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            IEnumerable<Station> stations = await stationDao.FindByCreatorIdAsync(1);

            Assert.IsTrue(stations.Any());

            IEnumerable<Station> stations2 = await stationDao.FindByCreatorIdAsync(1231234123);

            Assert.IsFalse(stations2.Any());
        }

        [TestMethod]
        public async Task Update() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = await stationDao.FindByIdAsync(1);
            string name = station.Name;
            station.Name = "TEST UPDATE";
            bool update1 = await stationDao.UpdateAllAsync(station);
            Assert.IsTrue(update1);

            Station station1 = await stationDao.FindByIdAsync(1);
            Assert.IsTrue(station1.Name == "TEST UPDATE");

            station.Name = name;
            bool update2 = await stationDao.UpdateAllAsync(station);
            Assert.IsTrue(update2);

            Station station2 = await stationDao.FindByIdAsync(1);
            Assert.IsTrue(station2.Name == name);

        }

        [TestMethod]
        public async Task Add() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = new Station("Test!!", 1, 42.111, 42.111, 85, 122.4, 1);
            bool insert1 = await stationDao.AddStationAsync(station);
            Assert.IsTrue(insert1);

            Station station1 = (await stationDao.FindByNameAsync("Test!!")).FirstOrDefault();
            Assert.IsTrue(station1 != null && station1.Name == station.Name);

            bool delete = await stationDao.DeleteStationAsync(station1);
            Assert.IsTrue(delete);
        }

        [TestMethod]
        public async Task Delete() {
            IStationDao stationDao =
                new AdoStationDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            Station station = new Station("Test2!!", 1, 42.111F, 42.111, 85, 122.4, 1);
            bool insert1 = await stationDao.AddStationAsync(station);
            Assert.IsTrue(insert1);

            Station station1 = (await stationDao.FindByNameAsync("Test2!!")).FirstOrDefault();
            Assert.IsTrue(station1 != null && station1.Name == station.Name);

            bool delete = await stationDao.DeleteStationAsync(station1);
            Assert.IsTrue(delete);

            Station station2 = (await stationDao.FindByNameAsync("Test2!!")).FirstOrDefault();
            Assert.IsTrue(station2 == null);
        }
    }
}
