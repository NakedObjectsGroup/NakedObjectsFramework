// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Diagnostics;

namespace NakedObjects.Xat.Performance {
    public class ProfilerSystem {
        protected internal virtual long Memory {
            get { return Process.GetCurrentProcess().WorkingSet64; }
        }

        protected internal virtual long Time {
            get {
                // convert to nS 
                return DateTime.Now.Ticks*100;
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}