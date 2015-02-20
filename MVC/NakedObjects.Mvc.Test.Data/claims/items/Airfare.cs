// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public class Airfare : Journey {
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
        }
    }
} //end of root namespace