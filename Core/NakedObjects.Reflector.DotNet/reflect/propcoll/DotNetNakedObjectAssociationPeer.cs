// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Reflect.Propcoll {
    public abstract class DotNetNakedObjectAssociationPeer : DotNetNakedObjectMemberPeer, INakedObjectAssociationPeer {
        private readonly bool oneToMany;
        protected readonly INakedObjectReflector reflector;
        protected readonly Type returnType;

        protected DotNetNakedObjectAssociationPeer(INakedObjectReflector reflector,  IIdentifier identifier, Type returnType, bool oneToMany)
            : base(identifier) {
            this.reflector = reflector;
            this.returnType = returnType;
            this.oneToMany = oneToMany;
        }

        #region INakedObjectAssociationPeer Members

        public virtual INakedObjectSpecification Specification {
            get { return returnType == null ? null : reflector.LoadSpecification(returnType); }
        }

        public bool IsOneToMany {
            get { return oneToMany; }
        }

        public bool IsOneToOne {
            get { return !IsOneToMany; }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}