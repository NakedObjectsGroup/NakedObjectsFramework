// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    ///     Anything in the metamodel (which also includes peers in the reflector) that has a name and description.
    /// </summary>
    public interface INamedAndDescribed {
        /// <summary>
        ///     Returns a description of how the member is used - this complements the help text.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Return the name for this member - the field or action. This is based on the name of this member.
        /// </summary>
        /// <seealso cref="IMemberSpec.Id" />
        string Name { get; }
    }
}