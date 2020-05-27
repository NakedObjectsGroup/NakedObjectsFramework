// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Xat {
    public static class XatUtils {
        private static ITestNaked AsTestNaked(this object parameter, INakedObjectManager manager) => parameter is ITestNaked testNaked ? testNaked : new TestParameterObject(manager, parameter);

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