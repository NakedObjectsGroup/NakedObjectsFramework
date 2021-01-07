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
    ///     Indicates that the member (property, collection or action) to which it is applied should never be
    ///     visible to the user
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class HiddenAttribute : Attribute {
        public HiddenAttribute(WhenTo w) => Value = w;

        //Equivalent to specifying WhenTo.Always
        public HiddenAttribute() : this(WhenTo.Always) { }

        public WhenTo Value { get; }
    }
}