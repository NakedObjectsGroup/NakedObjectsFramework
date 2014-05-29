// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Properties.Access;

namespace NakedObjects.Architecture.Facets.Properties.Modify {
    /// <summary>
    ///     The mechanism by which the value of the property can be initialised.
    /// </summary>
    /// <para>
    ///     This differs from the <see cref="IPropertySetterFacet" /> in that it is only called when
    ///     object is set up (after persistence) and not every time a property changes; hence
    ///     it will not be made part of a transaction.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to invoking the mutator method for a property.
    /// </para>
    /// <seealso cref="IPropertyAccessorFacet" />
    /// <seealso cref="IPropertySetterFacet" />
    /// <seealso cref="IPropertyClearFacet" />
    public interface IPropertyInitializationFacet : IFacet {
        /// <summary>
        ///     Sets the value of this property
        /// </summary>
        void InitProperty(INakedObject nakedObject, INakedObject nakedValue);
    }
}