﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Persist;

namespace NakedFramework.Core.Persist;

public class DetachedObjects : IDetachedObjects {
    public DetachedObjects(object[] toSave, object[] toDelete, (object proxy, object updated)[] toUpdate, Func<IDictionary<object, object>, bool> postSaveFunction) {
        ToSave = toSave;
        ToDelete = toDelete;
        ToUpdate = toUpdate;
        PostSaveFunction = postSaveFunction;
    }

    public List<(object original, object updated)> SavedAndUpdated { get; } = new();
    public Func<IDictionary<object, object>, bool> PostSaveFunction { get; }

    public object[] ToDelete { get; }
    public object[] ToSave { get; }
    public (object proxy, object updated)[] ToUpdate { get; }
}