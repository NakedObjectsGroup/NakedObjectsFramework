// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class DefaultRelType : ObjectRelType {
        private readonly INakedObjectActionParameterSurface parameter;
        private DefaultRelType(UriMtHelper helper) : base(RelValues.Default, helper) {}

        public DefaultRelType(INakedObjectActionParameterSurface parameter, UriMtHelper helper)
            : this(helper) {
            this.parameter = parameter;
        }

        public override string Name {
            get { return base.Name + helper.GetRelParametersFor(parameter); }
        }
    }
}