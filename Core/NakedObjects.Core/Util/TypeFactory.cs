// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;

namespace NakedObjects.Core.Util {
    public static class TypeFactory {
        public static Type GetNullableTypeFromLoadedAssembly(string className) {
            Type type = GetTypeFromLoadedAssembly(className);
            return typeof (Nullable<>).MakeGenericType(new[] {type});
        }

        public static Type GetTypeFromLoadedAssembly(string className) {
            if (className.EndsWith("?")) {
                return GetNullableTypeFromLoadedAssembly(className.Remove(className.Length - 1));
            }

            try {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    Type type = assembly.GetType(className);
                    if (type != null) {
                        return type;
                    }
                }
            }
            catch (Exception e) {
                throw new InitialisationException(string.Format(Resources.NakedObjects.ErrorFindingClass, className), e);
            }

            throw new InitialisationException(string.Format(Resources.NakedObjects.ErrorFindingClass, className));
        }
    }
}