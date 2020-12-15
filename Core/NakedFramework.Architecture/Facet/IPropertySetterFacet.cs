// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Architecture.Facet {
    /// <summary>
    ///     The mechanism by which the value of the property can be set
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to invoking the mutator method for a property.
    /// </para>
    /// <seealso cref="IPropertyAccessorFacet" />
    /// <seealso cref="IPropertyInitializationFacet" />
    public interface IPropertySetterFacet : IFacet {
        string PropertyName { get; }

        /// <summary>
        ///     Sets the value of this property
        /// </summary>
        void SetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter nakedValue, ITransactionManager transactionManager, ISession session, ILifecycleManager lifecycleManager);
    }
}