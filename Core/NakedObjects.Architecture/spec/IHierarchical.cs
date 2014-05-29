// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
        void AddSubclass(INakedObjectSpecification specification);

        /// <summary>
        ///     Determines if this specification represents the same specification, or a subclass, of the specified
        ///     specification.
        /// </summary>
        bool IsOfType(INakedObjectSpecification specification);
    }
}