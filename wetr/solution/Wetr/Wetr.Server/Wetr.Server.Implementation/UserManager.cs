using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Ado;
using Wetr.Dal.Interface;
using Wetr.Domain;
using Wetr.Server.Interface;

namespace Wetr.Server.Implementation {
    
    public class UserManager : IUserManager {
        private static string _connectionStringConfigName = "WetrDBConnection";
        private static IUserDao iUserDao = null;

        private static IUserDao GetIUserDao() {
            return iUserDao ?? (iUserDao =
                       new AdoUserDao(DefaultConnectionFactory.FromConfiguration(_connectionStringConfigName)));
        }

        public UserManager(IUserDao userDao = null) {
            iUserDao = userDao;
        }

        public async Task<bool> CheckLogin(string username, string password) {
            if (!username.Equals("") && !password.Equals("")) {
                IUserDao userDao = GetIUserDao();
                return await userDao.CheckPasswordAsync(username, password);
            }

            return false;
        }

        public async Task<User> GetUserByUsername(string username) {
            if (!username.Equals("")) {
                IUserDao userDao = GetIUserDao();
                return await userDao.FindByUsernameAsync(username);
            }

            return null;
        }

        public async Task<User> GetUserById(int id) {
            try {
                IUserDao userDao = GetIUserDao();
                return await userDao.FindByIdAsync(id);
            }
            catch (Exception) {
                return null;
            }
        }

        public async Task<User> GetUserByEmail(string email) {
            if (!email.Equals("")) {
                IUserDao userDao = GetIUserDao();
                return await userDao.FindByEmailAsync(email);
            }

            return null;
        }

        public async Task<bool> UpdateUser(User user) {
            if (user != null) {
                IUserDao userDao = GetIUserDao();
                return await userDao.UpdateAllAsync(user);
            }

            return false;
        }

        public async Task<bool> UpdatePassword(string username, string password) {
            if (!username.Equals("") && !password.Equals("")) {
                IUserDao userDao = GetIUserDao();
                return await userDao.UpdatePasswordAsync(username, password);
            }

            return false;
        }

        public async Task<bool> AddUser(User user) {
            if (user != null) {
                IUserDao userDao = GetIUserDao();
                return await userDao.AddUserAsync(user);
            }

            return false;
        }

        public async Task<bool> DeleteUser(User user) {
            if (user != null) {
                IUserDao userDao = GetIUserDao();
                return await userDao.DeleteUserAsync(user);
            }

            return false;
        }
    }
}
