// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.SpecImmutable;
using System;

namespace NakedFramework.Core.Util; 

public static class ToStringHelpers {
    public static string NameAndHashCode(object forObject) => $"{CreateName(forObject)}";

    private static string CreateName(object forObject) => $"{Name(forObject)}@{HashCode(forObject)}";

    public static string Name(object forObject)
    {
        var name = forObject.GetType().FullName;
        return name[(name.LastIndexOf('.') + 1)..];
    }

    public static string AsHex(int number) => Convert.ToString(number, 16);

    public static string HashCode(object forObject) => AsHex(forObject.GetHashCode());

    public static string SuperClass(ITypeSpecImmutable spec) => spec.Superclass is null ? "object" : spec.Superclass.FullName;
}

// Copyright (c) Naked Objects Group Ltd.