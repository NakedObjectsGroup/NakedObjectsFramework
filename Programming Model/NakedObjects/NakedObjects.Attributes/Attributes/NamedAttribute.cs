// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Used when you want to specify the way something is named on the user interface i.e. when you do
    ///     not want to use the name generated automatically by the system. It can be applied to objects,
    ///     members (properties, collections, and actions) and to parameters within an action method.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Instead of this may use <see cref="System.ComponentModel.DisplayNameAttribute" /> but note that it is not applicable to interfaces
    ///         or parameters.
    ///     </para>
    /// </remarks>
    /// <seealso cref="PluralAttribute" />
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class NamedAttribute : Attribute {
        public NamedAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}