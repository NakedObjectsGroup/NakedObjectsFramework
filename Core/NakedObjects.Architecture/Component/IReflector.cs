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
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    /// The Reflector is responsible for parsing the code of the domain model and creating the 
    /// MetamodelManager (consisting of Specifications) from this. The Reflector is only run when the 
    /// application is first started-up, and is not used once the application is running.  If the
    /// application has been provided with a previously-generated-and-persisted MetamodelManager, then
    /// the Reflector is not called at all.
    /// </summary>
    public interface IReflector {
        IClassStrategy ClassStrategy { get; }
        IFacetFactorySet FacetFactorySet { get; }
        ITypeSpecBuilder[] AllObjectSpecImmutables { get; }
        bool IgnoreCase { get; }
        bool ConcurrencyChecking { get; }
        IMetamodel Metamodel { get; }
        ITypeSpecBuilder LoadSpecification(Type type);

        /// <summary>
        /// For when you know the expected subclass of the Spec
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        T LoadSpecification<T>(Type type) where T : ITypeSpecImmutable;

        void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore);
        void Reflect();
    }

    // Copyright (c) Naked Objects Group Ltd.
}