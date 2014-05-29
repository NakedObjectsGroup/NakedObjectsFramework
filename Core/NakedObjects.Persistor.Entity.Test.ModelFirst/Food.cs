// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;

namespace ModelFirst
{
    public  class Food
    {
        #region Primitive Properties
    
        public virtual int Id { get; set; }
    
        public virtual string Name { get; set; }

        #endregion
    
        #region Navigation Properties
    
    
        public virtual Person Person
        {
            get { return _person; }
            set { _person = value; }
        }
    
        private Person _person;

        #endregion
    }
}
