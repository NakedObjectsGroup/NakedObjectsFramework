// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Value;

namespace RestfulObjects.Test.Data {
    public class WithAttachments {
        [Key, Title]
        public virtual int Id { get; set; }

        public FileAttachment FileAttachment {
            get {
                var emptyBa = new byte[] {};
                return new FileAttachment(emptyBa, "afile", "application/pdf");
            }
        }

        public Image Image {
            get {
                var emptyBa = new byte[] {};
                return new Image(emptyBa, "animage", "image/jpeg");
            }
        }
    }
}