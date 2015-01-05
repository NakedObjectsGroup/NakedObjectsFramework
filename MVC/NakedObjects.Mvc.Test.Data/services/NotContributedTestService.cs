using MvcTestApp.Tests.Helpers;
using NakedObjects;

namespace Expenses.Services {
    public class NotContributedTestService {

        public void NeverContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) { }

        public void NotContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) { }

        public void AlwaysContributed(
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass1-NotContributedTestService:")]NotContributedTestClass1 class1,
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass2-NotContributedTestService:")]NotContributedTestClass2 class2) { }

        public void ContributedToClass1(
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass1-NotContributedTestService:")]NotContributedTestClass1 class1,
            NotContributedTestClass2 class2) { }

        public void ContributedToClass2(
            NotContributedTestClass1 class1,
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass2-NotContributedTestService:")]NotContributedTestClass2 class2) { }
    }
}
