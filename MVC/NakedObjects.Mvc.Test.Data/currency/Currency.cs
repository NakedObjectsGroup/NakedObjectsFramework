// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;
using NakedObjects.Value;

namespace Expenses.Currencies {
    [Bounded, Immutable(WhenTo.OncePersisted)]
    public class Currency {
        #region Title & Icon

        public virtual string Title() {
            return CurrencyCode;
        }

        #endregion

        #region Currency Code

        public virtual string CurrencyCode { get; set; }

        #endregion

        #region Currency Country

        public virtual string CurrencyCountry { get; set; }

        #endregion

        #region Currency Name

        public virtual string CurrencyName { get; set; }

        #endregion

        private Image currencyImage = new Image(new byte[0], "TestImage.jpg", @"image\jpeg"); // just for testing
        public virtual Image CurrencyImage {
            get { return currencyImage; }
            set { currencyImage = value; }
        }

        private FileAttachment currencyFile = new FileAttachment(new byte[0] , "TestFile.pdf", @"application\pdf" ); // just for testing
        public virtual FileAttachment CurrencyFile {
            get { return currencyFile; }
            set { currencyFile = value; }
        }

        public virtual byte[] CurrencyByteArray {
            get { return new byte[0]; }
        }

        public void UploadImage(Image image) {}

        public void UploadFile(FileAttachment fileAttachment) {}

        public void UploadByteArray(byte[] bytes) {}
    }
}