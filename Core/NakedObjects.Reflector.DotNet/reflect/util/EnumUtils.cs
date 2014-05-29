// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;

namespace NakedObjects {
    public static class EnumUtils {
        public static When ToWhen(this WhenTo when) {
            return (When) (int) when;
        }

        public static Architecture.Facets.Where ToWhere(this Where where) {
            return (Architecture.Facets.Where) (int) where;
        }
    }
}