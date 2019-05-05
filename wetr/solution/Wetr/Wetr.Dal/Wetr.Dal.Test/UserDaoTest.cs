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
    public class UserDaoTest {
        private string _connectionStringConfigName = "WetrDBConnection";

        [TestMethod]
        public async Task FindAll() {
            IUserDao userDao = 
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            IEnumerable<User> users = await userDao.FindAllAsync();

            Assert.IsTrue(users.Count() > 1);
        }

        [TestMethod]
        public async Task FindById() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            User user = await userDao.FindByIdAsync(1);

            Assert.IsTrue(user.Id == 1);
        }

        [TestMethod]
        public async Task FindByUsername() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            User user = await userDao.FindByUsernameAsync("max.müller");

            Assert.IsTrue(user.Username == "max.müller");
        }

        [TestMethod]
        public async Task FindByEmail() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            User user = await userDao.FindByEmailAsync("kevin.bauer@gmail.com");

            Assert.IsTrue(user.Email == "kevin.bauer@gmail.com");
        }

        [TestMethod]
        public async Task CheckPassword() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            bool verify = await userDao.CheckPasswordAsync("max.müller", "max8");

            Assert.IsTrue(verify);
        }

        [TestMethod]
        public void VerifyPassword() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            bool verify =
                userDao.VerifyPassword("max8", "$2a$11$YsECnv19uBc7n6xYLfpoqeaVT5uyJktW7VJO2sE5PT4mrbapHKkWq");
            bool verify1 =
                userDao.VerifyPassword("maxasdf8", "$2a$11$YsECnv19uBc7n6xYLfpoqeaVT5uyJktW7VJO2sE5PT4mrbapHKkWq");

            Assert.IsTrue(verify);
            Assert.IsFalse(verify1);
        }

        [TestMethod]
        public async Task Update() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            User user = await userDao.FindByIdAsync(1);
            string email = user.Email;

            user.Email = "test@test.at";
            bool update1 = await userDao.UpdateAllAsync(user);
            Assert.IsTrue(update1);
            User user1 = await userDao.FindByIdAsync(1);
            Assert.IsTrue(user1.Email == user.Email);

            user.Email = email;
            bool update2 = await userDao.UpdateAllAsync(user);
            Assert.IsTrue(update2);

            User user2 = await userDao.FindByIdAsync(1);
            Assert.IsTrue(user2.Email == email);

        }

        [TestMethod]
        public async Task UpdatePassword() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            bool change1 = await userDao.UpdatePasswordAsync("max.müller", "TESTPASSWORD");
            Assert.IsTrue(change1);
            bool check1 = await userDao.CheckPasswordAsync("max.müller", "TESTPASSWORD");
            Assert.IsTrue(check1);

            bool change2 = await userDao.UpdatePasswordAsync("max.müller", "max8");
            Assert.IsTrue(change2);
            bool check2 = await userDao.CheckPasswordAsync("max.müller", "max8");
            Assert.IsTrue(check2);
        }

        [TestMethod]
        public async Task Add() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            User user = new User("test", "test", "test@test.at", "test", "test", new DateTime(2010, 8, 18), 1);
            bool insert = await userDao.AddUserAsync(user);
            Assert.IsTrue(insert);

            User user1 = await userDao.FindByUsernameAsync("test");
            Assert.IsTrue(user1 != null && user1.Username == "test");

            bool delete = await userDao.DeleteUserAsync(user1);
            Assert.IsTrue(delete);
        }

        [TestMethod]
        public async Task Delete() {
            IUserDao userDao =
                new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName));

            User user = new User("test1", "test1", "test1@test.at", "test1", "test1", new DateTime(2010, 8, 18), 1);
            bool insert = await userDao.AddUserAsync(user);
            Assert.IsTrue(insert);

            User user1 = await userDao.FindByUsernameAsync("test1");
            Assert.IsTrue(user1 != null && user1.Username == "test1");

            bool delete = await userDao.DeleteUserAsync(user1);
            Assert.IsTrue(delete);
            
            User user2 = await userDao.FindByUsernameAsync("test1");
            Assert.IsTrue(user2 == null);
        }
    }
}
