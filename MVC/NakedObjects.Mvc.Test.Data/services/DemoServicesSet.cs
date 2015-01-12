// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.RecordedActions;
using MvcTestApp.Tests.Helpers;
using NakedObjects.Services;
using NakedObjects;
using System.Linq;
using System.Reflection;
using NakedObjects.Util;
using NakedObjects.Resources;
using System.ComponentModel;

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
                new SimpleRepositoryDescribedCustomHelperTestClass()
            };

            return services.ToArray();
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


}