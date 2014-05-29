// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Reflection;

namespace NakedObjects.Architecture.Facets {
    public enum MethodType {
        Object,
        Class
    };

    /// <summary>
    ///     This is a bit of a hack, but want to be able to centralize knowledge about rules.
    /// </summary>
    public interface IMethodRemover {
        void RemoveMethod(MethodInfo method);

        void RemoveMethods(IList<MethodInfo> methodList);
    }
}