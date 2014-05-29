// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Hide {
    /// <summary>
    ///     Hide a property, collection or action
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     annotating the member with <see cref="DisabledAttribute" />
    /// </para>
    public interface IHiddenFacet : ISingleWhenValueFacet, IHidingInteractionAdvisor {
        /// <summary>
        ///     The reason why the (feature of the) target object is currently hidden, or <c>null</c> if visible
        /// </summary>
        string HiddenReason(INakedObject target);
    }
}