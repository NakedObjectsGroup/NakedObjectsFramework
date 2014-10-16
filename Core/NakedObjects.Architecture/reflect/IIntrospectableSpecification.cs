// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    /// <summary>
    ///     Introduced to remove special-case processing for <see cref="INakedObjectSpecification" />s that
    ///     are not introspectable.
    /// </summary>
    public interface IIntrospectableSpecification : ISpecification {
        // TODO expose lots of stuff while refactoring 


        Type Type { get; }
        string FullName { get; }
        string ShortName { get; }
        IOrderSet<INakedObjectActionPeer> ObjectActions { get; }
        IList<Tuple<string, string, IOrderSet<INakedObjectActionPeer>>> ContributedActions { get; }
        IList<Tuple<string, string, IOrderSet<INakedObjectActionPeer>>> RelatedActions { get; }
        IOrderSet<INakedObjectAssociationPeer> Fields { get; set; }
        IIntrospectableSpecification[] Interfaces { get; set; }
        IIntrospectableSpecification[] Subclasses { get; set; }
        bool Service { get; set; }
        INakedObjectValidation[] ValidationMethods { get; set; }
        IIntrospectableSpecification Superclass { get; }
        bool IsObject { get; }
        bool IsCollection { get; }
        bool IsParseable { get; }

        /// <summary>
        ///     Discovers what attributes and behaviour the type specified by this specification. As specification are
        ///     cyclic (specifically a class will reference its subclasses, which in turn reference their superclass)
        ///     they need be created first, and then later work out its internals. This allows for cyclic references to
        ///     the be accommodated as there should always a specification available even though it might not be
        ///     complete.
        /// </summary>
        void Introspect(IFacetDecoratorSet decorator);

        void PopulateAssociatedActions(Type[] services);
        void MarkAsService();
        void AddSubclass(IIntrospectableSpecification subclass);
        bool IsOfType(IIntrospectableSpecification specification);
        string GetIconName(INakedObject forObject);
        string GetTitle(INakedObject nakedObject);
    }


    // Copyright (c) Naked Objects Group Ltd.
}