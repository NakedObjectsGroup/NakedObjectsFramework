// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Metamodel.Utils {
    public class NakedObjectValidationMethod {
        private readonly MethodInfo method;

        public NakedObjectValidationMethod(MethodInfo method) {
            this.method = method;
        }

        public string[] ParameterNames {
            get { return method.GetParameters().Select(p => p.Name.ToLower()).ToArray(); }
        }

        public string Execute(INakedObject obj, INakedObject[] parameters) {
            return InvokeUtils.Invoke(method, obj, parameters) as string;
        }
    }
}