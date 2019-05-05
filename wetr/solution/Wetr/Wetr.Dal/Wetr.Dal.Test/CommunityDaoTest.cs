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
    public class CommunityDaoTest {
        private static readonly string configName = "WetrDBConnection";

        [TestMethod]
        public async Task TestFindAll() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Community> communities;

            communities = await communityDao.FindAllAsync();
            Assert.IsTrue(communities.Any());
        }

        [TestMethod]
        public async Task TestFindById() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));

            Community community1 = await communityDao.FindByIdAsync(1);
            Assert.AreEqual(community1.Id, 1);

            Community community5000 = await communityDao.FindByIdAsync(5000);
            Assert.IsNull(community5000);
        }

        [TestMethod]
        public async Task TestFindByZipCode() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));
            
            IEnumerable<Community> communities;

            communities = await communityDao.FindByZipCodeAsync(4240);
            Assert.IsTrue(communities.Any());

            communities = await communityDao.FindByZipCodeAsync(0);
            Assert.IsFalse(communities.Any());
        }

        [TestMethod]
        public async Task TestFindByName() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));
            
            IEnumerable<Community> communities;

            communities = await communityDao.FindByNameAsync("Freistadt");
            Assert.IsTrue(communities.Any());

            communities = await communityDao.FindByNameAsync("aaa");
            Assert.IsFalse(communities.Any());
        }

        [TestMethod]
        public async Task TestFindByDistrictId() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));
            
            IEnumerable<Community> communities;

            communities = await communityDao.FindByDistrictIdAsync(101);
            Assert.IsTrue(communities.Any());

            communities = await communityDao.FindByDistrictIdAsync(0);
            Assert.IsFalse(communities.Any());
        }


        [TestMethod]
        public async Task Add() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));

            Community community = new Community(9999, "Test Community", 101);

            bool inserted = await communityDao.AddCommunityAsync(community);
            Assert.IsTrue(inserted);

            community = (await communityDao.FindByNameAsync("Test Community")).FirstOrDefault();

            await communityDao.DeleteCommunityAsync(community);
        }

        [TestMethod]
        public async Task Update() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));

            Community community = await communityDao.FindByIdAsync(1);

            string originalName = community.Name;
            community.Name = "New name";
            bool update1 = await communityDao.UpdateCommunityAsync(community);
            Assert.IsTrue(update1);

            community = await communityDao.FindByIdAsync(1);
            Assert.AreEqual(community.Name, "New name");

            community.Name = originalName;
            bool update2 = await communityDao.UpdateCommunityAsync(community);
            Assert.IsTrue(update2);

            community = await communityDao.FindByIdAsync(1);
            Assert.AreEqual(community.Name, originalName);
        }

        [TestMethod]
        public async Task Delete() {
            ICommunityDao communityDao = new AdoCommunityDao(DefaultConnectionFactory.FromConfiguration(configName));

            Community community = new Community(9999, "Test Community", 101);

            bool inserted = await communityDao.AddCommunityAsync(community);
            Assert.IsTrue(inserted);

            community = (await communityDao.FindByNameAsync("Test Community")).FirstOrDefault();
            bool deleted = await communityDao.DeleteCommunityAsync(community);
            Assert.IsTrue(deleted);

            community = (await communityDao.FindByNameAsync("Test Community")).FirstOrDefault();
            Assert.IsNull(community);
        }
    }
}
