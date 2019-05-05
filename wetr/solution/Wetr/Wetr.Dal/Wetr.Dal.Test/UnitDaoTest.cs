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
    public class UnitDaoTest {
        private static readonly string configName = "WetrDBConnection";

        [TestMethod]
        public async Task TestFindAll() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Unit> units = await unitDao.FindAllAsync();
            Assert.IsTrue(units.Any());
        }

        [TestMethod]
        public async Task TestFindById() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            Unit unit1 = await unitDao.FindByIdAsync(1);
            Assert.IsNotNull(unit1);

            Unit unit100 = await unitDao.FindByIdAsync(100);
            Assert.IsNull(unit100);
        }

        [TestMethod]
        public async Task TestFindByShortName() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Unit> units = await unitDao.FindByShortNameAsync("km/h");
            Assert.IsNotNull(units.Any());
        }

        [TestMethod]
        public async Task TestFindByLongName() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Unit> units = await unitDao.FindByLongNameAsync("Percent");
            Assert.IsNotNull(units.Any());
        }

        [TestMethod]
        public async Task Add() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            Unit unit = new Unit("short_test", "long test name");

            bool inserted = await unitDao.AddUnitAsync(unit);
            Assert.IsTrue(inserted);

            unit = (await unitDao.FindByShortNameAsync("short_test")).FirstOrDefault();

            await unitDao.DeleteUnitAsync(unit);
        }
        
        [TestMethod]
        public async Task Update() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            Unit unit = await unitDao.FindByIdAsync(1);

            string originalLongName = unit.LongName;
            unit.LongName = "New long name";
            bool update1 = await unitDao.UpdateUnitAsync(unit);
            Assert.IsTrue(update1);

            unit = await unitDao.FindByIdAsync(1);
            Assert.AreEqual(unit.LongName, "New long name");

            unit.LongName = originalLongName;
            bool update2 = await unitDao.UpdateUnitAsync(unit);
            Assert.IsTrue(update2);

            unit = await unitDao.FindByIdAsync(1);
            Assert.AreEqual(unit.LongName, originalLongName);
        }
        
        [TestMethod]
        public async Task Delete() {
            IUnitDao unitDao = new AdoUnitDao(DefaultConnectionFactory.FromConfiguration(configName));

            Unit unit = new Unit("short_test", "long test name");

            bool inserted = await unitDao.AddUnitAsync(unit);
            Assert.IsTrue(inserted);

            unit = (await unitDao.FindByShortNameAsync("short_test")).FirstOrDefault();
            bool deleted = await unitDao.DeleteUnitAsync(unit);
            Assert.IsTrue(deleted);

            unit = (await unitDao.FindByShortNameAsync("short_test")).FirstOrDefault();
            Assert.IsNull(unit);
        }
    }
}
