// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public class Airfare : Journey {
            #region Airline and Flight

            private string m_airlineAndFlight;

            [MemberOrder(Sequence = "2.4"), Named("Airline & Flight No.")]
            public virtual string AirlineAndFlight {
                get { return m_airlineAndFlight; }

                set { m_airlineAndFlight = value; }
            }

            public virtual void ModifyAirlineAndFlight(string newAirline) {
                AirlineAndFlight = newAirline;
                CheckIfComplete();
            }

            public virtual void ClearAirlineAndFlight() {
                AirlineAndFlight = null;
                CheckIfComplete();
            }

            public virtual string DisableAirlineAndFlight() {
                return DisabledIfLocked();
            }

            #endregion

            #region Copying

            protected internal override void CopyAnyEmptyFieldsSpecificToSubclassOfJourney(AbstractExpenseItem otherItem) {
                if (otherItem is Airfare) {
                    var airfare = (Airfare) otherItem;
                    if (string.IsNullOrEmpty(m_airlineAndFlight)) {
                        ModifyAirlineAndFlight(airfare.AirlineAndFlight);
                    }
                }
            }

            #endregion

            protected internal override bool MandatoryJourneySubClassFieldsComplete() {
                return !string.IsNullOrEmpty(m_airlineAndFlight);
            }
        }
    }
} //end of root namespace