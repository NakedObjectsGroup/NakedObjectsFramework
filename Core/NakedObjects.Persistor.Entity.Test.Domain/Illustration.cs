using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Illustration
    {
    
        #region Primitive Properties
        #region IllustrationID (Int32)
    [MemberOrder(100)]
        public virtual int  IllustrationID {get; set;}

        #endregion

        #region Diagram (String)
    [MemberOrder(110), Optionally]
        public virtual string  Diagram {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region ProductModelIllustrations (Collection of ProductModelIllustration)
    		
    	    private ICollection<ProductModelIllustration> _productModelIllustrations = new List<ProductModelIllustration>();
    		
    		[MemberOrder(130), Disabled]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustrations
        {
            get
            {
                return _productModelIllustrations;
            }
    		set
    		{
    		    _productModelIllustrations = value;
    		}
        }

        #endregion


        #endregion

    }
}
