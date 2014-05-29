// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Xml.Linq;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Snapshot.Xml.Utility {
    public class Place {
        private readonly XElement element;
        private readonly INakedObject nakedObject;

        public Place(INakedObject nakedObject, XElement element) {
            this.nakedObject = nakedObject;
            this.element = element;
        }

        public XElement XmlElement {
            get { return element; }
        }

        public INakedObject NakedObject {
            get { return nakedObject; }
        }
    }
}