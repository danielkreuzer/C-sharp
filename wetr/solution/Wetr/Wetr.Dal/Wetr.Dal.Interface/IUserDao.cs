using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IUserDao {
        Task<User> FindByIdAsync(int id);
        Task<IEnumerable<User>> FindAllAsync();
        Task<User> FindByUsernameAsync(string username);
        Task<User> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(string username, string password);
        bool VerifyPassword(string password, string hash);
        Task<bool> UpdateAllAsync(User user);
        Task<bool> UpdatePasswordAsync(string username, string password);
        Task<bool> AddUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
    }
}
