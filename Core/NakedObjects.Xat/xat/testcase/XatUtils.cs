// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Xat {
    public static class XatUtils {
        private static ITestNaked AsTestNaked(this object parameter, INakedObjectManager manager) {
            return (parameter is ITestNaked) ? (ITestNaked) parameter : new TestParameterObject(manager, parameter);
        }

        public static ITestNaked[] AsTestNakedArray(this object parameter, INakedObjectManager manager) {
            return new[] {AsTestNaked(parameter, manager)};
        }

        public static ITestNaked[] AsTestNakedArray(this IEnumerable<object> parameters, INakedObjectManager manager) {
            // this is because passing null to a 'params' parameter  = null 
            // while passing nothing = object[0] 
            return parameters == null ? new ITestNaked[] {null} : parameters.Select(p => p.AsTestNaked(manager)).ToArray();
        }
    }
}