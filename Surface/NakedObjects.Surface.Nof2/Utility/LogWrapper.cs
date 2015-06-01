// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Surface.Utility;
//using LogManager = org.apache.log4j.LogManager;

namespace NakedObjects.Surface.Nof2.Utility {
    public class LogWrapper : ILogWrapper {
        private readonly ILog log = LogManager.GetLogger(typeof (LogWrapper));

        #region ILogWrapper Members

        public void DebugFormat(string fmt, params object[] parms) {
            log.DebugFormat(fmt, parms);
        }

        public void InfoFormat(string fmt, params object[] parms) {
            log.InfoFormat(fmt, parms);
        }

        public void WarnFormat(string fmt, params object[] parms) {
            log.WarnFormat(fmt, parms);
        }

        public void ErrorFormat(string fmt, params object[] parms) {
            log.ErrorFormat(fmt, parms);
        }

        public void FatalFormat(string fmt, params object[] parms) {
            log.FatalFormat(fmt, parms);
        }

        #endregion
    }
}