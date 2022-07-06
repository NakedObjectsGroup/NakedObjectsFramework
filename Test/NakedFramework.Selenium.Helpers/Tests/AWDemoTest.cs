// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Selenium.Helpers.Tests; 

public abstract class AWTest : GeminiTest {
    protected int CustomerServiceActions = 5;
    protected int MainMenusCount = 10; //'Empty' menu should not show
    protected int OrderServiceActions = 8;
    protected string CustomersMenuUrl => GeminiBaseUrl + "home?m1=CustomerRepository";
    protected string OrdersMenuUrl => GeminiBaseUrl + "home?m1=OrderRepository";
    protected string SpecialOffersMenuUrl => GeminiBaseUrl + "home?m1=SpecialOfferRepository";
    protected string ProductServiceUrl => GeminiBaseUrl + "home?m1=ProductRepository";
    protected string SalesServiceUrl => GeminiBaseUrl + "home?m1=SalesRepository";
    protected string WorkOrdersMenuUrl => GeminiBaseUrl + "home?m1=WorkOrderRepository";
}