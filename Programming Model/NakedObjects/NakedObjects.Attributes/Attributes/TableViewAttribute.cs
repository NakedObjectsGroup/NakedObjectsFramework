// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework;

namespace NakedObjects {
    /// <summary>
    ///     Indicate whether  the annotated object should have a title column when displayed in a table.
    ///     Also provide a list of the properties of the object to be shown as columns when the object is displayed in a table.
    ///     The columns will be displayed in the same order as the list of properties.
    /// </summary>
    public class TableViewAttribute : AbstractTableViewAttribute
    {
        public TableViewAttribute(bool title, params string[] columns) : base(title, columns) { }

    }
}