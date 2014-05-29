// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace Expenses {
    namespace ExpenseClaims.Items {
        public class GeneralExpense : AbstractExpenseItem {
            public override string IconName()
            {
                return "GeneralExpense";
            }
            protected internal override void CopyAnyEmptyFieldsSpecificToSubclassOfAbstractExpenseItem(AbstractExpenseItem otherItem) {
                // No extra fields to copy
            }

            protected internal override bool MandatorySubClassFieldsComplete() {
                return true; // No extra fields to check.
            }
        }
    }
} //end of root namespace