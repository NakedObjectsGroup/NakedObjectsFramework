// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;

namespace NakedFunctions.Reflector.Component {
    public record Context : IContext {
        private readonly IObjectPersistor persistor;
        private readonly IServiceProvider provider;

        public Context(IObjectPersistor persistor, IServiceProvider provider) {
            this.persistor = persistor;
            this.provider = provider;
        }

        public object Action { get; init; }
        public object[] PendingSave { get; init; } = Array.Empty<object>();

        public IQueryable<T> Instances<T>() where T : class => persistor.Instances<T>();

        public T GetService<T>() => provider.GetService<T>();

        public IContext WithPendingSave(params object[] toBeSaved) => this with {PendingSave = PendingSave.Union(toBeSaved).ToArray()};

        public IContext WithAction<T>(Action<T> action) => this with {Action = action};
    }
}