// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.RecordedActions;
using MvcTestApp.Tests.Helpers;
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
                new SimpleRepository<CustomHelperTestClass>(),
                new SimpleRepository<DescribedCustomHelperTestClass>()
            };

            return services.ToArray();
        }
    }
}