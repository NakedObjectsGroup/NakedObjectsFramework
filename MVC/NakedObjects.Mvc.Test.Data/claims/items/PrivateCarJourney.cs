// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public class PrivateCarJourney : Journey {
            public override string IconName() {
                return "PrivateCarJourney";
            }

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
        }
    }
} //end of root namespace