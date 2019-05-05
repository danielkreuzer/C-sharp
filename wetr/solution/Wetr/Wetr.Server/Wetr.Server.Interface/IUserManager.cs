using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Server.Interface {
    public interface IUserManager {
        Task<bool> CheckLogin(string username, string password);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
        Task<bool> UpdatePassword(string username, string password);
        Task<bool> AddUser(User user);
        Task<bool> DeleteUser(User user);
    }
}
