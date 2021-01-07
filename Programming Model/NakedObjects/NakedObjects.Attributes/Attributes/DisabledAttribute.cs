// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework;

namespace NakedObjects {
    /// <summary>
    ///     The annotated member cannot be used in any instance of its class
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When applied to the property it means that the user may not modify
    ///         the value of that property (though it may still be modified programmatically).
    ///         When applied to an action method, it means that the user cannot invoke that method
    ///         <para>
    ///             This attribute can also take a single parameter indicating when it is to be disabled
    ///             <see cref="WhenTo" /> (defaulting to <see cref="WhenTo.Always" />)
    ///         </para>
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class DisabledAttribute : Attribute {
        public DisabledAttribute() => Value = WhenTo.Always;

        public DisabledAttribute(WhenTo w) => Value = w;

        public WhenTo Value { get; }
    }
}