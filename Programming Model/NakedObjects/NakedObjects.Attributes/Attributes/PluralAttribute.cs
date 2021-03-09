// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Specify the plural form of the object's name
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Where the framework displays a collection of objects it may use the plural form of the object type
    ///         in view. By default the plural name will be created by adding an 's' to the end of the singular name
    ///         (whether that is the class name or another name specified using the Named attribute). Where the single
    ///         name ends in 'y' then the default plural name will end in 'ies' - for example a collection of Country
    ///         objects will be titled Countries. Where these conventions do not work, the programmer may specify
    ///         the plural form of the name using the Plural atttibute
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class PluralAttribute : Attribute {
        public PluralAttribute(string s) => Value = s;

        public string Value { get; }
    }
}