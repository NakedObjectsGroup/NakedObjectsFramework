// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;

namespace NakedObjects.Core.Util {
    public static class ServiceUtils {
        public static string GetId(object obj) {
            PropertyInfo m = obj.GetType().GetProperty("Id", typeof (string));
            if (m != null) {
                return (string) m.GetValue(obj, null);
            }
            return obj.GetType().Name;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}