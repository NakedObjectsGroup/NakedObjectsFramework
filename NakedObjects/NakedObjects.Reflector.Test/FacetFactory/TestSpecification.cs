// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Spec;

namespace NakedObjects.Reflector.Test.FacetFactory;

// This is for testing of facets 
internal class TestSpecification : Specification, IObjectSpecBuilder {
    public TestSpecification() : base(null) { }

    public Type Type { get; }
    public string FullName { get; }
    public string ShortName { get; }
    public IMenuImmutable ObjectMenu { get; }
    public IReadOnlyList<IActionSpecImmutable> OrderedObjectActions { get; }
    public IReadOnlyList<IActionSpecImmutable> OrderedContributedActions { get; }
    public IReadOnlyList<IActionSpecImmutable> OrderedCollectionContributedActions { get; }
    public IReadOnlyList<IActionSpecImmutable> OrderedFinderActions { get; }
    public IReadOnlyList<IAssociationSpecImmutable> OrderedFields { get; }
    public IReadOnlyList<ITypeSpecImmutable> Interfaces { get; }
    public IReadOnlyList<ITypeSpecImmutable> Subclasses { get; }
    public ITypeSpecImmutable Superclass { get; }
    public bool IsObject { get; }
    public bool IsCollection { get; }
    public bool IsQueryable { get; }
    public bool IsParseable { get; }
    public bool IsOfType(ITypeSpecImmutable otherSpecification) => throw new NotImplementedException();
    public string[] GetLocallyContributedActionNames(string id) => throw new NotImplementedException();

    public bool IsPlaceHolder { get; }
    public bool IsPendingIntrospection { get; }
    public IList<IAssociationSpecImmutable> UnorderedFields { get; }
    public IList<IActionSpecImmutable> UnorderedObjectActions { get; }
    public IImmutableDictionary<string, ITypeSpecBuilder> Introspect(IFacetDecoratorSet decorator, IIntrospector introspector, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => throw new NotImplementedException();

    public void AddSubclass(ITypeSpecImmutable subclass) {
        throw new NotImplementedException();
    }

    public void AddContributedFunctions(IList<IActionSpecImmutable> result) {
        throw new NotImplementedException();
    }

    public void AddContributedFields(IList<IAssociationSpecImmutable> addedFields) {
        throw new NotImplementedException();
    }

    public void RemoveAction(IActionSpecImmutable action, ILogger logger) {
        throw new NotImplementedException();
    }

    public void CompleteIntegration() {
        throw new NotImplementedException();
    }

    public IList<IActionSpecImmutable> UnorderedContributedActions { get; }

    public void AddContributedActions(IList<IActionSpecImmutable> contributedActions, Type[] services) {
        throw new NotImplementedException();
    }

    public void AddCollectionContributedActions(IList<IActionSpecImmutable> collectionCntributedActions) {
        throw new NotImplementedException();
    }

    public void AddFinderActions(IList<IActionSpecImmutable> finderActions) {
        throw new NotImplementedException();
    }

    public IReadOnlyList<IActionSpecImmutable> GetLocallyContributedActions(string id) => throw new NotImplementedException();
}