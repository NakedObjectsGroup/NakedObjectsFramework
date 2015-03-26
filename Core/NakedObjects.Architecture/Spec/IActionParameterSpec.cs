// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Spec {
    public interface IActionParameterSpec : IFeatureSpec {
        /// <summary>
        ///     The parameter type spec
        /// </summary>
        IObjectSpec Spec { get; }

        /// <summary>
        ///     The Owning <see cref="IActionSpec" />
        /// </summary>
        IActionSpec Action { get; }

        /// <summary>
        ///     Returns a flag indicating if it can be left unset when the action can be invoked
        /// </summary>
        bool IsMandatory { get; }
        
        /// <summary>
        ///     Returns the zero-based index to this parameter
        /// </summary>
        int Number { get; }

        /// <summary>
        ///     Returns the identifier of the member, which must not change. This should be all Pascal-case with no
        ///     spaces: so if the member is called 'Return Date' then the a suitable id would be 'ReturnDate'.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Whether the parameter has a bounded set associated or a set of options coded.
        /// </summary>
        bool IsChoicesEnabled { get; }

        /// <summary>
        ///     Whether the parameter has a bounded set associated or a set of options coded.
        /// </summary>
        bool IsMultipleChoicesEnabled { get; }

        /// <summary>
        ///     Whether the parameter has a autocomplete method associated.
        /// </summary>
        bool IsAutoCompleteEnabled { get; }

        /// <summary>
        ///     Whether proposed value for this parameter is valid
        /// </summary>
        IConsent IsValid(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter proposedValue);

        /// <summary>
        ///     Get set of options for the parameter - either coded choices or bounded set
        /// </summary>
        INakedObjectAdapter[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues);

        /// <summary>
        ///     Get set of options for the parameter - either coded choices or bounded set
        /// </summary>
        Tuple<string, IObjectSpec>[] GetChoicesParameters();

        /// <summary>
        ///     Get set of autocompletions for the parameter
        /// </summary>
        INakedObjectAdapter[] GetCompletions(INakedObjectAdapter nakedObjectAdapter, string autoCompleteParm);

        /// <summary>
        ///     GetDefault value for parameter
        /// </summary>
        INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     GetDefault type value for parameter
        /// </summary>
        TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter);
    }
}