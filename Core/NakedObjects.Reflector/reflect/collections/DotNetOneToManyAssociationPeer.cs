// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;

namespace NakedObjects.Reflector.DotNet.Reflect.Collections {
    public class DotNetOneToManyAssociationPeer : DotNetNakedObjectAssociationPeer {
        public DotNetOneToManyAssociationPeer(IMetadata metadata, IIdentifier name, Type returnType)
            : base(metadata, name, returnType, true) {}

        public Type ElementType { get; set; }

        #region INakedObjectAssociationPeer Members

        /// <summary>
        ///     Return the <see cref="INakedObjectSpecification" /> for the  Type that the collection holds.
        /// </summary>
        public override INakedObjectSpecification Specification {
            get { return Metadata.GetSpecification(ElementType ?? typeof (object)); }
        }

        #endregion

        public override string ToString() {
            return "OneToManyAssociation [name=\"" + Identifier + "\",Type=" + Specification + " ]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}