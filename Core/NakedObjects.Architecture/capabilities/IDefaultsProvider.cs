// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Capabilities {
    /// <summary>
    ///     Not yet fully supported
    /// </summary>
    public interface IDefaultsProvider<T> {
        T DefaultValue { get; }
    }
}