// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using Expenses.Currencies;
using Expenses.ExpenseEmployees;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    public class ViewModelTestClass : IViewModel {
        public IDomainObjectContainer Container { protected get; set; }

        [Hidden]
        public Employee Employee { get; set; }

        public string UserName {
            get { return Employee.UserName; }
            set { Employee.UserName = value; }
        }

        public string[] DeriveKeys() {
            return new[] {Employee.Name};
        }

        public void PopulateUsingKeys(string[] instanceId) {
            string name = instanceId.First();
            Employee = Container.Instances<Employee>().Single(e => e.Name == name);
        }

        public void ChangeCurrency(Currency newCurrency) {
            Employee.Currency = newCurrency;
        }
    }
}