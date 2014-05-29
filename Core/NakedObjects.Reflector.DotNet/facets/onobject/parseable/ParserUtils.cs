// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    public static class ParserUtils {
        public static Type ParserOrNull<T>(Type candidateType, string classCandidateName) {
            Type type = candidateType != null ? TypeUtils.ImplementingTypeOrNull(candidateType.FullName, typeof (IParser<T>)) : null;
            return type ?? TypeUtils.ImplementingTypeOrNull(classCandidateName, typeof (IParser<T>));
        }
    }
}