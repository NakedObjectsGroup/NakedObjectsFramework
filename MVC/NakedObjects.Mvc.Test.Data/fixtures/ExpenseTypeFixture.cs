// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using NakedObjects;

namespace Expenses.Fixtures {
    public class ExpenseTypeFixture {
        public static ExpenseType AIRFARE;
        public static ExpenseType CAR_RENTAL;
        public static ExpenseType GENERAL;
        public static ExpenseType HOTEL;
        public static ExpenseType MEAL;
        public static ExpenseType MOBILE_PHONE;
        public static ExpenseType PRIVATE_CAR;
        public static ExpenseType TAXI;
        public IDomainObjectContainer Container { protected get; set; }

        public void Install() {
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