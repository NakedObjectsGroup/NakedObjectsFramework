// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework;

namespace NakedObjects.Services; 

/// <summary>
///     An implementation of IObjectFinder that will delegate the string representation of a type to an injected
///     ITypeCodeMapper
///     service, if one exists. (Otherwise it will default to using the fully-qualified class name).
/// </summary>
public class ObjectFinderWithTypeCodeMapper : ObjectFinder {
    #region Injected Services

    public ITypeCodeMapper TypeCodeMapper { set; protected get; }

    #endregion

    #region Convert between Type and string representation (code) for Type

    protected override Type TypeFromCode(string code) => TypeCodeMapper.TypeFromCode(code);

    protected override string CodeFromType(object obj) {
        var type = obj.GetType().GetProxiedType();
        return TypeCodeMapper.CodeFromType(type);
    }

    #endregion
}