// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Configuration {
    public interface IReflectorConfiguration {
        /// <summary>
        /// This is expected to contain any domain types that are not directly accessible by navigating the actions on the services. Eg an implementation 
        /// of an interface. Generic collection types can be specified and should be passed in without type parameters.
        /// </summary>
        /// <remarks>
        /// These types will always be introspected and so are implicitly 'whitelisted'
        /// </remarks>
        Type[] TypesToIntrospect { get; }

        Type[] Services { get; }

        /// <summary>
        ///  A whitelist of namespaces of the types that will be introspected. 
        /// </summary>
        /// <remarks>
        /// These match on the start so 'MyDomain' will  match 'MyDomain.SomeTypes'  and MyDomain.OtherTypes'
        /// </remarks>
        string[] SupportedNamespaces { get; }

        /// <summary>
        /// Standard implementation of this contains system value and collection types recognised by the Framework. 
        /// The list is exposed so that types can be added or removed before reflection. Generic collection types should be specified 
        /// without type parameters.
        /// </summary>
        /// <remarks>
        /// These types will always be introspected and so are implicitly 'whitelisted'
        /// </remarks>
        List<Type> SupportedSystemTypes { get; }

        Func<IMenuFactory, IMenu[]> MainMenus { get; }
        bool IgnoreCase { get; }

        /// <summary>
        /// Informs the Reflection Framework on how to Load and Introspect Dependencies
        /// Default will be false which will Reflect the dependencies the standard way
        /// If Set to True, the dependencies will be loaded asynchronously, and also
        /// method sorting will be skipped
        /// </summary>
        bool ParallelReflectionMode { get; }
    }
}