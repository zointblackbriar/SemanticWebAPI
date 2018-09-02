using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemanticAPI.Entities;


namespace SemanticAPI.Helpers
{
    public class AppException : Exception
    {
        //Pure Inheritance
        public AppException() : base() { }

        //@Param message for Exception
        public AppException(string message) : base(message) { }
        //@Param String and Args for Exception
        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
