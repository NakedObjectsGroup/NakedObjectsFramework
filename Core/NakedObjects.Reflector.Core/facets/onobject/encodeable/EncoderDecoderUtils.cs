// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    public static class EncoderDecoderUtils {
        public static Type EncoderDecoderOrNull<T>(Type candidateClass, string classCandidateName) {
            Type type = candidateClass != null ? TypeUtils.ImplementingTypeOrNull(candidateClass.FullName, typeof (IEncoderDecoder<T>)) : null;
            return type ?? TypeUtils.ImplementingTypeOrNull(classCandidateName, typeof (IEncoderDecoder<T>));
        }
    }
}