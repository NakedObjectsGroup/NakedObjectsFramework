// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    ///     Strategy used to determine facts about classes, such as whether an an obj of a particular class can be
    ///     used as a field. Alternative implementations could, for example, exclude types in a specific namespace.
    /// </summary>
    public interface IClassStrategy {
        /// <summary>
        ///     Return the actual Type for the supplied Type.  For example, if the application uses
        ///     proxies or other means to wrap a Type, then this method should return the
        ///     underlying Type which should be exposed by the introspector.
        /// </summary>
        Type GetType(Type type);

        /// <summary>
        ///     Return true if the class is used by the system, and should therefore not be exposed to the user as a
        ///     field
        /// </summary>
        //bool IsSystemClass(Type type);
        string GetKeyForType(Type type);

        bool IsTypeToBeIntrospected(Type type);
        Type FilterNullableAndProxies(Type type);
        bool IsSystemClass(Type introspectedType);
    }

    // Copyright (c) Naked Objects Group Ltd.
}