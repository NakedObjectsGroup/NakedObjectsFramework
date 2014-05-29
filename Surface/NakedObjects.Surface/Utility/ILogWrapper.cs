// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface.Utility {
    // this is to hide log4net from all code above the Surface ... 
    // this is to avoid problems with log4net versioning caused by their propensity to change keys  

    public interface ILogWrapper {
        void DebugFormat(string fmt, params object[] parms);
        void InfoFormat(string fmt, params object[] parms);
        void WarnFormat(string fmt, params object[] parms);
        void ErrorFormat(string fmt, params object[] parms);
        void FatalFormat(string fmt, params object[] parms);
    }
}