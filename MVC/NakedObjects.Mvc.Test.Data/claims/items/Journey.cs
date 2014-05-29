// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public abstract class Journey : AbstractExpenseItem {
            #region Description

            [Disabled()]
            public override string Description {
                get { return base.Description; }
                set { base.Description = value; }
            }

            private void CreateDescription() {
                string description = Origin + " - " + Destination;
                if (ReturnJourney) {
                    description += " (return)";
                }
                Description = description;
            }

            #endregion

            #region Origin

            private string m_origin;

            [MemberOrder(Sequence = "2.1")]
            public virtual string Origin {
                get { return m_origin; }

                set { m_origin = value; }
            }

            public virtual void ModifyOrigin(string newOrigin) {
                Origin = newOrigin;
                CheckIfComplete();
                CreateDescription();
            }

            public virtual string DisableOrigin() {
                return DisabledIfLocked();
            }

            #endregion

            #region Destination

            private string m_destination;

            [MemberOrder(Sequence = "2.2")]
            public virtual string Destination {
                get { return m_destination; }

                set { m_destination = value; }
            }

            public virtual void ModifyDestination(string newDestination) {
                Destination = newDestination;
                CheckIfComplete();
                CreateDescription();
            }

            public virtual string DisableDestination() {
                return DisabledIfLocked();
            }

            #endregion

            #region Return journey

            private bool m_returnJourney;

            [MemberOrder(Sequence = "2.3")]
            public virtual bool ReturnJourney {
                get { return m_returnJourney; }

                set { m_returnJourney = value; }
            }

            public virtual void ModifyReturnJourney(ref bool newReturnJourney) {
                ReturnJourney = newReturnJourney;
                CheckIfComplete();
                CreateDescription();
            }

            public virtual string DisableReturnJourney() {
                return DisabledIfLocked();
            }

            #endregion

            #region Copying

            protected internal override void CopyAnyEmptyFieldsSpecificToSubclassOfAbstractExpenseItem(AbstractExpenseItem otherItem) {
                if (otherItem is Journey) {
                    var journey = (Journey) otherItem;
                    if (string.IsNullOrEmpty(m_origin)) {
                        ModifyOrigin(journey.Origin);
                    }
                    if (string.IsNullOrEmpty(m_destination)) {
                        ModifyDestination(journey.Destination);
                    }
                }
                CopyAnyEmptyFieldsSpecificToSubclassOfJourney(otherItem);
            }

            protected internal abstract void CopyAnyEmptyFieldsSpecificToSubclassOfJourney(AbstractExpenseItem otherItem);

            #endregion

            #region Fields complete

            protected internal override bool MandatorySubClassFieldsComplete() {
                return m_origin != null && ! (m_origin.Equals("")) && m_destination != null && ! (m_destination.Equals("")) && MandatoryJourneySubClassFieldsComplete();
            }

            protected internal abstract bool MandatoryJourneySubClassFieldsComplete();

            #endregion
        }
    }
} //end of root namespace