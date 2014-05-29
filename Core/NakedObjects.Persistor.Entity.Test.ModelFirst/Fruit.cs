// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;

namespace ModelFirst
{
    public  class Fruit : Food
    {
        #region Primitive Properties
    
        public virtual bool Organic { get; set; }

        #endregion
    
    }
}
