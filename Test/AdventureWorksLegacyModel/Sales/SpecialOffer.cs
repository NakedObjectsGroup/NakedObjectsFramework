// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using Legacy.Types;
using NakedObjects;

namespace AdventureWorksModel
{
    [LegacyType]
    public class SpecialOffer : TitledObject
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting()
        {
            rowguid = Guid.NewGuid();
            ModifiedDate.DateTime = DateTime.Now;
        }

        public virtual void Updating() => ModifiedDate.DateTime = DateTime.Now;
        #endregion
        [NakedObjectsIgnore]
        public virtual int SpecialOfferID { get; set; }

        #region Description
        internal string mappedDescription;
        private TextString myDescription;

        [MemberOrder(10)]
        public virtual TextString Description => myDescription ??= new TextString(mappedDescription, v => mappedDescription = v);
        #endregion

        [MemberOrder(20)]
        [Mask("P")]
        public virtual decimal DiscountPct { get; set; }

        #region Type
        internal string mappedType;
        private TextString myType;

        [MemberOrder(30)]
        public virtual TextString Type => myType ??= new TextString(mappedType, v => mappedType = v);
        #endregion

        #region Category
        internal string mappedCategory;
        private TextString myCategory;

        [MemberOrder(40)]
        public virtual TextString Category => myCategory ??= new TextString(mappedCategory, v => mappedCategory = v);
        #endregion

        #region StartDate
        internal DateTime mappedStartDate;
        private Date myStartDate;

        [MemberOrder(51)]
        public virtual Date StartDate => myStartDate ??= new Date(mappedStartDate, v => mappedStartDate = v);
        #endregion

        #region EndDate (Legacy Property)
        internal DateTime mappedEndDate;
        private Date myEndDate;

        [MemberOrder(52)]
        public virtual Date EndDate => myEndDate ??= new Date(mappedEndDate, v => mappedEndDate = v);
        #endregion

        [DisplayAsProperty, Named("Duration (days)"), MemberOrder(53)]
        public int Duration() => 0; //TODO

        #region MinQty (Legacy Property)
        internal int mappedMinQty;
        private WholeNumber myMinQty;

        [MemberOrder(61)]
        public virtual WholeNumber MinQty => myMinQty ??= new WholeNumber(mappedMinQty, v => mappedMinQty = v);
        #endregion

     
        #region EditMinQty (Legacy Action)
        public void ActionEditMinQty(WholeNumber minQty) => MinQty.Number = minQty.Number;

        public void AboutActionEditMinQty(ActionAbout a, WholeNumber minQty)
        {
            switch (a.TypeCode)
            {
                case AboutTypeCodes.Parameters:
                    a.ParamDefaultValues[0] = MinQty;
                    break;
                case AboutTypeCodes.Usable:
                    a.UnusableReason = ValidateEditMinQty(minQty.Number);
                    a.Usable = string.IsNullOrEmpty(a.UnusableReason);              
                    break;
                default: 
                    break;
            }
        }

        private string ValidateEditMinQty(int minQty) => minQty > 0 ? null : "Min Qty must be > 0";
        #endregion

        #region MaxQty (Legacy Property)
        internal int? mappedMaxQty;
        private WholeNumber myMaxQty;
            
        [MemberOrder(62)]
        public virtual WholeNumber MaxQty => myMaxQty ??= new WholeNumber(mappedMaxQty.GetValueOrDefault(), v => mappedMaxQty = v);
        #endregion

        //TODO
        public virtual string[] ChoicesCategory()
        {
            return new[] { "Reseller", "Customer" };
        }

        public virtual DateTime DefaultStartDate()
        {
            return DateTime.Now;
        }

        public virtual DateTime DefaultEndDate()
        {
            return DateTime.Now.AddDays(90);
        }

        #region Title

        public override string ToString() => Description.Text;

        public Title Title() => new Title(Description);

        #endregion

        #region ModifiedDate and rowguid

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate
        internal DateTime mappedModifiedDate;
        private TimeStamp myModifiedDate;

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual TimeStamp ModifiedDate => myModifiedDate ??= new TimeStamp(mappedModifiedDate, s => mappedModifiedDate = s);
        #endregion

        #endregion
    }
}