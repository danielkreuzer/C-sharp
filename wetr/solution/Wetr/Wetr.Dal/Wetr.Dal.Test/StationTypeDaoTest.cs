using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wetr.Dal.Ado;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Test {
    [TestClass]
    public class StationTypeDaoTest {
        private string _connectionStringConfigName = "WetrDBConnection";

        [TestMethod]
        public async Task FindAll() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            IEnumerable<StationType> stationTypes = await stationTypeDao.FindAllAsync();
            int count = stationTypes.Count();

            Assert.IsTrue(count > 1);
        }

        [TestMethod]
        public async Task FindById() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            StationType stationType = await stationTypeDao.FindByIdAsync(1);

            Assert.IsTrue(stationType != null && stationType.Id == 1);
        }

        [TestMethod]
        public async Task FindByManufacturerModel() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            IEnumerable<StationType> stationTypes = await stationTypeDao.FindByManufacturerModelAsync("SKEY", "1");
            StationType stationType = stationTypes.FirstOrDefault();

            Assert.IsTrue(stationType != null && stationType.Id == 1);
        }

        [TestMethod]
        public async Task FindByManufacturer() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            IEnumerable<StationType> stationTypes = await stationTypeDao.FindByManufacturerAsync("SKEY");
            StationType stationType = stationTypes.FirstOrDefault();

            Assert.IsTrue(stationType != null && stationType.Id == 1);
        }

        [TestMethod]
        public async Task Update() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            StationType stationType = await stationTypeDao.FindByIdAsync(1);
            string manufacturer = stationType.Manufacturer;
            stationType.Manufacturer = "Test";
            bool update1 = await stationTypeDao.UpdateAllAsync(stationType);
            Assert.IsTrue(update1);

            StationType stationType1 = await stationTypeDao.FindByIdAsync(1);
            Assert.IsTrue(stationType1.Manufacturer == "Test");

            stationType.Manufacturer = manufacturer;
            bool update2 = await stationTypeDao.UpdateAllAsync(stationType);
            Assert.IsTrue(update2);

            StationType stationType2 = await stationTypeDao.FindByIdAsync(1);
            Assert.IsTrue(stationType2.Manufacturer == manufacturer);
        }

        [TestMethod]
        public async Task Add() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            StationType stationType = new StationType("Test", "Test");

            bool insert = await stationTypeDao.AddStationTypeAsync(stationType);
            Assert.IsTrue(insert);

            StationType stationType1 =
                (await stationTypeDao.FindByManufacturerModelAsync(stationType.Manufacturer, stationType.Model)).FirstOrDefault();

            Assert.IsTrue(stationType1 != null && stationType1.Model == stationType.Model);

            bool delete = await stationTypeDao.DeleteStationTypeAsync(stationType1);
            Assert.IsTrue(delete);
        }

        [TestMethod]
        public async Task Delete() {
            IStationTypeDao stationTypeDao =
                new AdoStationTypeDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            StationType stationType = new StationType("Test2", "Test2");

            bool insert = await stationTypeDao.AddStationTypeAsync(stationType);
            Assert.IsTrue(insert);

            StationType stationType1 =
                (await stationTypeDao.FindByManufacturerModelAsync(stationType.Manufacturer, stationType.Model)).FirstOrDefault();

            Assert.IsTrue(stationType1 != null && stationType1.Model == stationType.Model);

            bool delete = await stationTypeDao.DeleteStationTypeAsync(stationType1);
            Assert.IsTrue(delete);

            StationType stationType2 = (await stationTypeDao.FindByManufacturerModelAsync(stationType.Manufacturer, stationType.Model)).FirstOrDefault();
            Assert.IsTrue(stationType2 == null);
        }
    }
}
