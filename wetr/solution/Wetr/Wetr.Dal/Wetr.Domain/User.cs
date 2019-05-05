using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class User {

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CommunityId { get; set; }

        public User() {}

        public User(int id, string username, string password, string email, string firstName, string lastName, DateTime dateOfBirth, int communityId) {
            Id = id;
            Username = username;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            CommunityId = communityId;
        }

        public User(string username, string password, string email, string firstName, string lastName, DateTime dateOfBirth, int communityId) {
            Username = username;
            Password = password;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            CommunityId = communityId;
        }
    }
}
