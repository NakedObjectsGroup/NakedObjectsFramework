// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using NakedObjects;

namespace Expenses.Fixtures {
    public class ExpenseTypeFixture  {

        public IDomainObjectContainer Container { protected get; set; }

        public static ExpenseType AIRFARE;
        public static ExpenseType CAR_RENTAL;
        public static ExpenseType GENERAL;
        public static ExpenseType HOTEL;
        public static ExpenseType MEAL;
        public static ExpenseType MOBILE_PHONE;
        public static ExpenseType PRIVATE_CAR;
        public static ExpenseType TAXI;

        public  void Install() {
            GENERAL = CreateType(typeof (GeneralExpense), "General Expense");
            AIRFARE = CreateType(typeof (Airfare), "Airfare");
            CAR_RENTAL = CreateType(typeof (CarRental), "Car Rental");
            HOTEL = CreateType(typeof (Hotel), "Hotel");
            MEAL = CreateType(typeof (GeneralExpense), "Meal");
            MOBILE_PHONE = CreateType(typeof (GeneralExpense), "Mobile Phone");
            PRIVATE_CAR = CreateType(typeof (PrivateCarJourney), "Private Car Journey");
            TAXI = CreateType(typeof (Taxi), "Taxi");
        }

        [Hidden]
        public virtual ExpenseType CreateType(Type correspondingClass, string titleString) {
            var type = Container.NewTransientInstance<ExpenseType>();
            type.CorrespondingClassName = correspondingClass.FullName;
            type.TitleString = titleString;
            Container.Persist(ref type);
            return type;
        }
    }
}