// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects {
    /// <summary>
    ///     Merely defines that object has a single integer key called 'Id'.
    ///     Used by PolymorphicNavigator, for example.
    /// </summary>
    public interface IHasIntegerId {
        int Id { get; }
    }
}