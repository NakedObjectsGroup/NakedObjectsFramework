using NakedFunctions;

namespace AdventureWorksModel {
    public record ServiceWithNoVisibleActions
    {

        [NakedObjectsIgnore]
        public void DoSomething() { }
    }
}
