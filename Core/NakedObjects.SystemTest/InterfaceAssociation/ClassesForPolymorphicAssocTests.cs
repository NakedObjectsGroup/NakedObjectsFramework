// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace NakedObjects.SystemTest.PolymorphicAssociations {
    #region Classes used by test

    public interface IPayee : IHasIntegerId { }

    public class PolymorphicPayment : IHasIntegerId {
        public Services.PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public IDomainObjectContainer Container { protected get; set; }

        #region IHasIntegerId Members

        public virtual int Id { get; set; }

        #endregion

        #region Payee Property of type IPayee ('role' interface)

        private IPayee _Payee;

        [Disabled]
        public virtual PolymorphicPaymentPayeeLink PayeeLink { get; set; }

        [NotPersisted]
        [Optionally]
        public IPayee Payee {
            get => PolymorphicNavigator.RoleObjectFromLink(ref _Payee, PayeeLink, this);
            set {
                _Payee = value;
                PayeeLink = PolymorphicNavigator.UpdateAddOrDeleteLink(_Payee, PayeeLink, this);
            }
        }

        //TODO:  Move this method into LifeCycle Methods region, or add code into existing Persisting method
        public void Persisting() {
            PayeeLink = PolymorphicNavigator.NewTransientLink<PolymorphicPaymentPayeeLink, IPayee, PolymorphicPayment>(_Payee, this);
        }

        #endregion

        #region PayableItems Collection of type IPayableItem

        //TODO:  Create a type 'PolymorphicPaymentPayableItemLink', which can either inherit from PolymorphicLink<IPayableItem, PolymorphicPayment>
        //or otherwise implement IPolymorphicLink<IPayableItem, PolymorphicPayment>.

        [Hidden(WhenTo.Always)]
        public virtual ICollection<PolymorphicPaymentPayableItemLink> PayableItemLinks { get; set; } = new List<PolymorphicPaymentPayableItemLink>();

        /// <summary>
        ///     This is an optional, derived collection, which shows the associated objects directly.
        ///     It is more convenient for the user, but each element is resolved separately, so more
        ///     expensive in processing terms.  Use this pattern only on smaller collections.
        /// </summary>
        [NotPersisted]
        public ICollection<IPayableItem> PayableItems {
            get { return PayableItemLinks.Select(x => x.AssociatedRoleObject).ToList(); }
        }

        public void AddPayableItem(IPayableItem value) {
            PolymorphicNavigator.AddLink<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(value, this);
        }

        public void RemovePayableItem(IPayableItem value) {
            PolymorphicNavigator.RemoveLink<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(value, this);
        }

        #endregion
    }

    public class PolymorphicPaymentPayeeLink : PolymorphicLink<IPayee, PolymorphicPayment> { }

    public class CustomerAsPayee : IPayee {
        public Services.PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        #region IPayee Members

        [Key]
        public virtual int Id { get; set; }

        #endregion

        public IQueryable<PolymorphicPayment> PaymentsToThisPayee() => PolymorphicNavigator.FindOwners<PolymorphicPaymentPayeeLink, IPayee, PolymorphicPayment>(this);
    }

    public class SupplierAsPayee : IPayee {
        #region IPayee Members

        [Key]
        public virtual int Id { get; set; }

        #endregion
    }

    public interface IPayableItem : IHasIntegerId { }

    public class PolymorphicPaymentPayableItemLink : PolymorphicLink<IPayableItem, PolymorphicPayment> { }

    public class InvoiceAsPayableItem : IPayableItem {
        public Services.PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        #region IPayableItem Members

        [Key]
        public virtual int Id { get; set; }

        #endregion

        public IQueryable<PolymorphicPayment> PaymentsContainingThis() => PolymorphicNavigator.FindOwners<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(this);
    }

    public class ExpenseClaimAsPayableItem : IPayableItem {
        #region IPayableItem Members

        [Key]
        public virtual int Id { get; set; }

        #endregion
    }

    #endregion

    #region Code First DBContext

    public class PolymorphicNavigationContext : DbContext {
        public PolymorphicNavigationContext(string name) : base(GetCs(name)) { }
        public DbSet<PolymorphicPayment> Payments { get; set; }
        public DbSet<PolymorphicPaymentPayeeLink> PayeeLinks { get; set; }
        public DbSet<PolymorphicPaymentPayableItemLink> PayableItemLinks { get; set; }
        public DbSet<CustomerAsPayee> Customers { get; set; }
        public DbSet<SupplierAsPayee> Suppliers { get; set; }
        public DbSet<InvoiceAsPayableItem> Invoices { get; set; }
        public DbSet<ExpenseClaimAsPayableItem> ExpenseClaims { get; set; }
        private static string GetCs(string name) => @$"Data Source={Constants.Server};Initial Catalog={name};Integrated Security=True;";

        public static void Delete(string name) => Database.Delete(GetCs(name));

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer(new DropCreateDatabaseAlways<PolymorphicNavigationContext>());
        }
    }

    #endregion
}