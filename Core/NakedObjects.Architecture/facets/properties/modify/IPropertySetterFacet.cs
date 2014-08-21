// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Properties.Modify {
    /// <summary>
    ///     The mechanism by which the value of the property can be set
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to invoking the mutator method for a property.
    /// </para>
    /// <seealso cref="IPropertyAccessorFacet" />
    /// <seealso cref="IPropertyClearFacet" />
    /// <seealso cref="IPropertyInitializationFacet" />
    public interface IPropertySetterFacet : IFacet {
        /// <summary>
        ///     Sets the value of this property
        /// </summary>
        void SetProperty(INakedObject nakedObject, INakedObject nakedValue, INakedObjectPersistor persistor);
    }
}