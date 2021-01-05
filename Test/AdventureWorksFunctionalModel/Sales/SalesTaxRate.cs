// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{

    //Redirects to the StateProvince object on the Azure server
    public record SalesTaxRate
    {

        public override string ToString() => $"Tax Rate for: {StateProvince}";

    [Hidden]
    public virtual int SalesTaxRateID { get; set; }
    public virtual byte TaxType { get; set; }
    public virtual decimal TaxRate { get; set; }
    public virtual string Name { get; set; }

    [Hidden]
    public virtual int StateProvinceID { get; set; }
    public virtual StateProvince StateProvince { get; set; }

    [MemberOrder(99), ConcurrencyCheck]
    public virtual DateTime ModifiedDate { get; set; }

    public virtual Guid rowguid { get; set; }

    [Hidden, NotMapped]
    public virtual string ServerName
    {
        get
        {
            return "nakedobjectsrodemo.azurewebsites.net";
        }
    }

    [Hidden, NotMapped]
    public virtual string Oid
    {
        get
        {
            return "AdventureWorksModel.StateProvince/" + StateProvinceID;
        }
    }
}

public static class SalesTaxRateFunctions
{
    public static SalesTaxRate Updating(SalesTaxRate a,  DateTime now) => a with { ModifiedDate = now };

    public static SalesTaxRate Updating(SalesTaxRate a,  DateTime now,  Guid guid) => a with { ModifiedDate = now, rowguid = guid };

}
}