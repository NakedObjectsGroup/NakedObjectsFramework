using NakedObjects.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel {
    /// <summary>
    /// The purpose of this auhtorizer is merely to demonstrate how
    /// authorization works, and for test purposes.  It just hides a few members
    /// that are not ever intended to be used.
    /// </summary>
    public class DemoAuthorizer : ITypeAuthorizer<object> {

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            if (target.GetType() == typeof(EmployeeRepository) && memberName == "FindRecentHires") return false;
            if (target.GetType() == typeof(OrderContributedActions) && memberName == "CommentAsUsersMiserable") return false;
            return true;
        }
    }
}
