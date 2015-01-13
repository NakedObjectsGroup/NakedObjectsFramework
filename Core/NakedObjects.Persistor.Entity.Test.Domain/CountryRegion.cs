using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class CountryRegion
    {
    
        #region Primitive Properties
        #region CountryRegionCode (String)
    [MemberOrder(100), StringLength(3)]
        public virtual string  CountryRegionCode {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region CountryRegionCurrencies (Collection of CountryRegionCurrency)
    		
    	    private ICollection<CountryRegionCurrency> _countryRegionCurrencies = new List<CountryRegionCurrency>();
    		
    		[MemberOrder(130), Disabled]
        public virtual ICollection<CountryRegionCurrency> CountryRegionCurrencies
        {
            get
            {
                return _countryRegionCurrencies;
            }
    		set
    		{
    		    _countryRegionCurrencies = value;
    		}
        }

        #endregion

        #region StateProvinces (Collection of StateProvince)
    		
    	    private ICollection<StateProvince> _stateProvinces = new List<StateProvince>();
    		
    		[MemberOrder(140), Disabled]
        public virtual ICollection<StateProvince> StateProvinces
        {
            get
            {
                return _stateProvinces;
            }
    		set
    		{
    		    _stateProvinces = value;
    		}
        }

        #endregion


        #endregion

    }
}
