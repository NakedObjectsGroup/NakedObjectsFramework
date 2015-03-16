// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     Whether the value of a property or paramter is outside a specified range
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <see cref="RangeAttribute" /> annotation
    /// </para>
    public interface IRangeFacet : IValidatingInteractionAdvisor, IFacet {
        IConvertible Min { get; }
        IConvertible Max { get; }

        /// <summary>
        ///     Returns true if the facet is a range applied to a Date
        /// </summary>
        bool IsDateRange { get; }

        /// <summary>
        ///     Whether the provided value is out of range
        /// </summary>
        int OutOfRange(INakedObjectAdapter nakedObjectAdapter);
    }
}