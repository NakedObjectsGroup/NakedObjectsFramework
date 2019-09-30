// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///  Attribute to apply to a domain type to indicate scope of reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NakedObjectsTypeAttribute : Attribute {
        /// <summary>
        /// Sets the ReflectionScope to the default value of 'All'
        /// </summary>
        public NakedObjectsTypeAttribute() {
            ReflectionScope = ReflectOver.All;
        }

        public NakedObjectsTypeAttribute(ReflectOver reflectionScope) {
            ReflectionScope = reflectionScope;
        }

        public ReflectOver ReflectionScope { get; set; }
    }

    public enum ReflectOver {
        /// <summary>
        /// Type and all members (except where marked [NakedObjectsIgnore])
        /// </summary>
        All = 1,

        /// <summary>
        /// Typically used on a system service, where the type must be in the meta-model, but there are no
        /// actions/properties for user display.
        /// </summary>
        TypeOnlyNoMembers = 2,

        /// <summary>
        /// Allows an 'additive' style of coding, where only those members marked [NakedObjectsInclude] are
        /// reflected over
        /// </summary>
        ExplicitlyIncludedMembersOnly = 3,

        /// <summary>
        /// The type and all members to be ignored by Naked Objects (and hence excluded from the meta-model)
        /// </summary>
        None = 4
    }
}