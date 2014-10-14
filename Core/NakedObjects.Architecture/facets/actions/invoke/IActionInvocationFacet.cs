// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Invoke {
    /// <summary>
    ///     Represents the mechanism by which the action should be invoked.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the actual action method itself (a <c>public</c> method that
    ///     does not represent a property, a collection or any of the supporting
    ///     methods).
    /// </para>
    public interface IActionInvocationFacet : IFacet {
        IIntrospectableSpecification ReturnType { get; }

        IIntrospectableSpecification OnType { get; }

        INakedObject Invoke(INakedObject target, INakedObject[] parameters, INakedObjectManager manager, ISession session, INakedObjectTransactionManager transactionManager);

        INakedObject Invoke(INakedObject target, INakedObject[] parameters, int resultPage, INakedObjectManager manager, ISession session, INakedObjectTransactionManager transactionManager);

        bool GetIsRemoting(INakedObject target);
    }


    // Copyright (c) Naked Objects Group Ltd.
}