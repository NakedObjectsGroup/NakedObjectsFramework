// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Specifies that this parameter
    ///     should not be displayed on the UI, but rather that the framework should provide a value for this parameter when the action is
    ///     invoked. Currently supported types:
    ///     - Integer (framework injects a random integer value in the full range of the integer type
    ///     - GUID (framework injects a new GUID)
    ///     - DateTime (framework injects the current date & time)
    ///     - IPrincipal (framework injects an IPrincipal corresponding to the current user)
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class InjectedAttribute : Attribute { }
}