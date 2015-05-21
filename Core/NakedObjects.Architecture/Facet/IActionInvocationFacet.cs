// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.SpecImmutable;
using System.Reflection;

namespace NakedObjects.Architecture.Facet {
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
        MethodInfo ActionMethod { get; }
        IObjectSpecImmutable ReturnType { get; }
        ITypeSpecImmutable OnType { get; }
        IObjectSpecImmutable ElementType { get; }
        bool IsQueryOnly { get; }
        INakedObjectAdapter Invoke(INakedObjectAdapter target, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager);
        INakedObjectAdapter Invoke(INakedObjectAdapter target, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager);
    }

    // Copyright (c) Naked Objects Group Ltd.
}