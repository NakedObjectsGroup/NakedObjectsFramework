using System;
using System.Linq;
using System.Collections.Generic;
using NakedObjects;
using NakedObjects.Fixtures;

namespace $rootnamespace$
{
    public class $safeitemname$
    {
        #region Injected Services
        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        public IDomainObjectContainer Container{ set; protected get; }

        #endregion

        public void Install()
        {

        }

        //Use the 'fact' shortcut to add a new factory method
    }
}