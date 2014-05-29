// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace NakedObjects.Web.Mvc.Models {
    public class PropertyViewModel {
        public PropertyViewModel(object contextObject, string propertyName) {
            ContextObject = contextObject;
            PropertyName = propertyName;
        }

        public object ContextObject { get; set; }
        public string PropertyName { get; set; }
    }
}