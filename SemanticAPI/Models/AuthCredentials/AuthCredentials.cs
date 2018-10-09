using System;

namespace SemanticAPI.Models.AuthCredentials
{
    public class AuthCredentials
    {
        public AuthCredentials()
        {
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public bool isValid
        {
            get
            {
                return Username != null && Password != null;
            }
        }
    }
}
