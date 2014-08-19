// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     See <see cref="IOneToOneAssociation" />/>
    /// </summary>
    public interface IOneToOneFeature : INakedObjectFeature {
        /// <summary>
        ///     Whether there are any choices provided (eg <c>ChoicesXxx</c> supporting method) for the association
        /// </summary>
        bool IsChoicesEnabled { get; }

        /// <summary>
        ///     Whether there are any autocompletion provided (eg <c>AutoCompleteXxx</c> supporting method) for the association
        /// </summary>
        bool IsAutoCompleteEnabled { get; }

        /// <summary>
        ///     Returns a list of possible references/values for this field, which the user can choose from
        /// </summary>
        INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues, INakedObjectPersistor persistor);

        /// <summary>
        ///     Returns a list of possible autocompletions for this field, which the user can choose from
        /// </summary>
        INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm, INakedObjectPersistor persistor);

        /// <summary>
        ///     Returns a parameter names and types if the field supports conditional choices
        /// </summary>
        Tuple<string, INakedObjectSpecification>[] GetChoicesParameters();
    }
}