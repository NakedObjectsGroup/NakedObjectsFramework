// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///   This is the immutable or 'static' core of the IObjectSpec.  It is created by the reflector during start-up, but can also be
    ///   serialised/deserialised and hence persisted.  However, it needs to be wrapped as an IObjectSpec at run-time in order to 
    ///   provide various run-time behaviours required of the Spec, which depend upon the run-time framework services.
    /// </summary>
    public interface IObjectSpecImmutable : ISpecification {
        // TODO expose lots of stuff while refactoring 


        Type Type { get; }
        string FullName { get; }
        string ShortName { get; }
        IOrderSet<IActionSpecImmutable> ObjectActions { get; }
        IList<Tuple<string, string, IOrderSet<IActionSpecImmutable>>> ContributedActions { get; }
        IList<Tuple<string, string, IOrderSet<IActionSpecImmutable>>> RelatedActions { get; }
        IOrderSet<IAssociationSpecImmutable> Fields { get; set; }
        IObjectSpecImmutable[] Interfaces { get; set; }
        IObjectSpecImmutable[] Subclasses { get; set; }
        bool Service { get; set; }
        INakedObjectValidation[] ValidationMethods { get; set; }
        IObjectSpecImmutable Superclass { get; }
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
        void Introspect(IFacetDecoratorSet decorator, IIntrospector introspector);
        void MarkAsService();
        void AddSubclass(IObjectSpecImmutable subclass);
        bool IsOfType(IObjectSpecImmutable specification);
        string GetIconName(INakedObject forObject);
        string GetTitle(INakedObject nakedObject);
    }


    // Copyright (c) Naked Objects Group Ltd.
}