// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using MvcTestApp.Tests.Helpers;
using NakedObjects;

namespace Expenses.Services {
    public class NotContributedTestService {
        public void NeverContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}
        public void NotContributed(NotContributedTestClass1 class1, NotContributedTestClass2 class2) {}

        public void AlwaysContributed(
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass1-NotContributedTestService:")] NotContributedTestClass1 class1,
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass2-NotContributedTestService:")] NotContributedTestClass2 class2) {}

        public void ContributedToClass1(
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass1-NotContributedTestService:")] NotContributedTestClass1 class1,
            NotContributedTestClass2 class2) {}

        public void ContributedToClass2(
            NotContributedTestClass1 class1,
            [ContributedAction(SubMenu = "Not Contributed Test Service", Id = "NotContributedTestClass2-NotContributedTestService:")] NotContributedTestClass2 class2) {}
    }
}