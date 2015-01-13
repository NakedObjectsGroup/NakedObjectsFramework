using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Currency
    {
    
        #region Primitive Properties
        #region CurrencyCode (String)
    [MemberOrder(100), StringLength(3)]
        public virtual string  CurrencyCode {get; set;}

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

        #region CurrencyRates (Collection of CurrencyRate)
    		
    	    private ICollection<CurrencyRate> _currencyRates = new List<CurrencyRate>();
    		
    		[MemberOrder(140), Disabled]
        public virtual ICollection<CurrencyRate> CurrencyRates
        {
            get
            {
                return _currencyRates;
            }
    		set
    		{
    		    _currencyRates = value;
    		}
        }

        #endregion

        #region CurrencyRates1 (Collection of CurrencyRate)
    		
    	    private ICollection<CurrencyRate> _currencyRates1 = new List<CurrencyRate>();
    		
    		[MemberOrder(150), Disabled]
        public virtual ICollection<CurrencyRate> CurrencyRates1
        {
            get
            {
                return _currencyRates1;
            }
    		set
    		{
    		    _currencyRates1 = value;
    		}
        }

        #endregion


        #endregion

    }
}
