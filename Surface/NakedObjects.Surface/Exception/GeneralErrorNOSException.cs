// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class GeneralErrorNOSException : NakedObjectsSurfaceException {
        public GeneralErrorNOSException(Exception e) : base(e.Message, e) {}

        public override string Message {
            get { return GetInnermostException(this).Message; }
        }

        private static Exception GetInnermostException(Exception e) {
            if (e.InnerException == null) {
                return e;
            }
            return GetInnermostException(e.InnerException);
        }
    }
}