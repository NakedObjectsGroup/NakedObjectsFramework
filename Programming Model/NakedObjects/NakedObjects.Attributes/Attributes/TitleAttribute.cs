// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Identifies a property as the title for an object
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When the title for an object is simply the state of a property then this attribute allows that property to
    ///         be marked so that it is used insteat of providing an additional title method.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class TitleAttribute : Attribute { }
}