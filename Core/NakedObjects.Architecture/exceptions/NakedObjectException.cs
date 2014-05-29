// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;

namespace NakedObjects.Architecture {
    public abstract class NakedObjectException : Exception {
        protected NakedObjectException() {}

        protected NakedObjectException(string messsage)
            : base(messsage) {}

        protected NakedObjectException(string messsage, Exception cause)
            : base(messsage, cause) {}

        protected NakedObjectException(Exception cause)
            : base(cause == null ? null : cause.ToString(), cause) {}

        public void PrintStackTrace(StreamWriter writer) {
            lock (writer) {
                WriteStackTrace(this, writer);
                if (InnerException != null) {
                    writer.Write("Root cause: ");
                    if (InnerException is NakedObjectException)
                        ((NakedObjectException) InnerException).PrintStackTrace(writer);
                    else
                        WriteStackTrace(InnerException, writer);
                }
            }
        }

        private static void WriteStackTrace(Exception exception, TextWriter stream) {
            stream.Write(exception.StackTrace);
            stream.Flush();
        }
    }
}