using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace AdventureWorksModel
{

    //Temporary replacement for container, using static functions
    public static class Container
    {
        public static IPrincipal Principal => throw new NotImplementedException();


        public static void DisposeInstance(object persistentObject)
        {
            throw new NotImplementedException();
        }

        public static bool IsPersistent(object obj)
        {
            throw new NotImplementedException();
        }

        public static ITitleBuilder NewTitleBuilder()
        {
            throw new NotImplementedException();
        }


        public static void Persist<T>(ref T transientObject)
        {
            throw new NotImplementedException();
        }

        public static void WarnUser(string message)
        {
            throw new NotImplementedException();
        }
    }
}
