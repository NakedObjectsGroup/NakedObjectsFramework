// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Properties.Modify {
    /// <summary>
    ///     Mechanism for clearing a property of an object (that is, setting it  to <c>null</c>).
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to
    ///     a method named <c>ClearXxx</c> (for a property <c>Xxx</c>). As a
    ///     fallback the standard model also supports invoking the <c>set_Xxx</c>
    ///     method with <c>null</c>.
    /// </para>
    public interface IPropertyClearFacet : IFacet {
        void ClearProperty(INakedObject nakedObject, INakedObjectPersistor persistor);
    }
}