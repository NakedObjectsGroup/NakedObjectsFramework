using NakedFunctions;

namespace AdventureWorksModel {
    public record ServiceWithNoVisibleActions
    {

        [NakedFunctionsIgnore]
        public void DoSomething() { }
    }
}
