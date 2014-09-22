using NakedObjects.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.SystemTest.ObjectFinderCompoundKeys
{
    #region Classes used by test

    public class PaymentContext : DbContext
    {
        public const string DatabaseName = "ObjectFinderWithCompoundKeys";
        public PaymentContext() : base(DatabaseName) { }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<CustomerOne> CustomerOnes { get; set; }
        public DbSet<CustomerTwo> CustomerTwos { get; set; }
        public DbSet<CustomerThree> CustomerThrees { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseAlways<PaymentContext>
    {
        protected override void Seed(PaymentContext context)
        {
            context.Payments.Add(new Payment());
            context.CustomerOnes.Add(new CustomerOne() { Id = 1 });
            context.CustomerTwos.Add(new CustomerTwo() { Id = 1, Id2 = "1001" });
            context.CustomerTwos.Add(new CustomerTwo() { Id = 2, Id2 = "1002" });
            context.CustomerThrees.Add(new CustomerThree() { Id = 1, Id2 = "1001", Number = 2001 });
            context.Suppliers.Add(new Supplier() { Id = 1, Id2 = 2001 });
            context.Employees.Add(new Employee() { Id = 1, Id2 = "foo" });
            context.SaveChanges();
        }
    }

    public class Payment
    {
        public IDomainObjectContainer Container { protected get; set; }
        public virtual int Id { get; set; }

        #region Payee Property (Interface Association)

        private IPayee myPayee;
        public IObjectFinder ObjectFinder { set; protected get; }

        [Optionally]
        public virtual string PayeeCompoundKey { get; set; }

        [NotPersisted, Optionally]
        public IPayee Payee
        {
            get
            {
                if (myPayee == null & !String.IsNullOrEmpty(PayeeCompoundKey))
                {
                    myPayee = ObjectFinder.FindObject<IPayee>(PayeeCompoundKey);
                }
                return myPayee;
            }
            set
            {
                myPayee = value;
                if (value == null)
                {
                    PayeeCompoundKey = null;
                }
                else
                {
                    PayeeCompoundKey = ObjectFinder.GetCompoundKey(value);
                }
            }
        }

        #endregion
    }

    public interface IPayee { }

    public class CustomerOne : IPayee
    {

        public virtual int Id { get; set; }
    }

    public class CustomerTwo : IPayee
    {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }
    }


    public class CustomerThree : IPayee
    {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }

        [Key]
        [Column(Order = 3)]
        public virtual int Number { get; set; }
    }

    public class Supplier : IPayee
    {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual short Id2 { get; set; }
    }


    public class Employee : IPayee
    {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }
    }

    #endregion
}
