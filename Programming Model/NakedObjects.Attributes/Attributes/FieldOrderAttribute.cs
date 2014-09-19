// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Specific the order of properties in one place in the class
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The recommended mechanism for specifying the order in which properties are listed to the user is
    ///         <see
    ///             cref="MemberOrderAttribute" />
    ///         .
    ///         FieldOrder provides an alternative mechanism, in which the order is specified in one place in the
    ///         class, with the added advantage (currently) that you can easily specify groupings (which may be rendered by the
    ///         framework as sub-menus). However, FieldOrder is more 'brittle' to change: if you change the name of an existing
    ///         property you will need to ensure that the corresponding name within the FieldOrder attribute is also changed.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FieldOrderAttribute : Attribute {
        public FieldOrderAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}