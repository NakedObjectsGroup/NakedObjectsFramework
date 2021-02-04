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
    internal abstract record Graph : Context {
        protected Graph(
            Context original) {
            Persistor = original.Persistor;
            Provider = original.Provider;
            Action = original.Action;
            Graphs = original.Graphs;
        }

        public object[] New { get; init; } = Array.Empty<object>();
        public (object proxy, object updated)[] Updated { get; init; } = Array.Empty<(object, object)>();

        public override IContext WithNew(object newObj) => this with {New = New.Append(newObj).ToArray()};

        public override IContext WithUpdated<T>(T proxy, T updated) => this with {Updated = Updated.Append((proxy, updated)).ToArray()};

        internal override Graph[] GetGraphs() => Graphs.Append(this).ToArray();
    }

    internal record NewGraph : Graph {
        public NewGraph(object root, Context context) :base(context)  => Root = root;
        public object Root { get; }
    }

    internal record UpdateGraph : Graph {
        public UpdateGraph((object, object) root, Context context) :base(context) => Root = root;
        public (object, object) Root { get; }
    }

    public record Context : IContext {
        internal IObjectPersistor Persistor { get; init; }
        internal IServiceProvider Provider { get; init; }

        public object Action { get; init; }
        
        internal Graph[] Graphs { get; set; } = Array.Empty<Graph>();

        public IQueryable<T> Instances<T>() where T : class => Persistor.Instances<T>();

        public T GetService<T>() => Provider.GetService<T>();

        public virtual IContext WithNew(object newObj) => new NewGraph(newObj, this);

        public virtual IContext WithUpdated<T>(T original, T updated) => new UpdateGraph((original, updated), this);

        public IContext WithAction<T>(Action<T> action) => this with {Action = action};

        private IContext AddGraph(Graph graph) => graph with {Graphs = Graphs.Append(graph).ToArray()};

        internal virtual Graph[] GetGraphs() => Graphs;
    }
}