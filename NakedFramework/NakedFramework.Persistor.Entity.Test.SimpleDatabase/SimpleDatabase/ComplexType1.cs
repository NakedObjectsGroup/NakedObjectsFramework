// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NakedObjects;

namespace SimpleDatabase;

[ComplexType]
[Owned]
public class ComplexType1 {
    [Root]
    [NotMapped]
    public object Parent { get; set; }

    #region Primitive Properties

    // ReSharper disable InconsistentNaming
    public string s1 { get; set; }

    public string s2 { get; set; }
    // ReSharper restore InconsistentNaming

    #endregion
}