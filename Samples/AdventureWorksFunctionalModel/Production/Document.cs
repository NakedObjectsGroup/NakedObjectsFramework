// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel
{
    public class Document : IHasModifiedDate
    {

        public Document(
            int documentID,
            string title,
            string fileName,
            string fileExtension,
            string revision,
            int changeNumber,
            byte status,
            string documentSummary,
            byte[] document1,
            ICollection<ProductDocument> productDocument,
            DateTime modifiedDate
            )
        {
            DocumentID = documentID;
            Title = title;
            FileName = fileName;
            FileExtension = fileExtension;
            Revision = revision;
            ChangeNumber = changeNumber;
            Status = status;
            DocumentSummary = documentSummary;
            Document1 = document1;
            ProductDocument = productDocument;
            ModifiedDate = modifiedDate;
        }

        public Document() { }

        public virtual int DocumentID { get; set; }
        public virtual string Title { get; set; }
        public virtual string FileName { get; set; }
        public virtual string FileExtension { get; set; }
        public virtual string Revision { get; set; }
        public virtual int ChangeNumber { get; set; }
        public virtual byte Status { get; set; }
        public virtual string DocumentSummary { get; set; }
        public byte[] Document1 { get; set; }

        public ICollection<ProductDocument> ProductDocument { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

    }

    public static class DocumentFunctions
    {
        public static Document Updating(Document d, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(d, now);

        }
    }
}