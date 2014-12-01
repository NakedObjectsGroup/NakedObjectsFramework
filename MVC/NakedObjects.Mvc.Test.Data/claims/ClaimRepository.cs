// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;
using Expenses.Services;
using NakedObjects;
using NakedObjects.Core.Util;
using NakedObjects.Services;
using NakedObjects.Menu;

namespace Expenses {
    namespace ExpenseClaims {
        public enum ClaimStatusEnum {
            New,
            Paid
        }

        [Named("Claims")]
        public class ClaimRepository : AbstractFactoryAndRepository {
            #region Injected Services

            #region Injected: UserFinder

            private IUserFinder m_userFinder;

            public IUserFinder UserFinder {
                set { m_userFinder = value; }
            }

            #endregion

            #endregion

            public static string CLAIM_DIFFERENTIATOR = " - ";

            public static void Menu(ITypedMenu<ClaimRepository> menu) {
                menu.CreateSubMenuOfSameType("Find")
                    .AddAction("MyRecentClaims")
                    .AddAction("FindMyClaims")
                    .AddAction("FindMyClaimsByEnumStatus")
                    .AddAction("ClaimsAwaitingMyApproval");
                menu.CreateSubMenuOfSameType("Approve")
                    .AddAction("ApproveClaims");
                menu.AddRemainingNativeActions();
            }


            [NakedObjectsIgnore]
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
                return findExpenseItemsOfType(item.Claim.Claimant, item.ExpenseType);
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
            public IList<Claim> findClaimsAwaitingApprovalBy(Employee approver) {
                IQueryable<Claim> query =
                    from claim in Instances<Claim>()
                    where approver.Equals(claim.Approver) && claim.Status.TitleString.Equals(ClaimStatus.SUBMITTED)
                    select claim;

                return query.ToList();
            }

            [Hidden]
            public IList<AbstractExpenseItem> findExpenseItemsOfType(Employee employee, ExpenseType type) {
                IQueryable<AbstractExpenseItem> query =
                    from item in Instances<AbstractExpenseItem>()
                    where item.ExpenseType.Equals(type) && item.Claim.Claimant.Equals(employee)
                    select item;

                return query.ToList();
            }

            [Eagerly(EagerlyAttribute.Do.Rendering)]
            [TableView(false, "Status", "DateCreated", "Approver")]
            public virtual IList<Claim> FindMyClaims([Optionally] ClaimStatus status, [Optionally] string description) {
                return FindClaims(MeAsEmployee(), status, description);
            }

            public virtual IList<Claim> FindMyClaimsByEnumStatus(ClaimStatusEnum eStatus) {
                ClaimStatus status = FindClaimStatus(eStatus.ToString());

                return FindClaims(MeAsEmployee(), status, null);
            }

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

            [MemberOrder(Sequence = "3")]
            public virtual Claim CreateNewClaim([StringLength(100)] string description) {
                return CreateNewClaim(MeAsEmployee(), description);
            }

            [Executed(Where.Remotely)]
            public virtual string Default0CreateNewClaim() {
                return DefaultUniqueClaimDescription(MeAsEmployee());
            }

            public virtual IList<Claim> ClaimsAwaitingMyApproval() {
                return findClaimsAwaitingApprovalBy(MeAsEmployee());
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
                while (! (DescriptionIsUniqueForClaimant(employee, description))) {
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

            public void ApproveClaims(IEnumerable<Claim> claims) {
                claims.ForEach(c => c.ApproveItems(true));
            }
        }
    }
}

//end of root namespace