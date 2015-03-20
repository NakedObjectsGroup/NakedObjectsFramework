// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.RecordedActions;
using MvcTestApp.Tests.Helpers;
using NakedObjects;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Services;

namespace Expenses.Services {
    public static class DemoServicesSet {
        public static object[] ServicesSet() {
            var services = new List<object> {
                new EmployeeRepository(),
                new ClaimRepository(),
                new RecordedActionRepository(),
                new RecordActionService(),
                new DummyMailSender(),
                new SimpleRepositoryCustomHelperTestClass(),
                new SimpleRepositoryDescribedCustomHelperTestClass(),
                new ServiceWithNoVisibleActions() //Should not be rendered
            };

            return services.ToArray();
        }

        public static IMenu[] MainMenus(IMenuFactory factory) {
            var claimMenu = factory.NewMenu<ClaimRepository>(false);
            ClaimRepository.Menu(claimMenu);
            return new IMenu[] {
                factory.NewMenu<EmployeeRepository>(true),
                claimMenu,
                factory.NewMenu<DummyMailSender>(true),
                factory.NewMenu<SimpleRepositoryCustomHelperTestClass>(true),
                factory.NewMenu<SimpleRepositoryDescribedCustomHelperTestClass>(true),
                factory.NewMenu<object>(false, "Empty"),  //Should not be rendered
                factory.NewMenu<ServiceWithNoVisibleActions>(true)
            };
        }
    }

    [DisplayName("Custom Helper Test Classes")]
    public class SimpleRepositoryCustomHelperTestClass : SimpleRepository<CustomHelperTestClass> {
        [MemberOrder(Sequence = "1"), FinderAction()]
        public override CustomHelperTestClass NewInstance() {
            return base.NewInstance();
        }

        [MemberOrder(Sequence = "2"), FinderAction()]
        public override IQueryable<CustomHelperTestClass> AllInstances() {
            return Container.Instances<CustomHelperTestClass>();
        }

        [MemberOrder(Sequence = "3"), FinderAction()]
        public override CustomHelperTestClass GetRandom() {
            return base.Random<CustomHelperTestClass>();
        }

        [MemberOrder(Sequence = "4"), FinderAction()]
        public override CustomHelperTestClass FindByKey(int key) {
            return base.FindByKey(key);
        }
    }

    [DisplayName("Described Custom Helper Test Classes")]
    public class SimpleRepositoryDescribedCustomHelperTestClass : SimpleRepository<DescribedCustomHelperTestClass> {
        [MemberOrder(Sequence = "1"), FinderAction()]
        public override DescribedCustomHelperTestClass NewInstance() {
            return base.NewInstance();
        }

        [MemberOrder(Sequence = "2"), FinderAction()]
        public override IQueryable<DescribedCustomHelperTestClass> AllInstances() {
            return Container.Instances<DescribedCustomHelperTestClass>();
        }

        [MemberOrder(Sequence = "3"), FinderAction()]
        public override DescribedCustomHelperTestClass GetRandom() {
            return base.Random<DescribedCustomHelperTestClass>();
        }

        [MemberOrder(Sequence = "4"), FinderAction()]
        public override DescribedCustomHelperTestClass FindByKey(int key) {
            return base.FindByKey(key);
        }
    }

    public class ServiceWithNoVisibleActions {

        [Hidden]
        public void DoSomething() {}
    }
}