// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace NakedObjects.SystemTest.PolymorphicAssociations {

    #region Classes used by test

    public interface IPayee : IHasIntegerId {}

    public class PolymorphicPayment : IHasIntegerId {
        public Services.PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        public IDomainObjectContainer Container { protected get; set; }

        public virtual int Id { get; set; }

        #region Payee Property of type IPayee ('role' interface)

        //TODO:  Create a type 'PolymorphicPaymentPayeeLink', which can either inherit from PolymorphicLink<IPayee, PolymorphicPayment>
        //or otherwise implement IPolymorphicLink<IPayee, PolymorphicPayment>.

        private IPayee _Payee;

        [Disabled]
        public virtual PolymorphicPaymentPayeeLink PayeeLink { get; set; }

        [NotPersisted, Optionally]
        public IPayee Payee {
            get { return PolymorphicNavigator.RoleObjectFromLink(ref _Payee, PayeeLink, this); }
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

        private ICollection<PolymorphicPaymentPayableItemLink> _PayableItem = new List<PolymorphicPaymentPayableItemLink>();

        [Hidden]
        public virtual ICollection<PolymorphicPaymentPayableItemLink> PayableItemLinks {
            get { return _PayableItem; }
            set { _PayableItem = value; }
        }

        /// <summary>
        ///     This is an optional, derrived collection, which shows the associated objects directly.
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

    public class PolymorphicPaymentPayeeLink : PolymorphicLink<IPayee, PolymorphicPayment> {}


    public class CustomerAsPayee : IPayee {
        public Services.PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        [Key]
        public virtual int Id { get; set; }

        public IQueryable<PolymorphicPayment> PaymentsToThisPayee() {
            return PolymorphicNavigator.FindOwners<PolymorphicPaymentPayeeLink, IPayee, PolymorphicPayment>(this);
        }
    }


    public class SupplierAsPayee : IPayee {
        [Key]
        public virtual int Id { get; set; }
    }

    public interface IPayableItem : IHasIntegerId {};

    public class PolymorphicPaymentPayableItemLink : PolymorphicLink<IPayableItem, PolymorphicPayment> {}

    public class InvoiceAsPayableItem : IPayableItem {
        public Services.PolymorphicNavigator PolymorphicNavigator { set; protected get; }

        [Key]
        public virtual int Id { get; set; }

        public IQueryable<PolymorphicPayment> PaymentsContainingThis() {
            return PolymorphicNavigator.FindOwners<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(this);
        }
    }


    public class ExpenseClaimAsPayableItem : IPayableItem {
        [Key]
        public virtual int Id { get; set; }
    }

    #endregion

    #region Code First DBContext 

    public class MyContext : DbContext {
        public MyContext(string name) : base(name) {}
        public MyContext() {}
        public DbSet<PolymorphicPayment> Payments { get; set; }
        public DbSet<CustomerAsPayee> Customers { get; set; }
        public DbSet<SupplierAsPayee> Suppliers { get; set; }
        public DbSet<InvoiceAsPayableItem> Invoices { get; set; }
        public DbSet<ExpenseClaimAsPayableItem> ExpenseClaims { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //Initialisation
            //Use the Naked Objects > DbInitialiser template to add a custom initialiser, then reference thus:
            Database.SetInitializer(new DropCreateDatabaseAlways<MyContext>());

            //Mappings
            //Use the Naked Objects > Mapping template to add mapping classes & reference them thus:
            //modelBuilder.Configurations.Add(new Employee_Mapping());
        }
    }

    #endregion
}