// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Core.Util;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Model;

public class ListValue : IValue {
    private readonly IValue[] internalValue;

    public ListValue(IValue[] value) => internalValue = value;

    #region IValue Members

    public object GetValue(IFrameworkFacade facade, UriMtHelper helper, IOidStrategy oidStrategy) {
        var items = internalValue.Select(iv => iv.GetValue(facade, helper, oidStrategy)).ToArray();

        if (!items.Any()) {
            return null;
        }

        var types = items.Select(i => GetProxiedType(i.GetType())).ToArray();
        var type = GetCommonBaseType(types, types.First());

        var collType = typeof(List<>).MakeGenericType(type);
        var coll = (IList)Activator.CreateInstance(collType);

        Array.ForEach(items, i => coll.Add(i));
        return coll;
    }

    #endregion

    private static Type GetProxiedType(Type type) => FasterTypeUtils.IsAnyProxy(type) ? type.BaseType : type;

    // end clone 

    private static Type GetCommonBaseType(Type[] types, Type baseType) => types.Any(type => !type.IsAssignableFrom(baseType)) ? GetCommonBaseType(types, baseType.BaseType) : baseType;
}