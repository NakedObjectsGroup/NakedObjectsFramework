// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects {
    /// <summary>
    ///     Use for immutable objects where there is a bounded set of instances
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The number of instances is expected to be small enough that all instances can be held in memory.
    ///         The viewer will use this information to render all the instances of this class available to the user
    ///         in a convenient form, such as a drop-down list. Although this is not enforced, Bounded is intended
    ///         for use on sealed (non-overridable) classes. Its behaviour when used on interfaces, or classes with sub-classes
    ///         is not specified).
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class BoundedAttribute : Attribute { }
}