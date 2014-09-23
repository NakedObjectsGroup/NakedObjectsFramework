// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
        INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues, ILifecycleManager persistor);

        /// <summary>
        ///     Returns a list of possible autocompletions for this field, which the user can choose from
        /// </summary>
        INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm, ILifecycleManager persistor);

        /// <summary>
        ///     Returns a parameter names and types if the field supports conditional choices
        /// </summary>
        Tuple<string, INakedObjectSpecification>[] GetChoicesParameters();
    }
}