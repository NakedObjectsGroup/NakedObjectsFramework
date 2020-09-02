using NakedFunctions;

namespace AdventureWorksModel {
    public record ServiceWithNoVisibleActions
    {

        [Hidden]
        public void DoSomething() { }
    }
}
