using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Data Transfer Objects
//The user DTO is a data transfer object used send selected user data to and from the 
//users api end points

namespace SemanticAPI.Dtos
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
