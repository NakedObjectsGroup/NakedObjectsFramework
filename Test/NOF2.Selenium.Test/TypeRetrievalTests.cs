﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;

namespace NOF2.Selenium.Test;

[TestClass]
public class TypeRetrievalTests : BaseTest {
    private const string prefix = "object?i1=View&o1=AW.Types.";

    [TestMethod]
    public void RetrieveInstanceOfTypesDirectlyByURL() {
        AccessInstanceWithTitle("Department--1 ", "Engineering");
        AccessInstanceWithTitle("Employee--2", "Terri Duffy");
        AccessInstanceWithTitle("EmployeeDepartmentHistory--1--16--1--631808640000000000", "Executive 15/02/2003");
        AccessInstanceWithTitle("EmployeePayHistory--1--631808640000000000", "£125.50 from 15/02/2003");
        AccessInstanceWithTitle("JobCandidate--1", "Job Candidate");
        AccessInstanceWithTitle("Shift--1", "Day");
        AccessInstanceWithTitle("Address--1", "1970 Napa Ct....");
        AccessInstanceWithTitle("AddressType--1", "Billing");
        AccessInstanceWithTitle("BusinessEntity--2", "Terri Duffy");
        AccessInstanceWithTitle("BusinessEntityAddress--11174--20240--2", "Home: 1868 Alexander Pl...");
        AccessInstanceWithTitle("BusinessEntityContact--1168--1167--11", "Greg Johnson");
        AccessInstanceWithTitle("ContactType--1", "Accounting Manager");
        AccessInstanceWithTitle("CountryRegion--AD", "Andorra");
        AccessInstanceWithTitle("EmailAddress--1--1", "ken0@adventure-works.com");
        AccessInstanceWithTitle("Password--1", "Password");
        AccessInstanceWithTitle("Person--2", "Terri Duffy");
        AccessInstanceWithTitle("PersonPhone--1--697-555-0142--1", "Cell:697-555-0142");
        AccessInstanceWithTitle("PhoneNumberType--1", "Cell");
        AccessInstanceWithTitle("StateProvince--1", "Alberta");
        AccessInstanceWithTitle("BillOfMaterial--893", "BillOfMaterial: 893");
        AccessInstanceWithTitle("Culture--ar", "Arabic");
        AccessInstanceWithTitle("Illustration--3", "Illustration: 3");
        AccessInstanceWithTitle("Location--1", "Tool Crib");
        AccessInstanceWithTitle("Product--1", "Adjustable Race");
        AccessInstanceWithTitle("ProductCategory--1", "Bikes");
        AccessInstanceWithTitle("ProductCostHistory--707--632557728000000000", "£12.03 01/07/2005~");
        AccessInstanceWithTitle("ProductDescription--3", "Chromoly steel.");
        AccessInstanceWithTitle("ProductInventory--1--1", "408 in Tool Crib - A");
        AccessInstanceWithTitle("ProductListPriceHistory--707--632557728000000000", "ProductListPriceHistory: 707");
        AccessInstanceWithTitle("ProductModel--1", "Classic Vest");
        AccessInstanceWithTitle("ProductModelIllustration--7--3", "ProductModelIllustration: 7-3");
        AccessInstanceWithTitle("ProductModelProductDescriptionCulture--1--1199--en%20%20%20%20", "ProductModelProductDescriptionCulture: 1-1199-en");
        AccessInstanceWithTitle("ProductPhoto--1", "Product Photo: 1");
        AccessInstanceWithTitle("ProductProductPhoto--1--1", "ProductProductPhoto: 1-1");
        AccessInstanceWithTitle("ProductReview--1", "*****");
        AccessInstanceWithTitle("ProductSubcategory--1", "Mountain Bikes");
        AccessInstanceWithTitle("ScrapReason--1", "Brake assembly not as ordered");
        AccessInstanceWithTitle("TransactionHistory--100000", "TransactionHistory: 100000");
        AccessInstanceWithTitle("UnitMeasure--BOX", "Boxes");
        AccessInstanceWithTitle("WorkOrder--1", "LL Road Frame - Black, 58: 04/07/2005");
        AccessInstanceWithTitle("WorkOrderRouting--13--747--1", "Frame Forming");
        AccessInstanceWithTitle("ProductVendor--1--1580", "ProductVendor: 1-1580");
        AccessInstanceWithTitle("PurchaseOrderDetail--1--1", "4 x Adjustable Race");
        AccessInstanceWithTitle("PurchaseOrderHeader--1", "PO from Litware, Inc., 17/05/2005");
        AccessInstanceWithTitle("ShipMethod--1", "XRQ - TRUCK GROUND");
        AccessInstanceWithTitle("Vendor--1492", "Australia Bike Retailer");
        AccessInstanceWithTitle("CountryRegionCurrency--AE--AED", "CountryRegionCurrency: United Arab Emirates AED - Emirati Dirham");
        AccessInstanceWithTitle("CreditCard--1", "**********5310");
        AccessInstanceWithTitle("Currency--AED", "AED - Emirati Dirham");
        AccessInstanceWithTitle("CurrencyRate--1", "1.0000");
        AccessInstanceWithTitle("Customer--1", "AW00000001 A Bike Store");
        AccessInstanceWithTitle("PersonCreditCard--293--17038", "PersonCreditCard: 293-17038");
        AccessInstanceWithTitle("SalesOrderDetail--43659--1", "1 x Mountain-100 Black, 42");
        AccessInstanceWithTitle("SalesOrderHeader--43659", "SO43659");
        AccessInstanceWithTitle("SalesOrderHeaderSalesReason--43697--5", "SalesOrderHeaderSalesReason: 43697-5");
        AccessInstanceWithTitle("SalesPerson--274", "Stephen Jiang");
        AccessInstanceWithTitle("SalesReason--1", "Price");
        AccessInstanceWithTitle("SalesTaxRate--1", "Sales Tax Rate for Alberta");
        AccessInstanceWithTitle("SalesTerritory--1", "Northwest");
        AccessInstanceWithTitle("SalesTerritoryHistory--275--2--632557728000000000", "Michael Blythe Northeast");
        AccessInstanceWithTitle("ShoppingCartItem--2", "3 x Full-Finger Gloves, M");
        AccessInstanceWithTitle("SpecialOffer--1", "No Discount");
        AccessInstanceWithTitle("SpecialOfferProduct--1--680", "SpecialOfferProduct: 1-680");
        AccessInstanceWithTitle("Store--292", "Next-Door Bike Store");
    }

    private void AccessInstanceWithTitle(string identifier, string title) {
        helper.GotoProductUrlDirectly(prefix + identifier);
        helper.GetObjectView().AssertTitleIs(title);
    }

    #region Overhead

    protected override string BaseUrl => TestConfig.BaseLegacyUrl;

    private Helper helper;

    [TestInitialize]
    public virtual void InitializeTest() {
        helper = new Helper(BaseUrl, "gemini", Driver, Wait);
    }

    #endregion
}