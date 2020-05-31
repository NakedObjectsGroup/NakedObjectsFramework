// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("ErrorLog")]
    public class ErrorLog {
        public int ErrorLogID { get; set; }

        public DateTime ErrorTime { get; set; }

        [Required]
        [StringLength(128)]
        public string UserName { get; set; }

        public int ErrorNumber { get; set; }

        public int? ErrorSeverity { get; set; }

        public int? ErrorState { get; set; }

        [StringLength(126)]
        public string ErrorProcedure { get; set; }

        public int? ErrorLine { get; set; }

        [Required]
        [StringLength(4000)]
        public string ErrorMessage { get; set; }
    }
}