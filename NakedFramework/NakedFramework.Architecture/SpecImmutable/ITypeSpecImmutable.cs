// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Architecture.SpecImmutable;

/// <summary>
///     This is the immutable or 'static' core of the IObjectSpec.  It is created by the reflector during start-up, but can
///     also be
///     serialized/deserialized and hence persisted.  However, it needs to be wrapped as an IObjectSpec at run-time in
///     order to
///     provide various run-time behaviors required of the Spec, which depend upon the run-time framework services.
/// </summary>
public interface ITypeSpecImmutable : ISpecificationBuilder {
    Type Type { get; }
    string FullName { get; }
    string ShortName { get; }
    IMenuImmutable ObjectMenu { get; }
    IReadOnlyList<IActionSpecImmutable> OrderedObjectActions { get; }
    IReadOnlyList<IActionSpecImmutable> OrderedContributedActions { get; }
    IReadOnlyList<IActionSpecImmutable> OrderedCollectionContributedActions { get; }
    IReadOnlyList<IActionSpecImmutable> OrderedFinderActions { get; }
    IReadOnlyList<IAssociationSpecImmutable> OrderedFields { get; }
    IReadOnlyList<Type> Interfaces { get; }
    IReadOnlyList<Type> Subclasses { get; }
    Type Superclass { get; }
    bool IsObject { get; }
    bool IsCollection { get; }
    bool IsQueryable { get; }

    bool IsParseable { get; }

    //Will return true if this is a sub-type of the passed-in spec
    bool IsOfType(ITypeSpecImmutable otherSpecification);
    string[] GetLocallyContributedActionNames(string id);
}

// Copyright (c) Naked Objects Group Ltd.