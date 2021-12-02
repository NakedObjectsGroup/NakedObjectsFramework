// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;

namespace NakedFunctions.Reflector.Component;

public record FunctionalContext : IContext {
    internal IDictionary<object, object> ProxyMap = new Dictionary<object, object>();
    internal IObjectPersistor Persistor { get; init; }
    internal IServiceProvider Provider { get; init; }

    internal object[] New { get; init; } = Array.Empty<object>();
    internal (object proxy, object updated)[] Updated { get; init; } = Array.Empty<(object, object)>();
    internal object[] Deleted { get; init; } = Array.Empty<object>();

    public Func<IContext, IContext> PostSaveFunction { get; init; }

    public IQueryable<T> Instances<T>() where T : class => Persistor.Instances<T>();

    public T GetService<T>() => Provider.GetService<T>();

    public IContext WithNew<T>(T newObj) where T : class => this with { New = New.Append(newObj).ToArray() };

    public IContext WithUpdated<T>(T proxy, T updated) where T : class => this with { Updated = Updated.Append((proxy, updated)).ToArray() };
    public IContext WithDeleted<T>(T deleteObj) where T : class => this with { Deleted = Deleted.Append(deleteObj).ToArray() };

    public IContext WithDeferred(Func<IContext, IContext> function) => this with { PostSaveFunction = function };

    public T Reload<T>(T unsaved) where T : class => ProxyMap.ContainsKey(unsaved) ? (T)ProxyMap[unsaved] : Persistor.ValidateProxy(unsaved);

    public T Resolve<T>(T obj) where T : class => (T)Persistor.Resolve(obj);
}