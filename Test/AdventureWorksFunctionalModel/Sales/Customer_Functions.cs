






using System;
using AW.Types;

namespace AW.Functions {
    public static class Customer_Functions {
        internal static BusinessEntity BusinessEntity(this Customer c) {
            if (c.Store is not null) {
                return c.Store;
            }

            if (c.Person is not null) {
                return c.Person;
            }

            throw new Exception("Customer is neither Store nor Person!");
        }

        public static bool HideStore(this Customer c) => !IsStore(c);

        public static bool HidePerson(this Customer c) => !IsIndividual(c);

        internal static bool IsIndividual(this Customer c) => !IsStore(c);

        internal static bool IsStore(this Customer c) => c.Store != null;
    }
}