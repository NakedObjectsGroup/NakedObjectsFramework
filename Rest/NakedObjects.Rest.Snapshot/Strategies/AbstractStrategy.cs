// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    public abstract class AbstractStrategy {
        protected AbstractStrategy(IOidStrategy oidStrategy, RestControlFlags flags) {
            OidStrategy = oidStrategy;
            Flags = flags;
        }

        protected IOidStrategy OidStrategy { get; }
        protected RestControlFlags Flags { get; }

        public MapRepresentation GetExtensions() => GetExtensionsForSimple();

        protected IDictionary<string, object> GetTableViewCustomExtensions((bool, string[])? tableViewData) {
            if (tableViewData == null) {
                return null;
            }

            var (title, columns) = tableViewData.Value;

            return new Dictionary<string, object> {
                [JsonPropertyNames.CustomTableViewTitle] = title,
                [JsonPropertyNames.CustomTableViewColumns] = columns
            };
        }

        protected abstract MapRepresentation GetExtensionsForSimple();
    }
}