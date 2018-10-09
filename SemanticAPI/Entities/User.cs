using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        //firstname
        public string FirstName { get; set; }
        //lastname
        public string LastName { get; set; }
        //username
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
