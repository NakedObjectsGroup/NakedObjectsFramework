// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public interface IConsentSurface {
        bool IsAllowed { get; }
        bool IsVetoed { get; }
        string Reason { get; }
        Exception Exception { get; }
    }
}