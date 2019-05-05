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
    public class MeasurementTypeDaoTest {
        private static readonly string configName = "WetrDBConnection";

        [TestMethod]
        public async Task TestFindAll() {
            IMeasurementTypeDao measurementTypeDao = new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<MeasurementType> measurementTypes = await measurementTypeDao.FindAllAsync();
            Assert.IsTrue(measurementTypes.Any());
        }

        [TestMethod]
        public async Task TestFindById() {
            IMeasurementTypeDao measurementTypeDao = new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(configName));

            MeasurementType measurementType1 = await measurementTypeDao.FindByIdAsync(1);
            Assert.IsNotNull(measurementType1);

            MeasurementType measurementType100 = await measurementTypeDao.FindByIdAsync(100);
            Assert.IsNull(measurementType100);
        }

        [TestMethod]
        public async Task TestFindByName()
        {
            IMeasurementTypeDao measurementTypeDao = new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(configName));

            MeasurementType measurementType1 = await measurementTypeDao.FindByNameAsync("temperature");
            Assert.IsNotNull(measurementType1);

            MeasurementType measurementType2 = await measurementTypeDao.FindByNameAsync("abcdefg");
            Assert.IsNull(measurementType2);
        }

        [TestMethod]
        public async Task Add() {
            IMeasurementTypeDao measurementTypeDao = new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(configName));

            MeasurementType measurement = new MeasurementType("Test");

            bool insert = await measurementTypeDao.AddMeasurementTypeAsync(measurement);
            Assert.IsTrue(insert);

            measurement = await measurementTypeDao.FindByNameAsync("Test");
            Assert.IsNotNull(measurement);

            await measurementTypeDao.DeleteMeasurementTypeAsync(measurement);
        }

        [TestMethod]
        public async Task Update() {
            IMeasurementTypeDao measurementTypeDao = new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(configName));

            MeasurementType measurementType = await measurementTypeDao.FindByIdAsync(1);

            string originalName = measurementType.Name;
            measurementType.Name = "New name";
            bool update1 = await measurementTypeDao.UpdateMeasurementTypeAsync(measurementType);
            Assert.IsTrue(update1);

            measurementType = await measurementTypeDao.FindByIdAsync(1);
            Assert.AreEqual(measurementType.Name, "New name");

            measurementType.Name = originalName;
            bool update2 = await measurementTypeDao.UpdateMeasurementTypeAsync(measurementType);
            Assert.IsTrue(update2);

            measurementType = await measurementTypeDao.FindByIdAsync(1);
            Assert.AreEqual(measurementType.Name, originalName);
        }

        [TestMethod]
        public async Task Delete() {
            IMeasurementTypeDao measurementTypeDao = new AdoMeasurementTypeDao(DefaultConnectionFactory.FromConfiguration(configName));

            MeasurementType measurement = new MeasurementType("Test");

            bool insert = await measurementTypeDao.AddMeasurementTypeAsync(measurement);
            Assert.IsTrue(insert);

            measurement = await measurementTypeDao.FindByNameAsync("Test");
            Assert.IsNotNull(measurement);

            bool deleted = await measurementTypeDao.DeleteMeasurementTypeAsync(measurement);
            Assert.IsTrue(deleted);

            measurement = await measurementTypeDao.FindByNameAsync("Test");
            Assert.IsNull(measurement);
        }
    }
}
