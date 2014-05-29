// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public class CarRental : AbstractExpenseItem {
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
                return m_numberOfDays > 0 && m_rentalCompany != null & ! (m_rentalCompany.Equals(""));
            }
        }
    }
} //end of root namespace