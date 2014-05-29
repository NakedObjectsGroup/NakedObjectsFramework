// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Spec {
    public interface IActionContainer {
        INakedObjectAction[] GetRelatedServiceActions();

        /// <summary>
        ///     Returns an array of actions of the specified type
        /// </summary>
        INakedObjectAction[] GetObjectActions();
    }
}