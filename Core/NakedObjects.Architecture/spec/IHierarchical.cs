// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Spec {
    public interface IHierarchical {
        /// <summary>
        ///     Returns true if the <see cref="Subclasses" /> method will return an array of one or more elements (ie,
        ///     not an empty array).
        /// </summary>
        bool HasSubclasses { get; }

        /// <summary>
        ///     Get the list of specifications for all the interfaces that the class represented by this specification
        ///     implements.
        /// </summary>
        INakedObjectSpecification[] Interfaces { get; }

        /// <summary>
        ///     Get the list of specifications for the subclasses of the class represented by this specification
        /// </summary>
        INakedObjectSpecification[] Subclasses { get; }

        /// <summary>
        ///     Get the specification for this specification's class's superclass
        /// </summary>
        INakedObjectSpecification Superclass { get; }

        /// <summary>
        ///     Add the class for the specified specification as a subclass of this specification's class
        /// </summary>
        //void AddSubclass(INakedObjectSpecification specification);
        /// <summary>
        ///     Determines if this specification represents the same specification, or a subclass, of the specified
        ///     specification.
        /// </summary>
        bool IsOfType(INakedObjectSpecification specification);
    }
}