// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using NakedObjects.Services;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.ObjectFinderCompoundKeys {
    #region Classes used by test

    public class PaymentContext : DbContext {
        public const string DatabaseName = "ObjectFinderWithCompoundKeys";
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public PaymentContext() : base(Cs) { }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<CustomerOne> CustomerOnes { get; set; }
        public DbSet<CustomerTwo> CustomerTwos { get; set; }
        public DbSet<CustomerThree> CustomerThrees { get; set; }
        public DbSet<CustomerFour> CustomerFours { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public static void Delete() => Database.Delete(Cs);
      
    }

    public class DatabaseInitializer {
        public static void Seed(PaymentContext context) {
            context.Payments.Add(new Payment());
            context.CustomerOnes.Add(new CustomerOne {Id = 1});
            context.CustomerTwos.Add(new CustomerTwo {Id = 1, Id2 = "1001"});
            context.CustomerTwos.Add(new CustomerTwo {Id = 2, Id2 = "1002"});
            context.CustomerThrees.Add(new CustomerThree {Id = 1, Id2 = "1001", Number = 2001});
            context.CustomerFours.Add(new CustomerFour {Id = 1, Id2 = new DateTime(2015, 1, 16)});
            context.CustomerFours.Add(new CustomerFour {Id = 1, Id2 = new DateTime(2015, 1, 17)});
            context.Suppliers.Add(new Supplier {Id = 1, Id2 = 2001});
            context.Employees.Add(new Employee {Id = 1, Id2 = "foo"});
            context.SaveChanges();
        }
    }

    public class Payment {
        public IDomainObjectContainer Container { protected get; set; }
        public virtual int Id { get; set; }

        #region Payee Property (Interface Association)

        private IPayee myPayee;
        public IObjectFinder ObjectFinder { set; protected get; }

        [Optionally]
        public virtual string PayeeCompoundKey { get; set; }

        [NotPersisted]
        [Optionally]
        public IPayee Payee {
            get {
                if ((myPayee == null) & !string.IsNullOrEmpty(PayeeCompoundKey)) {
                    myPayee = ObjectFinder.FindObject<IPayee>(PayeeCompoundKey);
                }

                return myPayee;
            }
            set {
                myPayee = value;
                PayeeCompoundKey = value == null ? null : ObjectFinder.GetCompoundKey(value);
            }
        }

        #endregion
    }

    public interface IPayee { }

    public class CustomerOne : IPayee {
        public virtual int Id { get; set; }
    }

    public class CustomerTwo : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }
    }

    public class CustomerThree : IPayee {
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

    public class CustomerFour : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual DateTime Id2 { get; set; }
    }

    public class Supplier : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual short Id2 { get; set; }
    }

    public class Employee : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }
    }

    #endregion
}