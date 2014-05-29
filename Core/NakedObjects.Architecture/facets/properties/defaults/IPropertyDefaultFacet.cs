// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Callbacks;

namespace NakedObjects.Architecture.Facets.Properties.Defaults {
    /// <summary>
    ///     Provides a default value for a property of a newly created object.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <c>DefaultXxx</c> supporting method for the property  <c>Xxx</c>.
    /// </para>
    /// <para>
    ///     An alternative mechanism may be to specify the value in the created callback.
    /// </para>
    /// <seealso cref="ICreatedCallbackFacet" />
    public interface IPropertyDefaultFacet : IFacet {
        /// <summary>
        ///     The default value for this property in a newly created object
        /// </summary>
        object GetDefault(INakedObject inObject);
    }
}