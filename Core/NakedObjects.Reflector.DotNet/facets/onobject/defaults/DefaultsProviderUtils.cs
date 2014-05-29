// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public static class DefaultsProviderUtils {
        public static Type DefaultsProviderOrNull<T>(Type candidateClass, string classCandidateName) {
            Type type = candidateClass != null ? TypeUtils.ImplementingTypeOrNull(candidateClass.FullName, typeof (IDefaultsProvider<T>)) : null;
            return type ?? TypeUtils.ImplementingTypeOrNull(classCandidateName, typeof (IDefaultsProvider<T>));
        }
    }
}