// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;
using NakedObjects;

namespace Expenses.Fixtures {
    public abstract class AbstractClaimFixture {
        protected internal virtual Claim CreateNewClaim(Employee employee, Employee approver, string description, ProjectCode projectCode, DateTime dateCreated) {
            Claim claim = ClaimRepository.CreateNewClaim(employee, description);
            claim.ModifyApprover(approver);
            claim.ModifyProjectCode(projectCode);
            claim.DateCreated = dateCreated;
            return claim;
        }

        private AbstractExpenseItem CreateExpenseItem(Claim claim, ExpenseType type, DateTime dateIncurred, string description, decimal amount) {
            AbstractExpenseItem item = (claim.CreateNewExpenseItem(type));
            item.ModifyDateIncurred(dateIncurred);
            item.ModifyDescription(description);
            item.ModifyAmount(Money(amount, claim));
            return item;
        }

        private static void ModifyStandardJourneyFields(Journey journey, string origin, string destination, bool returnJourney) {
            journey.ModifyOrigin(origin);
            journey.ModifyDestination(destination);
            bool tempAux = returnJourney;
            journey.ModifyReturnJourney(ref tempAux);
        }

        protected internal virtual GeneralExpense AddGeneralExpense(Claim claim, DateTime dateIncurred, string description, decimal amount) {
            var item = (GeneralExpense) (CreateExpenseItem(claim, ExpenseTypeFixture.GENERAL, dateIncurred, description, amount));
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual Airfare AddAirfare(Claim claim, DateTime dateIncurred, string description, decimal amount, string airline, string origin, string destination, bool returnJourney) {
            var item = (Airfare) (CreateExpenseItem(claim, ExpenseTypeFixture.AIRFARE, dateIncurred, description, amount));
            item.ModifyAirlineAndFlight(airline);
            ModifyStandardJourneyFields(item, origin, destination, returnJourney);
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual Hotel AddHotel(Claim claim, DateTime dateIncurred, string description, decimal amount, string hotelURL, int numberOfNights, decimal accommodation, decimal food, decimal other) {
            var item = (Hotel) (CreateExpenseItem(claim, ExpenseTypeFixture.HOTEL, dateIncurred, description, amount));
            item.ModifyHotelURL(hotelURL);
            item.ModifyNumberOfNights(numberOfNights);
            item.ModifyAccommodation(Money(accommodation, claim));
            item.ModifyFood(Money(food, claim));
            item.ModifyOther(Money(other, claim));
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual CarRental AddCarRental(Claim claim, DateTime dateIncurred, string description, decimal amount, string rentalCompany, int noOfDays) {
            var item = (CarRental) (CreateExpenseItem(claim, ExpenseTypeFixture.CAR_RENTAL, dateIncurred, description, amount));
            item.ModifyRentalCompany(rentalCompany);
            item.ModifyNumberOfDays(noOfDays);
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual GeneralExpense AddMobilePhone(Claim claim, DateTime dateIncurred, string phoneNumber, decimal amount) {
            var item = (GeneralExpense) (CreateExpenseItem(claim, ExpenseTypeFixture.MOBILE_PHONE, dateIncurred, "Phone No. " + phoneNumber, amount));
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual PrivateCarJourney AddPrivateCarJourney(Claim claim, DateTime dateIncurred, string description, string origin, string destination, bool returnJourney, int totalMiles) {
            var item = (PrivateCarJourney) (CreateExpenseItem(claim, ExpenseTypeFixture.PRIVATE_CAR, dateIncurred, description, 0M));
            ModifyStandardJourneyFields(item, origin, destination, returnJourney);
            item.ModifyTotalMiles(totalMiles);
            item.ModifyMileageRate(0.4);
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual Taxi AddTaxi(Claim claim, DateTime dateIncurred, string description, decimal amount, string origin, string destination, bool returnJourney) {
            var item = (Taxi) (CreateExpenseItem(claim, ExpenseTypeFixture.TAXI, dateIncurred, description, amount));
            ModifyStandardJourneyFields(item, origin, destination, returnJourney);
            Container.Persist(ref item);
            return item;
        }

        protected internal virtual GeneralExpense AddMeal(Claim claim, DateTime dateIncurred, string description, decimal amount) {
            var item = (GeneralExpense) (CreateExpenseItem(claim, ExpenseTypeFixture.MEAL, dateIncurred, description, amount));
            Container.Persist(ref item);
            return item;
        }
#pragma warning disable 612,618
        private static decimal Money(decimal amount, Claim claim) {
            return amount;
        }
#pragma warning restore 612,618

        #region Injected Services

        public IDomainObjectContainer Container { protected get; set; }

        #region Injected: ClaimRepository

        public ClaimRepository ClaimRepository { protected get; set; }

        #endregion

        #region Injected: EmployeeRepository

        public EmployeeRepository EmployeeRepository { protected get; set; }

        #endregion

        #endregion
    }
}