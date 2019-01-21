using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticAPI.ExceptionHelper
{
    //Inherited Exception Class
    public class ExceptionHelper : Exception
    {

        public ExceptionHelper() : base() { }
        public ExceptionHelper (string message):  base(message) { }

        public ExceptionHelper(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }


    }

}
