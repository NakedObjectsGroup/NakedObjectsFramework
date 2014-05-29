// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;

namespace RestfulObjects.Snapshot.Constants {
    public static class PredefinedTypeExtensions {
        public static string ToRoString(this PredefinedType pdt) {
            return pdt.ToString().Replace('_', '-').ToLower();
        }

        public static string[] PredefinedTypeValues() {
            return Enum.GetValues(typeof (PredefinedType)).Cast<PredefinedType>().Select(pdt => pdt.ToRoString()).ToArray();
        }
    }
}