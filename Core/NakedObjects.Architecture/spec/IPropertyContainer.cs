// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Spec {
    public interface IPropertyContainer {
        /// <summary>
        ///     Return all the properties that exist in an object of this specification,
        ///     although they need not all be accessible or visible.
        /// </summary>
        INakedObjectAssociation[] Properties { get; }

        /// <summary>
        ///     Get the <see cref="INakedObjectAssociation" /> representing the field with the specified field identifier.
        /// </summary>
        INakedObjectAssociation GetProperty(string id);

        INakedObjectValidation[] ValidateMethods();
    }
}