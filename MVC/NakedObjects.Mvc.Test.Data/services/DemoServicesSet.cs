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
                new SimpleRepository<DescribedCustomHelperTestClass>()
            };

            return services.ToArray();
        }
    }

    [DisplayName("Custom Helper Test Classes")]
    public class SimpleRepositoryCustomHelperTestClass : SimpleRepository<CustomHelperTestClass> {


              [MemberOrder(Sequence = "1"), FinderAction("Custom Helper Test Classes")]
        public override CustomHelperTestClass NewInstance() {
            return base.NewInstance();
        }
        [MemberOrder(Sequence = "2"), FinderAction("Custom Helper Test Classes")]
              public override IQueryable<CustomHelperTestClass> AllInstances() {
                  return Container.Instances<CustomHelperTestClass>();
        }

        [MemberOrder(Sequence = "3"), FinderAction("Custom Helper Test Classes")]
        public override CustomHelperTestClass GetRandom() {
            return Random<CustomHelperTestClass>();
        }

        [MemberOrder(Sequence = "4"), FinderAction("Custom Helper Test Classes")]
        public override CustomHelperTestClass FindByKey(int key) {
            PropertyInfo keyProperty = Container.GetSingleKey(typeof(CustomHelperTestClass));
            if (keyProperty.PropertyType != typeof (int)) {
                throw new DomainException(string.Format(ProgrammingModel.NoIntegerKey, typeof(CustomHelperTestClass)));
            }
            var result = Container.FindByKey<CustomHelperTestClass>(key);
            if (result == null) {
                WarnUser(ProgrammingModel.NoMatchSingular);
            }
            return result;
        }
    }
}