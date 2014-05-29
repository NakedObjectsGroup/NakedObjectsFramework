// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Spec {
    public interface IDefaultProvider {
        /// <summary>
        ///     Default value to be provided for properties or parameters that are not declared as
        ///     <see cref="OptionallyAttribute" /> but where the UI has not (yet) provided a value.
        /// </summary>
        object DefaultValue { get; }
    }
}