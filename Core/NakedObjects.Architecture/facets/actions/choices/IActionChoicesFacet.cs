// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Choices {
    /// <summary>
    ///     Obtain choices for each of the parameters of the action
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>ChoicesXxx</c> support method for an
    ///     action
    /// </para>
    public interface IActionChoicesFacet : IFacet {
        Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes { get; }
        bool IsMultiple { get; }
        object[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues);
    }
}