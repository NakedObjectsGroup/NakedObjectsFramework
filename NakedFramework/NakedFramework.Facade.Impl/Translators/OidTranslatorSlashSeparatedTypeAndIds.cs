// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Adapter;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;

namespace NakedFramework.Facade.Impl.Translators;

public class OidTranslatorSlashSeparatedTypeAndIds : IOidTranslator {
    private readonly INakedFramework framework;
    private readonly IKeyCodeMapper keyCodeMapper;
    private readonly ITypeCodeMapper typeCodeMapper;

    public OidTranslatorSlashSeparatedTypeAndIds(INakedFramework framework, ITypeCodeMapper typeCodeMapper, IKeyCodeMapper keyCodeMapper) {
        this.framework = framework;
        this.typeCodeMapper = typeCodeMapper;
        this.keyCodeMapper = keyCodeMapper;
    }

    private string GetCode(ITypeFacade spec) => GetCode(TypeUtils.GetType(spec.FullName));

    protected (string code, string key) GetCodeAndKeyAsTuple(IObjectFacade nakedObject) {
        var code = GetCode(nakedObject.Specification);
        return (code, GetKeyValues(nakedObject));
    }

    private static string KeyRepresentation(object obj) {
        var key = obj switch {
            DateTime time => time.Ticks,
            Guid => obj.ToString(),
            _ => obj
        };

        return (string)Convert.ChangeType(key, typeof(string));
    }

    protected string GetKeyValues(IObjectFacade nakedObjectForKey) {
        string[] keys;
        var wrappedNakedObject = ((ObjectFacade)nakedObjectForKey).WrappedNakedObject;

        if (wrappedNakedObject.Oid is ViewModelOid vmOid) {
            vmOid.UpdateKeysIfNecessary(wrappedNakedObject, framework);
            keys = vmOid.Keys;
        }
        else {
            var keyPropertyInfo = nakedObjectForKey.GetKeys();
            keys = keyPropertyInfo.Select(pi => KeyRepresentation(pi.GetValue(nakedObjectForKey.Object, null))).ToArray();
        }

        return keyCodeMapper.CodeFromKey(keys, nakedObjectForKey.Object.GetType());
    }

    private string GetCode(Type type) => typeCodeMapper.CodeFromType(type);

    #region IOidTranslator Members

    public IOidTranslation GetOidTranslation(params string[] id) =>
        id.Length switch {
            2 => new OidTranslationSlashSeparatedTypeAndIds(id.First(), id.Last()),
            1 => new OidTranslationSlashSeparatedTypeAndIds(id.First()),
            _ => null
        };

    public IOidTranslation GetOidTranslation(IObjectFacade objectFacade) {
        if (objectFacade.IsViewModel) {
            var vm = ((ObjectFacade)objectFacade).WrappedNakedObject;
            framework.LifecycleManager.PopulateViewModelKeys(vm, framework);
        }

        var (code, key) = GetCodeAndKeyAsTuple(objectFacade);
        return new OidTranslationSlashSeparatedTypeAndIds(code, key);
    }

    #endregion
}