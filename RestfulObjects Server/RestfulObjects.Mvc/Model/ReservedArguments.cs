// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Mvc.Model {
    public class ReservedArguments {
        public virtual bool ValidateOnly { get; set; }
        public virtual string DomainModel { get; set; }
        public virtual int ReservedArgumentsCount { get; set; }
        public virtual bool IsMalformed { get; set; }
        public string SearchTerm { get; set; }

        public virtual bool HasValue {
            get { return false; }
        }
    }
}