using NakedObjects.SystemTest.PolymorphicAssociations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.SystemTest.PolymorphicAssociations {
    public class FixtureEntities {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public void Install() {
            //Create Payments 1 to 11
            for (int i = 1; i <= 11; i++) {
                NewPersisted<PolymorphicPayment>();
            }

            NewPersisted<CustomerAsPayee>();
            NewPersisted<CustomerAsPayee>();

            NewPersisted<InvoiceAsPayableItem>();
            NewPersisted<InvoiceAsPayableItem>();
            NewPersisted<InvoiceAsPayableItem>();
        }

        private T NewPersisted<T>() where T : class, new(){
            var t = Container.NewTransientInstance<T>();
            Container.Persist(ref t);
            return t;
        }
    }
}
