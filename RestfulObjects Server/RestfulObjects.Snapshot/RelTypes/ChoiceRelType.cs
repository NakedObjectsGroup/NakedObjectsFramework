// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class ChoiceRelType : ObjectRelType {
        private readonly INakedObjectMemberSurface member;
        private readonly INakedObjectActionParameterSurface parameter;
        private ChoiceRelType(UriMtHelper helper) : base(RelValues.Choice, helper) {}

        public ChoiceRelType(INakedObjectAssociationSurface property, UriMtHelper helper) : this(helper) {
            member = property;
        }

        public ChoiceRelType(INakedObjectActionParameterSurface parameter, UriMtHelper helper)
            : this(helper) {
            this.parameter = parameter;
        }

        public override string Name {
            get { return base.Name + (parameter == null ? helper.GetRelParametersFor(member) : helper.GetRelParametersFor(parameter)); }
        }
    }
}