// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Base interface for fields, collections, actions and action parameters.
    /// </summary>
    public interface INakedObjectFeature : IFacetHolder, INamedAndDescribed {
        /// <summary>
        ///     Returns the specifications for the feature.
        /// </summary>
        /// <para>
        ///     Will be non-<c>null</c> value for everything <i>except</i> an  action.
        /// </para>
        INakedObjectSpecification Specification { get; }

        bool IsNullable { get; }
        IConsent IsUsable(ISession session, INakedObject target, INakedObjectPersistor persistor);
    }
}