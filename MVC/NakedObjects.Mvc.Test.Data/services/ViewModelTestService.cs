using System.Linq;
using Expenses.ExpenseEmployees;
using MvcTestApp.Tests.Helpers;
using NakedObjects;

namespace Expenses.Services {
    public class ViewModelTestService {
        public IDomainObjectContainer Container { protected get; set; }

        private ViewModelTestClass GetViewModel() {
            Employee e = Container.Instances<Employee>().First();
            var vm = Container.NewViewModel<ViewModelTestClass>();
            vm.Employee = e;
            return vm;
        }
    }
}