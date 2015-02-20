// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public class CarRental : AbstractExpenseItem {
            #region Copying

            protected internal override void CopyAnyEmptyFieldsSpecificToSubclassOfAbstractExpenseItem(AbstractExpenseItem otherItem) {
                if (otherItem is CarRental) {
                    var carRental = (CarRental) otherItem;

                    if (string.IsNullOrEmpty(m_rentalCompany)) {
                        ModifyRentalCompany(carRental.RentalCompany);
                    }
                    if (m_numberOfDays == 0) {
                        ModifyNumberOfDays(carRental.NumberOfDays);
                    }
                }
            }

            #endregion

            protected internal override bool MandatorySubClassFieldsComplete() {
                return m_numberOfDays > 0 && m_rentalCompany != null & !(m_rentalCompany.Equals(""));
            }

            #region Rental Company

            private string m_rentalCompany;

            [MemberOrder(Sequence = "2.1")]
            public virtual string RentalCompany {
                get { return m_rentalCompany; }

                set { m_rentalCompany = value; }
            }

            public virtual void ModifyRentalCompany(string newRentalCompany) {
                RentalCompany = newRentalCompany;
                CheckIfComplete();
            }

            public virtual void ClearRentalCompany() {
                RentalCompany = null;
                CheckIfComplete();
            }

            public virtual string DisableRentalCompany() {
                return DisabledIfLocked();
            }

            #endregion

            #region Number of Days

            private int m_numberOfDays;

            [MemberOrder(Sequence = "2.2")]
            public virtual int NumberOfDays {
                get { return m_numberOfDays; }

                set { m_numberOfDays = value; }
            }

            public virtual void ModifyNumberOfDays(int newNumberOfDays) {
                NumberOfDays = newNumberOfDays;
                CheckIfComplete();
            }

            public virtual void ClearNumberOfDays() {
                NumberOfDays = 0;
                CheckIfComplete();
            }

            public virtual string DisableNumberOfDays() {
                return DisabledIfLocked();
            }

            #endregion
        }
    }
} //end of root namespace