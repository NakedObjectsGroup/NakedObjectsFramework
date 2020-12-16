using NakedObjects;

namespace AdventureWorksModel {
    public class ServiceWithNoVisibleActions
    {

        [Hidden(WhenTo.Always)]
        public void DoSomething() { }
    }
}
