// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;
using Expenses.Services;
using NakedObjects;
using NakedObjects.Core.Util;
using NakedObjects.Menu;
using NakedObjects.Services;

namespace Expenses {
    namespace ExpenseClaims {
        public enum ClaimStatusEnum {
            New,
            Paid
        }

        [Named("Claims")]
        public class ClaimRepository : AbstractFactoryAndRepository {
            public static string CLAIM_DIFFERENTIATOR = " - ";

            public static void Menu(IMenu menu) {
                menu.AddAction("CreateNewClaim");
                menu.CreateSubMenu("Approve")
                    .AddAction("ApproveClaims");
                menu.CreateSubMenu("Find")
                    .AddAction("MyRecentClaims")
                    .AddAction("FindMyClaims")
                    .AddAction("FindMyClaimsByEnumStatus")
                    .AddAction("ClaimsAwaitingMyApproval");
            }

            //[NakedObjectsIgnore]
            public virtual IList<Claim> AllClaims() {
                return Instances<Claim>().ToList();
            }

            [NakedObjectsIgnore]
            public virtual IList<ProjectCode> ListAllCodes() {
                return Instances<ProjectCode>().ToList();
            }

            [Hidden]
            public virtual bool DescriptionIsUniqueForClaimant(Employee employee, string initialDescription) {
                IQueryable<Claim> query =
                    from claim in Instances<Claim>()
                    where claim.Claimant.Id == employee.Id && claim.Description == initialDescription
                    select claim;

                return !query.Any();
            }

            [Hidden]
            public ClaimStatus FindClaimStatus(string title) {
                IQueryable<ClaimStatus> query =
                    from obj in Instances<ClaimStatus>()
                    where obj.TitleString == title
                    select obj;

                return query.FirstOrDefault();
            }

            [Hidden]
            public virtual ExpenseItemStatus FindExpenseItemStatus(string title) {
                IQueryable<ExpenseItemStatus> query =
                    from obj in Instances<ExpenseItemStatus>()
                    where obj.TitleString == title
                    select obj;

                return query.FirstOrDefault();
            }

            [Hidden]
            public IList<AbstractExpenseItem> FindExpenseItemsLike(AbstractExpenseItem item) {
                // Simple implementation: could be extended to compare any fields that have already been set on the
                // item provided.
                return FindExpenseItemsOfType(item.Claim.Claimant, item.ExpenseType);
            }

            private IList<Claim> FindClaims(Employee employee, ClaimStatus status, string description) {
                IQueryable<Claim> query = Instances<Claim>();

                if (employee != null) {
                    query = query.Where(c => c.Claimant.Id == employee.Id);
                }

                if (status != null) {
                    query = query.Where(c => c.Status.Id == status.Id);
                }

                if (description != null) {
                    query = query.Where(c => c.Description.Contains(description));
                }

                return query.ToList();
            }

            [Hidden]
            public IList<Claim> FindClaimsAwaitingApprovalBy(Employee approver) {
                IQueryable<Claim> query =
                    from claim in Instances<Claim>()
                    where approver.Equals(claim.Approver) && claim.Status.TitleString.Equals(ClaimStatus.SUBMITTED)
                    select claim;

                return query.ToList();
            }

            [Hidden]
            public IList<AbstractExpenseItem> FindExpenseItemsOfType(Employee employee, ExpenseType type) {
                IQueryable<AbstractExpenseItem> query =
                    from item in Instances<AbstractExpenseItem>()
                    where item.ExpenseType.Equals(type) && item.Claim.Claimant.Equals(employee)
                    select item;

                return query.ToList();
            }

            [FinderAction()]
            [MemberOrder(2)]
            [Eagerly(EagerlyAttribute.Do.Rendering)]
            [TableView(false, "Status", "DateCreated", "Approver")]
            public virtual IList<Claim> FindMyClaims([Optionally] ClaimStatus status, [Optionally] string description) {
                return FindClaims(MeAsEmployee(), status, description);
            }

            [FinderAction()]
            [MemberOrder(3)]
            public virtual IList<Claim> FindMyClaimsByEnumStatus(ClaimStatusEnum eStatus) {
                ClaimStatus status = FindClaimStatus(eStatus.ToString());

                return FindClaims(MeAsEmployee(), status, null);
            }

            [FinderAction()]
            [MemberOrder(1)]
            public virtual IList<Claim> MyRecentClaims() {
                return FindClaims(MeAsEmployee(), null, null);
            }

            private Employee MeAsEmployee() {
                object user = m_userFinder.CurrentUserAsObject();
                if (user is Employee) {
                    return (Employee) user;
                }
                throw new Exception("Current user is not an Employee");
            }

            [FinderAction()]
            [MemberOrder(1)]
            public virtual Claim CreateNewClaim([StringLength(100)] string description) {
                return CreateNewClaim(MeAsEmployee(), description);
            }

            [Executed(Where.Remotely)]
            public virtual string Default0CreateNewClaim() {
                return DefaultUniqueClaimDescription(MeAsEmployee());
            }

            [FinderAction()]
            [MemberOrder(4)]
            public virtual IList<Claim> ClaimsAwaitingMyApproval() {
                return FindClaimsAwaitingApprovalBy(MeAsEmployee());
            }

            [Hidden]
            public virtual Claim CreateNewClaim(Employee employee, string description) {
                var newClaim = NewTransientInstance<Claim>();
                newClaim.Claimant = employee;
                newClaim.Approver = employee.NormalApprover;
                newClaim.InitialiseTotal();
                newClaim.Description = CreateUniqueDescription(employee, description);
                Persist(ref newClaim);
                newClaim.ChangeStatusToNew();
                return newClaim;
            }

            [Hidden]
            public virtual string DefaultUniqueClaimDescription(Employee employee) {
                return CreateUniqueDescription(employee, CreateDefaultClaimDescription(null));
            }

            [Hidden]
            public virtual string CreateDefaultClaimDescription(string inputDescription) {
                if (string.IsNullOrEmpty(inputDescription)) {
                    return DateTime.Now.ToShortDateString();
                }
                return inputDescription;
            }

            private string CreateUniqueDescription(Employee employee, string initialDescription) {
                int increment = 2;
                string description = initialDescription;
                while (!(DescriptionIsUniqueForClaimant(employee, description))) {
                    description = initialDescription + CLAIM_DIFFERENTIATOR + increment;
                    increment += 1;
                }
                return description;
            }

            [Hidden]
            public virtual AbstractExpenseItem CreateNewExpenseItem(Claim claim, ExpenseType typeOfExpense) {
                var item = (AbstractExpenseItem) (NewTransientInstance(Type.GetType(typeOfExpense.CorrespondingClassName)));
                item.ExpenseType = typeOfExpense;
                item.ModifyProjectCode(claim.ProjectCode);
                item.Claim = claim;
                item.InitialiseAmount();
                return item;
            }

            public void ApproveClaims([ContributedAction("Claims")] IQueryable<Claim> claims) {
                claims.ForEach(c => c.ApproveItems(true));
            }

            #region Injected Services

            #region Injected: UserFinder

            private IUserFinder m_userFinder;

            public IUserFinder UserFinder {
                set { m_userFinder = value; }
            }

            #endregion

            #endregion
        }
    }
}

//end of root namespace