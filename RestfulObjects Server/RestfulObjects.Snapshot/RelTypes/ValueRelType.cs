// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class ValueRelType : ObjectRelType {
        private readonly INakedObjectMemberSurface member;
        public ValueRelType(UriMtHelper helper) : base(RelValues.Value, helper) {}

        public ValueRelType(INakedObjectAssociationSurface property, UriMtHelper helper) : this(helper) {
            member = property;
        }

        public override string Name {
            get { return base.Name + helper.GetRelParametersFor(member); }
        }
    }
}