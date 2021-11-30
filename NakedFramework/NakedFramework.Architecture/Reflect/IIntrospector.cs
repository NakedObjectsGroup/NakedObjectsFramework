// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Architecture.Reflect {
    public interface IIntrospector {
        Type IntrospectedType { get; }

        Type SpecificationType { get; }

        /// <summary>
        ///     As per <see cref="MemberInfo.Name" />
        /// </summary>
        string ClassName { get; }

        IIdentifier Identifier { get; }
        string FullName { get; }
        string ShortName { get; }
        IList<IAssociationSpecImmutable> UnorderedFields { get; }
        IList<IActionSpecImmutable> UnorderedObjectActions { get; }
        ITypeSpecBuilder[] Interfaces { get; set; }
        ITypeSpecBuilder Superclass { get; set; }

        IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);
    }
}