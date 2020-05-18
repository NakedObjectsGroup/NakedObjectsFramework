// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("DatabaseLog")]
    public class DatabaseLog {
        public int DatabaseLogID { get; set; }

        public DateTime PostTime { get; set; }

        [Required]
        [StringLength(128)]
        public string DatabaseUser { get; set; }

        [Required]
        [StringLength(128)]
        public string Event { get; set; }

        [StringLength(128)]
        public string Schema { get; set; }

        [StringLength(128)]
        public string Object { get; set; }

        [Required]
        public string TSQL { get; set; }

        [Column(TypeName = "xml")]
        [Required]
        public string XmlEvent { get; set; }
    }
}