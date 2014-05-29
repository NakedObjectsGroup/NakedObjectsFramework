// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;

namespace Expenses.Fixtures {
    public class SvenClaim4Approved : AbstractClaimFixture {
        public static Employee DICK;
        public static Employee SVEN;
        public static Claim SVEN_CLAIM_4;

        public  void Install() {
            SVEN = EmployeeFixture.SVEN;
            DICK = EmployeeFixture.DICK;

            var date1 = new DateTime(2007, 7, 15);

            SVEN_CLAIM_4 = CreateNewClaim(SVEN, DICK, "July 07 - 2 visits to Dublin", ProjectCodeFixture.CODE2, date1);
            AddPrivateCarJourney(SVEN_CLAIM_4, date1, "Own car to airport", "Henley on Thames", "Heathrow", true, 50);
            AddGeneralExpense(SVEN_CLAIM_4, date1, "Car Parking at Heathrow", Convert.ToDecimal(42.9));
            AddAirfare(SVEN_CLAIM_4, date1, null, 165M, "Aer Lingus", "LHR", "DUB", true);
            AddTaxi(SVEN_CLAIM_4, date1, "Taxis to & from Hotel", 30M, "Dublin Airport", "Alexander Hotel", true);
            AddHotel(SVEN_CLAIM_4, date1, "Alexander Hotel", 0M, "http://ocallaghanhotels.visrez.com/dublinmain/Alexander.aspx", 1, 89M, Convert.ToDecimal(15.45), Convert.ToDecimal(3.5));
            AddMeal(SVEN_CLAIM_4, date1, "Dinner", 28M);

            date1 = new DateTime(2007, 7, 23);

            AddPrivateCarJourney(SVEN_CLAIM_4, date1, "Own car to airport", "Henley on Thames", "Heathrow", true, 50);
            AddGeneralExpense(SVEN_CLAIM_4, date1, "Car Parking at Heathrow", Convert.ToDecimal(42.9));
            AddAirfare(SVEN_CLAIM_4, date1, null, 129M, "Aer Lingus", "LHR", "DUB", true);
            AddTaxi(SVEN_CLAIM_4, date1, "Taxis to & from Hotel", 32M, "Dublin Airport", "Alexander Hotel", true);
            AddHotel(SVEN_CLAIM_4, date1, "Alexander Hotel", 0M, "http://ocallaghanhotels.visrez.com/dublinmain/Alexander.aspx", 1, 89M, Convert.ToDecimal(15.45), Convert.ToDecimal(4.8));
            AddMeal(SVEN_CLAIM_4, date1, "Dinner", 31M);

            SVEN_CLAIM_4.Submit(DICK, false);
            SVEN_CLAIM_4.ApproveItems(false);
        }
    }
}