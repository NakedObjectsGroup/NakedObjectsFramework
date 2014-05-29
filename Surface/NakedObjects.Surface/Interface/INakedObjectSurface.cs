// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Surface.Interface;

namespace NakedObjects.Surface {
    public interface INakedObjectSurface : IScalarPropertyHolder, ISurfaceHolder {      
        INakedObjectSpecificationSurface Specification { get; }
        INakedObjectSpecificationSurface ElementSpecification { get; }
        object Object { get; }
        IOidSurface Oid { get; }     
        IVersionSurface Version { get; }      
        IEnumerable<INakedObjectSurface> ToEnumerable();
        PropertyInfo[] GetKeys();
        INakedObjectSurface Page(int page, int size);
        int Count();
        AttachmentContext GetAttachment();
    }
}