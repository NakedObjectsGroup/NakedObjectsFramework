using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel
{
    public class ServiceWithNoVisibleActions
    {

        [Hidden(WhenTo.Always)]
        public void DoSomething() { }
    }
}
