// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NakedObjects.Reflector.DotNet")]
[assembly: InternalsVisibleTo("NakedObjects.Attributes.Test")]
[assembly: InternalsVisibleTo("NakedObjects.Helpers.Test")]

namespace NakedObjects.UtilInternal {
    internal interface IInternalAccess {
        PropertyInfo[] GetKeys(Type type);
        object FindByKeys(Type type, object[] keys);
    }
}