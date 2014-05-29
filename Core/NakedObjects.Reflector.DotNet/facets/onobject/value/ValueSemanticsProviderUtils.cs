// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public static class ValueSemanticsProviderUtils {
        public static Type ValueSemanticsProviderOrNull<T>(Type candidateClass, string classCandidateName) {
            Type type = candidateClass != null ? TypeUtils.ImplementingTypeOrNull(candidateClass.FullName, typeof (IValueSemanticsProvider<T>)) : null;
            return type ?? TypeUtils.ImplementingTypeOrNull(classCandidateName, typeof (IValueSemanticsProvider<T>));
        }
    }
}