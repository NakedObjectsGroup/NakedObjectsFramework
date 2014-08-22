// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Provides reflective access to a field on a domain object that is used to reference another domain object
    /// </summary>
    public interface IOneToOneAssociation : INakedObjectAssociation, IOneToOneFeature {
        /// <summary>
        ///     Initialise this field in the specified object with the specified reference - this call should only
        ///     affect the specified object, and not any related objects. It should also not be distributed. This is
        ///     strictly for re-initialising the object and not specifying an association, which is only done once.
        /// </summary>
        void InitAssociation(INakedObject inObject, INakedObject associate);

        /// <summary>
        ///     Determines if the specified reference is valid for setting this field in the specified object
        /// </summary>
        IConsent IsAssociationValid(INakedObject inObject, INakedObject associate, ISession session);

        /// <summary>
        ///     Set up the association represented by this field in the specified object with the specified reference -
        ///     this call sets up the logical state of the object and might affect other objects that share this
        ///     association (such as back-links or bidirectional association). To initialise a recreated object to this
        ///     logical state the <see cref="InitAssociation" /> method should be used on each of the objects.
        /// </summary>
        void SetAssociation(INakedObject inObject, INakedObject associate, INakedObjectPersistor persistor);
    }

    // Copyright (c) Naked Objects Group Ltd.
}