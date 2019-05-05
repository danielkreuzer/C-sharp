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
    public class MeasurementManagerTest {

        private static IMeasurementManager GetMeasurementManager() {
            var mock = new Mock<IMeasurementManager>();

            var measurementAnalytics = new List<MeasurementAnalytic> { new MeasurementAnalytic(0) };

            mock.Setup(m => m.Sum(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode) => measurementAnalytics);

            mock.Setup(m => m.Min(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode) => measurementAnalytics);

            mock.Setup(m => m.Max(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode) => measurementAnalytics);

            mock.Setup(m => m.Avg(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode) => measurementAnalytics);


            mock.Setup(m => m.Sum(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<IEnumerable<Station>>(s => s.Any())))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) => measurementAnalytics);

            mock.Setup(m => m.Min(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<IEnumerable<Station>>(s => s.Any())))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) => measurementAnalytics);

            mock.Setup(m => m.Max(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<IEnumerable<Station>>(s => s.Any())))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) => measurementAnalytics);

            mock.Setup(m => m.Avg(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<IEnumerable<Station>>(s => s.Any())))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) => measurementAnalytics);

            // Test with empty stations list
            mock.Setup(m => m.Avg(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.Is<IEnumerable<Station>>(s => !s.Any())))
                .ReturnsAsync((DateTime start, DateTime end, int queryMode, int groupByMode, IEnumerable<Station> stations) => new List<MeasurementAnalytic>());

            return mock.Object;
        }

        [TestMethod]
        public async Task Aggregates() {
            var measurementManager = GetMeasurementManager();

            Assert.IsTrue((await measurementManager.Sum(DateTime.Now, DateTime.Now, 0, 0)).Any());

            Assert.IsTrue((await measurementManager.Min(DateTime.Now, DateTime.Now, 0, 0)).Any());

            Assert.IsTrue((await measurementManager.Max(DateTime.Now, DateTime.Now, 0, 0)).Any());

            Assert.IsTrue((await measurementManager.Avg(DateTime.Now, DateTime.Now, 0, 0)).Any());
        }

        [TestMethod]
        public async Task AggregatesForStations() {
            var measurementManager = GetMeasurementManager();

            var stations = new List<Station> { new Station() };
            var emptyStations = new List<Station>();

            Assert.IsTrue((await measurementManager.Sum(DateTime.Now, DateTime.Now, 0, 0, stations)).Any());

            Assert.IsTrue((await measurementManager.Min(DateTime.Now, DateTime.Now, 0, 0, stations)).Any());

            Assert.IsTrue((await measurementManager.Max(DateTime.Now, DateTime.Now, 0, 0, stations)).Any());

            Assert.IsTrue((await measurementManager.Avg(DateTime.Now, DateTime.Now, 0, 0, stations)).Any());


            Assert.IsFalse((await measurementManager.Avg(DateTime.Now, DateTime.Now, 0, 0, emptyStations)).Any());
        }
    }
}
