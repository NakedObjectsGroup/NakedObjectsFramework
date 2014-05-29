// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;
using NakedObjects.Value;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public class PrivateCarJourney : Journey {

            public override string IconName()
            {
                return "PrivateCarJourney";
            }
            #region Total Miles field

            private int m_totalMiles;

            [MemberOrder(Sequence = "2.4")]
            public virtual int TotalMiles {
                get { return m_totalMiles; }

                set { m_totalMiles = value; }
            }

            public virtual void ModifyTotalMiles(int newMiles) {
                TotalMiles = newMiles;
                CheckIfComplete();
                RecalculateAmount();
            }

            public virtual string DisableTotalMiles() {
                return DisabledIfLocked();
            }

            #endregion

            #region MileageRate field

            private double m_mileageRate;

            [MemberOrder(Sequence = "2.5")]
            public virtual double MileageRate {
                get { return m_mileageRate; }

                set { m_mileageRate = value; }
            }

            public virtual void ModifyMileageRate(double newRate) {
                MileageRate = newRate;
                CheckIfComplete();
                RecalculateAmount();
            }

            public virtual string DisableMileageRate() {
                return DisabledIfLocked();
            }

            #endregion

            #region Overrides on Amount calculation

            [Disabled]
#pragma warning disable 612,618
            public override decimal Amount {

                get { return base.Amount; }
                set { base.Amount = value; }
            }

            private void RecalculateAmount() {
                ModifyAmount(Convert.ToDecimal(m_totalMiles*m_mileageRate));
            }
#pragma warning restore 612,618
            #endregion

            #region Copying

            protected internal override void CopyAnyEmptyFieldsSpecificToSubclassOfJourney(AbstractExpenseItem otherItem) {
                if (otherItem is PrivateCarJourney) {
                    var carJourney = (PrivateCarJourney) otherItem;
                    if (m_totalMiles == 0) {
                        ModifyTotalMiles(carJourney.TotalMiles);
                    }
                    if (m_mileageRate == 0) {
                        ModifyMileageRate(carJourney.MileageRate);
                    }
                }
            }

            #endregion

            protected internal override bool MandatoryJourneySubClassFieldsComplete() {
                return m_totalMiles > 0 && m_mileageRate > 0;
            }
        }
    }
} //end of root namespace