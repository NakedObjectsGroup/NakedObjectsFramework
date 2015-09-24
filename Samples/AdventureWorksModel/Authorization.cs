using NakedObjects.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel {
    public class DefaultAuthorizer : ITypeAuthorizer<object> {

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }
    }

    //Used to test that a specific Finder Action can be hidden by authorization.
    public class EmployeeRepositoryAuthorizer : ITypeAuthorizer<EmployeeRepository> {
        public bool IsEditable(IPrincipal principal, EmployeeRepository target, string memberName) {
            return true;
        }
        public bool IsVisible(IPrincipal principal, EmployeeRepository target, string memberName) {
            if (memberName == "FindRecentHires") return false;
            return true;
        }
    }
}
