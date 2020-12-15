// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Menu;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class MenuFacetViaMethod : MenuFacetAbstract {
        private readonly MethodInfo method;

        public MenuFacetViaMethod(MethodInfo method, ISpecification holder)
            : base(holder) =>
            this.method = method;

        //Creates a menu based on the definition in the object's Menu method
        public override void CreateMenu(IMetamodelBuilder metamodel) {
            var menu = new MenuImpl(metamodel, method.DeclaringType, false, GetMenuName(Spec));
            InvokeUtils.InvokeStatic(method, new object[] {menu});
            Menu = menu;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}