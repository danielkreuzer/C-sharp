using System;
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
    public class MeasurementDaoTest {
        private static readonly string configName = "WetrDBConnection";

        [TestMethod]
        public async Task TestFindAll() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Measurement> measurements = await measurementDao.FindAllAsync();
            Assert.IsTrue(measurements.Any());
        }

        [TestMethod]
        public async Task TestFindById() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            Measurement measurement1 = await measurementDao.FindByIdAsync(1);
            Assert.IsNotNull(measurement1);

            Measurement measurement0 = await measurementDao.FindByIdAsync(0);
            Assert.IsNull(measurement0);
        }

        [TestMethod]
        public async Task TestFindJoinedById() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            Measurement measurement1 = await measurementDao.FindJoinedByIdAsync(1);
            Assert.IsNotNull(measurement1);

            Measurement measurement0 = await measurementDao.FindJoinedByIdAsync(0);
            Assert.IsNull(measurement0);
        }

        [TestMethod]
        public async Task TestFindByStationId() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Measurement> measurements = await measurementDao.FindByStationIdAsync(1);
            Assert.IsTrue(measurements.Any());
        }

        [TestMethod]
        public async Task TestFindByDateRange() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            DateTime start = new DateTime(2016, 10, 28, 0, 0, 0);
            DateTime end = new DateTime(2017, 02, 10, 0, 0, 0);

            IEnumerable<Measurement> measurements = await measurementDao.FindByDateRangeAsync(start, end);
            Assert.IsTrue(measurements.Any());

            start = new DateTime(2000, 01, 01, 0, 0, 0);
            end = new DateTime(2005, 03, 5, 0, 0, 0);

            measurements = await measurementDao.FindByDateRangeAsync(start, end);
            Assert.IsFalse(measurements.Any());
        }

        [TestMethod]
        public async Task FindByDateRangeAndStation() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            DateTime start = new DateTime(2016, 10, 28, 0, 0, 0);
            DateTime end = new DateTime(2017, 02, 10, 0, 0, 0);

            IEnumerable<Measurement> measurements = await measurementDao.FindByDateRangeAndStationAsync(start, end, 1);
            Assert.IsTrue(measurements.Any());

            start = new DateTime(2000, 01, 01, 0, 0, 0);
            end = new DateTime(2005, 03, 5, 0, 0, 0);

            measurements = await measurementDao.FindByDateRangeAndStationAsync(start, end, 1);
            Assert.IsFalse(measurements.Any());
        }

        [TestMethod]
        public async Task FindByMeasurementType() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Measurement> measurements = await measurementDao.FindByMeasurementTypeAsync(1);
            Assert.IsTrue(measurements.Any());
        }

        [TestMethod]
        public async Task Add() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            Measurement measurement = new Measurement(12.13, 1, new DateTime(2000, 01, 02), 1, 1);

            bool inserted = await measurementDao.AddMeasurementAsync(measurement);
            Assert.IsTrue(inserted);

            measurement = (await measurementDao.FindByDateRangeAsync(new DateTime(2000, 01, 01), new DateTime(2000, 01, 03))).FirstOrDefault();
            Assert.IsTrue(measurement != null);

            await measurementDao.DeleteMeasurementAsync(measurement);
        }
        
        [TestMethod]
        public async Task Update() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            Measurement measurement = await measurementDao.FindByIdAsync(1);

            double originalTemperature = measurement.Value;
            measurement.Value = 100;
            bool update1 = await measurementDao.UpdateMeasurementAsync(measurement);
            Assert.IsTrue(update1);

            measurement = await measurementDao.FindByIdAsync(1);
            Assert.AreEqual(measurement.Value, 100);

            measurement.Value = originalTemperature;
            bool update2 = await measurementDao.UpdateMeasurementAsync(measurement);
            Assert.IsTrue(update2);

            measurement = await measurementDao.FindByIdAsync(1);
            Assert.AreEqual(measurement.Value, originalTemperature);
        }
        
        [TestMethod]
        public async Task Delete() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            Measurement measurement = new Measurement(12.13, 1, new DateTime(2000, 01, 02), 1, 1);

            bool inserted = await measurementDao.AddMeasurementAsync(measurement);
            Assert.IsTrue(inserted);

            measurement = (await measurementDao.FindByDateRangeAsync(new DateTime(2000, 01, 01), new DateTime(2000, 01, 03))).FirstOrDefault();
            bool deleted = await measurementDao.DeleteMeasurementAsync(measurement);
            Assert.IsTrue(deleted);

            measurement = (await measurementDao.FindByDateRangeAsync(new DateTime(2000, 01, 01), new DateTime(2000, 01, 03))).FirstOrDefault();
            Assert.IsNull(measurement);
        }

        [TestMethod]
        public async Task GetSum() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));
            double value = await measurementDao.GetSum(QueryMode.temperature);
            Assert.IsTrue(value > 1000000);

            MeasurementAnalytic measurementAnalytic = (await measurementDao.GetSum(
                new DateTime(2016, 11, 12, 00, 15, 05), new DateTime(2016, 11, 16, 14, 15, 07), QueryMode.temperature,
                GroupByMode.none)).FirstOrDefault();
            Assert.IsTrue(measurementAnalytic.Value < 1000);

            List<Station> test = new List<Station>();
            test.Add(new Station(1, "", 1, 1, 1, 1, 1, 1));
            test.Add(new Station(2, "", 1, 1, 1, 1, 1, 1));

            MeasurementAnalytic measurementAnalytic1 = (await measurementDao.GetSum(
                new DateTime(2016, 11, 12, 00, 15, 05), new DateTime(2016, 11, 16, 14, 15, 07), QueryMode.temperature,
                GroupByMode.none, test)).FirstOrDefault();
            Assert.IsTrue(measurementAnalytic1.Value > 50);

            //station_id IN (1,5) AND timestamp BETWEEN "2016-08-30 00:15:10" AND "2016-09-04 21:15:09"
            List<Station> test1 = new List<Station>();
            test1.Add(new Station(1, "", 1, 1, 1, 1, 1, 1));
            test1.Add(new Station(5, "", 1, 1, 1, 1, 1, 1));

            IEnumerable<MeasurementAnalytic> measurementAnalytic2 = (await measurementDao.GetSum(
                new DateTime(2016, 08, 13, 00, 15, 10), new DateTime(2016, 9, 4, 21, 15, 9), QueryMode.temperature,
                GroupByMode.hour, test1));
            Assert.IsTrue(measurementAnalytic2.First().Value > 13);
        }

        [TestMethod]
        public async Task TestFindLatestMeasurementsForStation() {
            IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Measurement> measurements = await measurementDao.GetLastMeasurementsForStation(47,0,10);
            Assert.IsTrue(measurements.Any());
        }

        //[TestMethod]
        //public async Task GetSum() {
        //    IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));
        //    double result = await measurementDao.GetSum(QueryMode.temperature);
        //    Assert.IsTrue(result > 1000000);
        //}

        //[TestMethod]
        //public async Task GetSumWithParams1() {
        //    IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));
        //    double result = await measurementDao.GetSum(new DateTime(2016,11,12,00,15,05), new DateTime(2016, 11, 16, 14, 15, 07), QueryMode.temperature);
        //    Assert.IsTrue(result < 1000);
        //}

        //[TestMethod]
        //public async Task GetSumWithParams2() {
        //    List<Station> test = new List<Station>();
        //    test.Add(new Station(1, "", 1, 1, 1, 1, 1, 1));
        //    test.Add(new Station(2, "", 1, 1, 1, 1, 1, 1));
        //    IMeasurementDao measurementDao = new AdoMeasurementDao(DefaultConnectionFactory.FromConfiguration(configName));
        //    double result = await measurementDao.GetSum(new DateTime(2016, 11, 12, 00, 15, 05), new DateTime(2016, 11, 16, 14, 15, 07), QueryMode.temperature, test);
        //    Assert.IsTrue(result > 50);
        //}
    }
}
