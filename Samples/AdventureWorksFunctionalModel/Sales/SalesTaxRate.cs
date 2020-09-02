// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;

using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorksModel {

    //Redirects to the StateProvince object on the Azure server
    public record SalesTaxRate : IRedirectedObject {
        #region Injected Services
        
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        public override string ToString()
        {
            var t = Container.NewTitleBuilder();
            t.Append("Tax Rate for: ").Append(StateProvince);
            return t.ToString();
        }

        [Hidden]
        public virtual int SalesTaxRateID { get; set; }
        public virtual byte TaxType { get; set; }
        public virtual decimal TaxRate { get; set; }
        public virtual string Name { get; set; }

        [Hidden]
        public virtual int StateProvinceID { get; set; }
        public virtual StateProvince StateProvince { get; set; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; set; }

        [Hidden, NotMapped]
        public virtual string ServerName { get
            {
                return "nakedobjectsrodemo.azurewebsites.net";
            }  set { } }

        [Hidden, NotMapped]
        public virtual string Oid { get {
                return "AdventureWorksModel.StateProvince/"+StateProvinceID;
            } set { } }

        #endregion

        #endregion
    }
}