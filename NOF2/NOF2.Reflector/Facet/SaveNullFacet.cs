// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class SaveNullFacet : SaveFacetAbstract, ISaveFacet {
    private static SaveNullFacet instance;

    private SaveNullFacet() { }

    public static SaveNullFacet Instance => instance ??= new SaveNullFacet();

    public override string Save(INakedFramework framework, INakedObjectAdapter nakedObject, ILogger logger) {
        var msg = $"Attempt to save an object without an ActionSave: {nakedObject.Spec.FullName}";
        logger.LogWarning(msg);
        return msg;
    }
}