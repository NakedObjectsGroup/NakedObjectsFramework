using NakedFramework;
using NakedObjects;

namespace AdventureWorksLegacyModel {
    public class ServiceWithNoVisibleActions
    {

        [Hidden(WhenTo.Always)]
        public void DoSomething() { }
    }
}
