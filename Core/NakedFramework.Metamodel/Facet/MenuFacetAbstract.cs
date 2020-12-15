﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Menu;
using NakedObjects.Resources;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class MenuFacetAbstract : FacetAbstract, IMenuFacet {
        protected MenuFacetAbstract(ISpecification holder)
            : base(typeof(IMenuFacet), holder) =>
            Menu = null;

        protected ITypeSpecImmutable Spec => (ITypeSpecImmutable) Specification;

        protected MenuImpl Menu { get; set; }

        #region IMenuFacet Members

        public IMenuImmutable GetMenu() => Menu;

        public abstract void CreateMenu(IMetamodelBuilder metamodel);

        #endregion

        protected static string GetMenuName(ITypeSpecImmutable spec) => spec is IServiceSpecImmutable ? spec.GetFacet<INamedFacet>().NaturalName : Model.ActionsMenuName;
    }
}