// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    ///     Anything in the metamodel (which also includes peers in the reflector) that has a name and description.
    /// </summary>
    public interface INamedAndDescribed {
        /// <summary>
        ///     Return the name for this member - the field or action. This is based on the name of this member.
        /// </summary>
        /// <seealso cref="INakedObjectMember.Id" />
        string GetName(INakedObjectPersistor persistor);

        /// <summary>
        ///     Returns a description of how the member is used - this complements the help text.
        /// </summary>
        /// <seealso cref="INakedObjectMember.Help" />
        string Description { get; }
    }
}