// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace Expenses {
    namespace ExpenseClaims.Items {
        public class Taxi : Journey {
            protected internal override void CopyAnyEmptyFieldsSpecificToSubclassOfJourney(AbstractExpenseItem otherItem) {
                // No extra fields to copy.
            }

            protected internal override bool MandatoryJourneySubClassFieldsComplete() {
                return true;
                // No extra fields tocheck
            }
        }
    }
} //end of root namespace