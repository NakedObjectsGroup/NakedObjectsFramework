// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;

namespace NakedObjects.Architecture.Interactions {
    public interface IInteractionContext {
        /// <summary>
        ///     The type of interaction
        /// </summary>
        /// <para>
        ///     Used by <see cref="IFacet" />s that apply only in certain conditions.  For
        ///     example, some facets for collections will care only when an object is
        ///     being added to the collection, but won't care when an object is being removed from
        ///     the collection.
        /// </para>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        InteractionType InteractionType { get; }

        /// <summary>
        ///     The  user or role <see cref="ISession" /> that is performing this interaction.
        /// </summary>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        ISession Session { get; }

        /// <summary>
        ///     How the interaction was initiated
        /// </summary>
        bool IsProgrammatic { get; }

        /// <summary>
        ///     The target object that this interaction is with.
        /// </summary>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        INakedObjectAdapter Target { get; }

        /// <summary>
        ///     The identifier of the object or member that is being identified with.
        /// </summary>
        /// <para>
        ///     If the <see cref="InteractionType" /> type is <see cref="Interactions.InteractionType.ObjectPersist" />,
        ///     will be the identifier of the <see cref="Target" /> object's specification.
        ///     Otherwise will be the identifier of the member.
        /// </para>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        IIdentifier Id { get; }

        /// <summary>
        ///     The proposed value for a property, or object being added/removed from a collection.
        /// </summary>
        /// <para>
        ///     Will be set if the <see cref="InteractionType" /> type is
        ///     <see
        ///         cref="Interactions.InteractionType.PropertyParamModify" />
        ///     ,
        ///     <see cref="Interactions.InteractionType.CollectionAddTo" /> or
        ///     <see
        ///         cref="Interactions.InteractionType.CollectionRemoveFrom" />
        ///     ;
        ///     <c>null</c> otherwise.  In the case of the collection interactions, may be safely downcast
        ///     to <see cref="INakedObjectAdapter" />
        /// </para>
        INakedObjectAdapter ProposedArgument { get; }

        /// <summary>
        ///     The arguments for a proposed action invocation.
        /// </summary>
        /// <para>
        ///     Will be set if the <see cref="InteractionType" /> type is <see cref="Interactions.InteractionType.ActionInvoke" />;
        ///     <c>null</c> otherwise.
        /// </para>
        INakedObjectAdapter[] ProposedArguments { get; }

        /// <summary>
        ///     Convenience to allow implementors of <see cref="IValidatingInteractionAdvisor" /> etc to determine
        ///     if the interaction's type applies.
        /// </summary>
        bool TypeEquals(InteractionType other);
    }
}