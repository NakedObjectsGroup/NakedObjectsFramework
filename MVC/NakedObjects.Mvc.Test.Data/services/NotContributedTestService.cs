using MvcTestApp.Tests.Helpers;
using NakedObjects;

namespace Expenses.Services {
    public class NotContributedTestService {

        public void NeverContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) { }

        public void NotContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) { }

        public void AlwaysContributed(
            [ContributedAction("Not Contributed Test Service")]NotContributedTestClass1 class1,
            [ContributedAction("Not Contributed Test Service")]NotContributedTestClass2 class2) { }

        public void ContributedToClass1(
            [ContributedAction("Not Contributed Test Service")]NotContributedTestClass1 class1,
            NotContributedTestClass2 class2) { }

        public void ContributedToClass2(
            NotContributedTestClass1 class1,
            [ContributedAction("Not Contributed Test Service")]NotContributedTestClass2 class2) { }
    }
}
