using System;

namespace NakedObjects.Xat.Generic {
    public interface ITestHasActions<T> : ITestHasActions {



        //ITestAction<T> GetAction(Func<T, Action> f);
        ITestAction<T, TReturn> GetAction<TReturn>(Func<T, Func<TReturn>> f);
        ITestAction<T, TReturn> GetAction<TParm, TReturn>(Func<T, Func<TParm, TReturn>> f);
    }

    public class Customer {
        public void BlackList() {}
        public string GetName() {
            return ""; }
        public string CreateNewOrder1(int prod, object s) {
            return "";
        }
    }


    public static class Test {
        public static void Testf(ITestHasActions<int> tas) {
            ITestAction<int, Type> r0 = tas.GetAction<Type>(x => x.GetType);
            ITestAction<int, bool> rr = tas.GetAction<object, bool>(x => x.Equals);
        }

        public static void TestBlackListWorks(ITestObject<Customer> tas) {
            // ITestObject<Customer> tas = GetService("Customer").Getaction("GetCustomer");

            var r0 = tas.GetAction<int, string>(x => x.CreateNewOrder1);

           
        }
    }
}