// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Framework;

namespace NakedFramework.Architecture.Component;

/// <summary>
///     Service that will inject an implementation of IDomainObjectContainer and/or INakedFramework into
///     domain objects and services when they are instantiated.
/// </summary>
public interface IDomainObjectInjector {
    INakedFramework Framework { set; }

    /// <summary>
    ///     Among other things, will inject all services into the object
    /// </summary>
    void InjectInto(object obj);

    void InjectIntoInline(object root, object inlineObject);

    void InjectParentIntoChild(object parent, object child);
}

// Copyright (c) Naked Objects Group Ltd.