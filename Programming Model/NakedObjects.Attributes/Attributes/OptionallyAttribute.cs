// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Used to indicate that either a property, or an action parameter, is optional.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         By default, the system assumes that all properties of an object are required, and therefore will
    ///         not let the user save a new object unless a value has been specified for each property. Similarly,
    ///         by default, the system assumes that all parameters in an action are required and will not let the
    ///         user execute that action unless values have been specified for each parameter.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class OptionallyAttribute : Attribute {}
}