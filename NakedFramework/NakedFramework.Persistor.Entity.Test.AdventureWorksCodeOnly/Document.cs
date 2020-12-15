// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Production.Document")]
    public class Document {
        public int DocumentID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(400)]
        public string FileName { get; set; }

        [Required]
        [StringLength(8)]
        public string FileExtension { get; set; }

        [Required]
        [StringLength(5)]
        public string Revision { get; set; }

        public int ChangeNumber { get; set; }

        public byte Status { get; set; }

        public string DocumentSummary { get; set; }

        [Column("Document")]
        public byte[] Document1 { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductDocument> ProductDocuments { get; set; } = new HashSet<ProductDocument>();
    }
}