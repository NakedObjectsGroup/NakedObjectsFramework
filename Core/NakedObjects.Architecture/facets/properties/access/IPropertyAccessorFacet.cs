// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Properties.Modify;

namespace NakedObjects.Architecture.Facets.Properties.Access {
    /// <summary>
    ///     The mechanism by which the value of the property can be accessed
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to invoking the accessor method for a property.
    /// </para>
    /// <seealso cref="IPropertySetterFacet" />
    public interface IPropertyAccessorFacet : IFacet {
        /// <summary>
        ///     Gets the value of this property from this object
        /// </summary>
        object GetProperty(INakedObject nakedObject);
    }
}