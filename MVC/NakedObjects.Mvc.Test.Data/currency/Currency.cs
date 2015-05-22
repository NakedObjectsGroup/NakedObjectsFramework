// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;
using NakedObjects.Value;

namespace Expenses.Currencies {
    [Bounded, Immutable(WhenTo.OncePersisted)]
    public class Currency {
        private byte[] currencyFile = new byte[] {};
        private byte[] currencyImage = new byte[] {};

        [Hidden(WhenTo.Always), Key]
        public int Id { get; set; }

        [NotMapped]
        public virtual Image CurrencyImage {
            get { return new Image(currencyImage, "TestImage.jpg", @"image\jpeg"); }
            set { currencyImage = value.GetResourceAsByteArray(); }
        }

        [NotMapped]
        public virtual FileAttachment CurrencyFile {
            get { return new FileAttachment(currencyFile, "TestFile.pdf", @"application\pdf"); }
            set { currencyFile = value.GetResourceAsByteArray(); }
        }

        public virtual byte[] CurrencyByteArray {
            get { return new byte[0]; }
        }

        #region Currency Code

        public virtual string CurrencyCode { get; set; }

        #endregion

        #region Currency Country

        public virtual string CurrencyCountry { get; set; }

        #endregion

        #region Currency Name

        public virtual string CurrencyName { get; set; }

        #endregion

        #region Title & Icon

        public virtual string Title() {
            return CurrencyCode;
        }

        #endregion

        public void UploadImage(Image image) {}
        public void UploadFile(FileAttachment fileAttachment) {}
        public void UploadByteArray(byte[] bytes) {}
    }
}