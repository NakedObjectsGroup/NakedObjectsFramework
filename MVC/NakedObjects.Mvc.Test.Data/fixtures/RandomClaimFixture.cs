// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;
using NakedObjects;
using NakedObjects.Value;

namespace Expenses.Fixtures {
    public class RandomClaimFixture : AbstractClaimFixture {

        

        private const int claimCount = 4;
        private readonly Random random = new Random();
        private IList<ProjectCode> codes;
        private IList<ExpenseType> expenseTypes;
        private int numberOfCodes;

#pragma warning disable 612,618
        private decimal RandomAmount {

            get { return new decimal(random.NextDouble()*1000); }
        }
#pragma warning restore 612,618

        private ProjectCode RandomProjectCode {
            get { return codes[random.Next(numberOfCodes)]; }
        }

        #region Injected Services

        #region Injected: ClaimRepository

        #endregion

        #region Injected: EmployeeRepository

        #endregion

        //
        //		* This region contains references to the services (Repositories, 
        //		* Factories or other Services) used by this domain object.  The 
        //		* references are injected by the application container.
        //		

        #endregion

        public  void Install() {
            LoadExpenseTypes();
            LoadProjectCodes();

            Employee claimant = (EmployeeRepository.FindEmployeeByName("Sven Bloggs")[0]);
            CreateClaims(claimant);
        }

        private void LoadExpenseTypes() {
            IList<ExpenseType> types = Container.Instances<ExpenseType>().ToList();
            expenseTypes = types;
        }

        private void CreateClaims(Employee employee) {
            for (int i = claimCount - 1; i >= 0; i--) {
                AddRandomExpenseItems(CreateClaim(employee));
            }
        }

        private Claim CreateClaim(Employee employee) {
            var rDate = new DateTime(random.Next(7) + 2000, random.Next(12) + 1, random.Next(28) + 1);
            Claim claim = ClaimRepository.CreateNewClaim(employee, rDate.ToString());
            claim.DateCreated = rDate;
            return claim;
        }

        private void AddRandomExpenseItems(Claim claim) {
            for (int count = random.Next(10); count >= 0; count--) {
                AddRandomExpenseItem(claim);
            }
        }

        private void AddRandomExpenseItem(Claim claim) {
            AbstractExpenseItem item = claim.CreateNewExpenseItem(expenseTypes[random.Next(8)]);
            Populate(item);
            Container.Persist(ref item);
        }

        private void Populate(AbstractExpenseItem item) {
            PopulateGeneral(item);

            if (item is Journey) {
                PopulateJourney((Journey) item);
            }
            if (item is Airfare) {
                PopulateAirfare((Airfare) item);
            }
            if (item is CarRental) {
                PopulateCarRental((CarRental) item);
            }
            if (item is PrivateCarJourney) {
                PopulatePrivateCarJourney((PrivateCarJourney) item);
            }

            if (item is Hotel) {
                PopulateHotel((Hotel) item);
            }
        }

        private void PopulateJourney(Journey journey) {
            journey.Origin = (new string[] {"London", "New York", "Tokyo"})[random.Next(3)];
            journey.Destination = (new string[] {"Chicago", "Sydney", "Berlin"})[random.Next(3)];
            journey.Amount = (RandomAmount);
            journey.ReturnJourney = Convert.ToBoolean(((random.Next()%2 == 0) ? false : true));
        }

        private void PopulateAirfare(Airfare airfare) {
            airfare.AirlineAndFlight = (new string[] {"BA", "Air France", "RyanAir"})[random.Next(3)];
        }

        private void PopulateCarRental(CarRental rental) {
            rental.RentalCompany = (new string[] {"Avis", "EasyCar", "Hertz"})[random.Next(3)];
            rental.NumberOfDays = random.Next(14) + 1;
        }

        private void PopulatePrivateCarJourney(PrivateCarJourney car) {
            car.MileageRate = (Convert.ToSingle(random.NextDouble()) + 0.01)*100;
            car.TotalMiles = random.Next(1000) + 1;
        }

        private void PopulateGeneral(AbstractExpenseItem item) {
            var rDate = new DateTime(random.Next(7) + 2000, random.Next(12) + 1, random.Next(28) + 1);
            item.ModifyAmount(RandomAmount);
            //item.modifyDescription(item.[GetType()]().FullName + " " + rDate.ToString())
            item.ModifyDateIncurred(rDate);
            item.ModifyProjectCode(RandomProjectCode);
        }

        private void PopulateHotel(Hotel item) {
            item.HotelURL = (new string[] {"The Grand", "The Ritz", "Albert at Bay"})[random.Next(3)];
            item.Accommodation = (RandomAmount);
            item.Food = (RandomAmount);
            item.Other = (RandomAmount);
            item.NumberOfNights = random.Next(14) + 1;
        }

        private void LoadProjectCodes() {
            IList<ProjectCode> allCodes = Container.Instances<ProjectCode>().ToList();
            codes = allCodes;
            numberOfCodes = allCodes.Count;
        }
    }
}