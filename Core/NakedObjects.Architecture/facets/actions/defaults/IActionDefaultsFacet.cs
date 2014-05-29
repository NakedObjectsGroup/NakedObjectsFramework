// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets.Actions.Defaults {
    /// <summary>
    ///     Obtain defaults for each of the parameters of the action
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>DefaultsXxx(...)</c> support method for an action
    /// </para>
    public interface IActionDefaultsFacet : IFacet {
        Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject);
    }
}