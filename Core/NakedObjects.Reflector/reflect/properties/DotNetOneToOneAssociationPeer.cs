// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;

namespace NakedObjects.Reflector.DotNet.Reflect.Properties {
    public class DotNetOneToOneAssociationPeer : DotNetNakedObjectAssociationPeer {
        public DotNetOneToOneAssociationPeer(IMetadata metadata, IIdentifier identifier, Type returnType)
            : base(metadata, identifier, returnType, false) {}

        public override string ToString() {
            return "Reference Association [name=\"" + Identifier + ", Type=" + Specification + " ]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}