using System.Linq;

namespace NakedObjects.SystemTest.PolymorphicAssociations {
    public abstract class FixtureLinks {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public void Install() {

            //Link Payment3 to Customer1
            var payment3 = FindById<PolymorphicPayment>(3);
            var link1 = Container.NewTransientInstance<PolymorphicPaymentPayeeLink>();
            link1.AssociatedRoleObjectType = CustomerType();
            link1.AssociatedRoleObjectId = 1;
            link1.Owner = payment3;
            Container.Persist(ref link1);

            //Link Payment6 to Invoice1
            var payment6 = FindById<PolymorphicPayment>(6);
            var link2 = Container.NewTransientInstance<PolymorphicPaymentPayableItemLink>();
            link2.AssociatedRoleObjectType = InvoiceType();
            link2.AssociatedRoleObjectId = 1;
            link2.Owner = payment6;
            Container.Persist(ref link2);

            //Link Payment7 to Invoice1
            var payment7 = Container.Instances<PolymorphicPayment>().Single(pp => pp.Id == 7);
            var link3 = Container.NewTransientInstance<PolymorphicPaymentPayableItemLink>();
            link3.AssociatedRoleObjectType = InvoiceType();
            link3.AssociatedRoleObjectId = 1;
            link3.Owner = payment7;
            Container.Persist(ref link3);

            //Fixtire for test FindOwnersForObject
            var payment9 = FindById<PolymorphicPayment>(9);
            var payment10 = FindById<PolymorphicPayment>(10);
            var payment11 = FindById<PolymorphicPayment>(11);
            var cus2 = FindById<CustomerAsPayee>(2);
            var inv3 = FindById<InvoiceAsPayableItem>(3);
            payment9.Payee = cus2;
            payment11.Payee = cus2;
            payment9.AddPayableItem(inv3);
            payment10.AddPayableItem(inv3);

        }

        protected abstract string InvoiceType();

        protected abstract string CustomerType();

        private T FindById<T>(int id) where T : class, IHasIntegerId, new(){
            return Container.Instances<T>().Single(x => x.Id == id);
        }
    }

    public class FixtureLinksUsingTypeName : FixtureLinks {

        protected override string InvoiceType() {
            return "NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem";
        }

        protected override string CustomerType() {
            return "NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee";
        }
    }

    public class FixtureLinksUsingTypeCode : FixtureLinks {

        protected override string InvoiceType() {
            return "INV";
        }

        protected override string CustomerType() {
            return "CUS";
        }
    }
}
