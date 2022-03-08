// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class TableViewFacet : FacetAbstract, ITableViewFacet {
    public TableViewFacet(bool title, string[] columns)
        : base(typeof(ITableViewFacet)) {
        Title = title;
        Columns = columns;
    }

    public static ITableViewFacet CreateTableViewFacet(bool title, string[] columns, IIdentifier identifier, ILogger logger) {
        columns ??= Array.Empty<string>();
        var distinctColumns = columns.Distinct().ToArray();

        if (columns.Length != distinctColumns.Length) {
            // we had duplicates - log
            var duplicates = columns.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).Aggregate("", (s, t) => s != "" ? $"{s}, {t}" : t);
            var name = identifier is null ? "Unknown" : identifier.ToString();
            logger.LogWarning($"Table View on {name} had duplicate columns {duplicates}");
            columns = distinctColumns;
        }

        return new TableViewFacet(title, columns);
    }

    #region ITableViewFacet Members

    public bool Title { get; set; }
    public string[] Columns { get; private set; }

    #endregion
}