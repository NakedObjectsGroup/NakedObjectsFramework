// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;

namespace NakedObjects.Xat {
    public static class XatUtils {
        public static ITestNaked AsTestNaked(this object parameter) {
            return (parameter is ITestNaked) ? (ITestNaked) parameter : new TestParameterObject(parameter);
        }

        public static ITestNaked[] AsTestNakedArray(this object parameter) {
            return new[] {AsTestNaked(parameter)};
        }

        public static ITestNaked[] AsTestNakedArray(this IEnumerable<object> parameters) {
            // this is because passing null to a 'params' parameter  = null 
            // while passing nothing = object[0] 
            return parameters == null ? new ITestNaked[] {null} : parameters.Select(AsTestNaked).ToArray();
        }
    }
}