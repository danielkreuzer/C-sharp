using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wetr.Domain;
using Wetr.Server.Interface;

namespace Wetr.Server.Test {
    [TestClass]
    public class StationManagerTest {

        private static IStationDataManager GetStationDataManager() {
            var mock = new Mock<IStationDataManager>();

            var stations = new List<Station> { new Station() };

            mock.Setup(m => m.GetAllStations()).ReturnsAsync(() => stations);

            mock.Setup(m => m.GetStationsByName(It.IsAny<string>())).ReturnsAsync((string name) => stations);

            mock.Setup(m => m.GetStationsByTypeId(It.IsAny<int>())).ReturnsAsync((int id) => stations);

            mock.Setup(m => m.GetStationById(It.IsAny<int>())).ReturnsAsync((int id) => new Station());

            mock.Setup(m => m.GetStationByCommunityId(It.IsAny<int>())).ReturnsAsync((int communityId) => stations);

            mock.Setup(m => m.GetStationByCreatorId(It.IsAny<int>())).ReturnsAsync((int creator) => stations);

            mock.Setup(m => m.UpdateStation(It.IsNotNull<Station>())).ReturnsAsync((Station station) => true);
            mock.Setup(m => m.UpdateStation(null)).ReturnsAsync((Station station) => false);

            mock.Setup(m => m.AddStation(It.IsNotNull<Station>())).ReturnsAsync((Station station) => true);
            mock.Setup(m => m.AddStation(null)).ReturnsAsync((Station station) => false);

            mock.Setup(m => m.DeleteStation(It.IsNotNull<Station>())).ReturnsAsync((Station station) => true);
            mock.Setup(m => m.DeleteStation(null)).ReturnsAsync((Station station) => false);

            mock.Setup(m => m.GetAllStationTypes()).ReturnsAsync(() => new List<StationType> { new StationType() });

            mock.Setup(m => m.GetAllCommunities()).ReturnsAsync(() => new List<Community> { new Community() });
            mock.Setup(m => m.GetAllDistricts()).ReturnsAsync(() => new List<District> { new District() });
            mock.Setup(m => m.GetAllProvinces()).ReturnsAsync(() => new List<Province> { new Province() });

            mock.Setup(m => m.FilterByRegion(It.IsNotNull<GeoCoordinate>(), It.IsNotNull<double>())).ReturnsAsync((GeoCoordinate g, double d) => stations);
            mock.Setup(m => m.FilterByRegion(null, It.IsNotNull<double>())).ReturnsAsync((GeoCoordinate g, double d) => new List<Station>());

            mock.Setup(m => m.FilterByRegion(It.IsNotNull<Community>())).ReturnsAsync((Community community) => stations);

            mock.Setup(m => m.FilterByRegion(It.IsNotNull<District>())).ReturnsAsync((District district) => stations);

            mock.Setup(m => m.FilterByRegion(It.IsNotNull<Province>())).ReturnsAsync((Province province) => stations);

            return mock.Object;
        }

        [TestMethod]
        public async Task GetAllStations() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetAllStations()).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetStationsByName() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetStationsByName("name")).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetStationsByTypeId() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetStationsByTypeId(0)).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetStationByCommunityId() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetStationByCommunityId(0)).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetStationByCreatorId() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetStationByCreatorId(0)).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task UpdateStation() {
            var stationManager = GetStationDataManager();

            Assert.IsTrue(await stationManager.UpdateStation(new Station()));

            Assert.IsFalse(await stationManager.UpdateStation(null));
        }

        [TestMethod]
        public async Task AddStation() {
            var stationManager = GetStationDataManager();

            Assert.IsTrue(await stationManager.AddStation(new Station()));

            Assert.IsFalse(await stationManager.AddStation(null));
        }

        [TestMethod]
        public async Task DeleteStation() {
            var stationManager = GetStationDataManager();

            Assert.IsTrue(await stationManager.DeleteStation(new Station()));

            Assert.IsFalse(await stationManager.DeleteStation(null));
        }

        [TestMethod]
        public async Task GetAllStationTypes() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetAllStationTypes()).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetAllCommunities() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetAllCommunities()).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetAllDistricts() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetAllDistricts()).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task GetAllProvinces() {
            var stationManager = GetStationDataManager();

            var b = (await stationManager.GetAllProvinces()).Any();
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task FilterByRegion() {
            var stationManager = GetStationDataManager();

            var b1 = (await stationManager.FilterByRegion(new GeoCoordinate(0, 0), 0));
            Assert.IsNotNull(b1);

            var b2 = (await stationManager.FilterByRegion(null, 0)).Any();
            Assert.IsFalse(b2);

            var b3 = (await stationManager.FilterByRegion(new Community())).Any();
            Assert.IsTrue(b3);

            var b4 = (await stationManager.FilterByRegion(new District())).Any();
            Assert.IsTrue(b4);

            var b5 = (await stationManager.FilterByRegion(new Province())).Any();
            Assert.IsTrue(b5);
        }
    }
}
