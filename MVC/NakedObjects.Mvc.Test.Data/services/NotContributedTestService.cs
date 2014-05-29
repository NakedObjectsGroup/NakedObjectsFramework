using MvcTestApp.Tests.Helpers;
using NakedObjects;

namespace Expenses.Services {
    public class NotContributedTestService {
        [NotContributedAction]
        public void NeverContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}

        [NotContributedAction(typeof (NotContributedTestClass1), typeof (NotContributedTestClass2))]
        public void NotContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}

        public void AlwaysContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}

        [NotContributedAction(typeof (NotContributedTestClass2))]
        public void ContributedToClass1(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}

        [NotContributedAction(typeof (NotContributedTestClass1))]
        public void ContributedToClass2(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}
    }
}
