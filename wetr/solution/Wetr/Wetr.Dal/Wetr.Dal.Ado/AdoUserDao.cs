using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoUserDao : IUserDao {
        private static readonly RowMapper<User> UserMapper =
            row => new User {
                Id = (int)row["id"],
                Username = (string)row["username"],
                Password = (string)row["password"],
                Email = (string)row["email"],
                FirstName = (string)row["first_name"],
                LastName = (string)row["last_name"],
                CommunityId = (int)row["community_id"],
                DateOfBirth = (DateTime)row["date_of_birth"]
            };

        private readonly AdoTemplate _template;

        public AdoUserDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<User> FindByIdAsync(int id) {
            return (await _template.QueryAsync("SELECT * FROM user WHERE id = @id",
                new[] { new QueryParameter("@id", id) }, UserMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<User>> FindAllAsync() {
            return await _template.QueryAsync("SELECT * FROM user", UserMapper);
        }

        public async Task<User> FindByUsernameAsync(string username) {
            return (await _template.QueryAsync("SELECT * FROM user WHERE username = @username",
                new[] { new QueryParameter("@username", username) }, UserMapper)).FirstOrDefault();
        }

        public async Task<User> FindByEmailAsync(string email) {
            return (await _template.QueryAsync("SELECT * FROM user WHERE email = @email",
                new[] { new QueryParameter("@email", email) }, UserMapper)).FirstOrDefault();
        }

        public async Task<bool> CheckPasswordAsync(string username, string password) {
            User user = await FindByUsernameAsync(username);


            //UI blocking tests
            //Thread.Sleep(5000);

            if (user != null) {
                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }

            return false;
        }

        public bool VerifyPassword(string password, string hash) {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Updates an existing user. NO hashing on password of user object.
        /// </summary>
        /// <param name="user">DO NOT USE FOR PASSWORD UPDATE</param>
        /// <returns>Returns true if the update was successful</returns>
        public async Task<bool> UpdateAllAsync(User user) {
            return await _template.ExecuteAsync(
                       "UPDATE user SET username = @username, password = @password, email = @email, first_name = @firstname, last_name = @lastname, date_of_birth = @dob, community_id = @community_id WHERE id = @id",
                       new[] {
                           new QueryParameter("@username", user.Username),
                           new QueryParameter("@password", user.Password),
                           new QueryParameter("@email", user.Email),
                           new QueryParameter("@firstname", user.FirstName),
                           new QueryParameter("@lastname", user.LastName),
                           new QueryParameter("@dob", user.DateOfBirth),
                           new QueryParameter("@community_id", user.CommunityId),
                           new QueryParameter("@id", user.Id)
                       }) == 1;
        }

        /// <summary>
        /// Update password of user. Automatically hashing on password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password">Automatically hashed!</param>
        /// <returns>Returns true is update on password was successful.</returns>
        public async Task<bool> UpdatePasswordAsync(string username, string password) {
            return await _template.ExecuteAsync(
                       "UPDATE user SET password = @password WHERE username = @username",
                       new[] {
                           new QueryParameter("@username", username),
                           new QueryParameter("@password", BCrypt.Net.BCrypt.HashPassword(password))
                       }) == 1;
        }

        /// <summary>
        /// Adds a new user. Automatically hashing on password of user object.
        /// </summary>
        /// <param name="user">Password should not be hashed.</param>
        /// <returns>Returns true if the insert was successful</returns>
        public async Task<bool> AddUserAsync(User user) {
            return await _template.ExecuteAsync(
                       "INSERT INTO user (username, password, email, first_name, last_name, date_of_birth, community_id) " +
                       "VALUES (@username, @password, @email, @firstname, @lastname, @dob, @community_id)",
                       new[] {
                           new QueryParameter("@username", user.Username),
                           new QueryParameter("@password", BCrypt.Net.BCrypt.HashPassword(user.Password)),
                           new QueryParameter("@email", user.Email),
                           new QueryParameter("@firstname", user.FirstName),
                           new QueryParameter("@lastname", user.LastName),
                           new QueryParameter("@dob", user.DateOfBirth),
                           new QueryParameter("@community_id", user.CommunityId)
                       }) == 1;
        }

        public async Task<bool> DeleteUserAsync(User user) {
            return await _template.ExecuteAsync(
                       "DELETE FROM user WHERE id = @id",
                       new[] {
                           new QueryParameter("@id", user.Id)
                       }) == 1;
        }
    }
}
