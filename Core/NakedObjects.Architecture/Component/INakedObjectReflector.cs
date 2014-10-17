// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Component {
    //TODO: Rename to IReflector and move into Component folder
    public interface INakedObjectReflector {
        IClassStrategy ClassStrategy { get; }

        IFacetFactorySet FacetFactorySet { get; }

        IObjectSpecImmutable[] AllObjectSpecImmutables { get; }

        IObjectSpecImmutable LoadSpecification(Type type);

        IObjectSpecImmutable LoadSpecification(string name);

        void InstallServiceSpecifications(Type[] types);

        void PopulateContributedActions(Type[] services);

        void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore);
    }

    // Copyright (c) Naked Objects Group Ltd.
}