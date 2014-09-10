// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.RecordedActions;
using MvcTestApp.Tests.Helpers;

namespace NakedObjects.Mvc.Test.Data {
    public class MvcTestContext : DbContext {
        public MvcTestContext(string name) : base(name) {}

        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<ExpenseItemStatus> ExpenseItemStatuses { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }
        public DbSet<ProjectCode> ProjectCodes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<RecordedAction> RecordedActions { get; set; }
        public DbSet<ChoicesTestClass> ChoicesTestClasses { get; set; }
        public DbSet<AutoCompleteTestClass> AutoCompleteTestClasses { get; set; }
        public DbSet<EnumTestClass> EnumTestClasses { get; set; }
        public DbSet<BoolTestClass> BoolTestClasses { get; set; }
        public DbSet<NotContributedTestClass1> NotContributedTestClass1s { get; set; }
        public DbSet<NotContributedTestClass2> NotContributedTestClass2s { get; set; }
        public DbSet<HintTestClass> HintTestClasses { get; set; }
    }
}