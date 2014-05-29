// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using MvcTestApp.Tests.Helpers;
using NakedObjects;

namespace Expenses.Fixtures {
    public class ChoicesTestFixture  {

        public IDomainObjectContainer Container { protected get; set; }


        public static ChoicesTestClass Class1;
        public static ChoicesTestClass Class2;
        public static ChoicesTestClass Class3;

        public static AutoCompleteTestClass Class4;
        public static AutoCompleteTestClass Class5;
        public static AutoCompleteTestClass Class6;

        public static BoolTestClass BoolClass;

        public static EnumTestClass EnumClass;
        public static NotContributedTestClass1 NC1Class;
        public static NotContributedTestClass2 NC2Class;

        public static HintTestClass HintTestClass;

        public void Install() {
            Class1 = CreateChoicesTest("Class1" );
            Class2 = CreateChoicesTest("Class2" );
            Class3 = CreateChoicesTest("Class3");
            Class4 = CreateAutoCompleteTest("Class4");
            Class5 = CreateAutoCompleteTest("Class5");
            Class6 = CreateAutoCompleteTest("Class6");

            BoolClass = CreateBoolTest("BoolClass");
            EnumClass = CreateEnumTest("EnumClass");

            NC1Class = CreateNotContributedTest1("NC1Class");
            NC2Class = CreateNotContributedTest2("NC2Class");

            HintTestClass = CreateHintTestClass("HintTestClass");
        }

        private ChoicesTestClass CreateChoicesTest(string name) {
            var ctc = Container.NewTransientInstance<ChoicesTestClass>();
            ctc.Name = name;
            Container.Persist(ref ctc);
            return ctc;
        }

        private AutoCompleteTestClass CreateAutoCompleteTest(string name) {
            var actc = Container.NewTransientInstance<AutoCompleteTestClass>();
            actc.Name = name;
            Container.Persist(ref actc);
            return actc;
        }

        private HintTestClass CreateHintTestClass(string name) {
            var htc = Container.NewTransientInstance<HintTestClass>();
            htc.TestString = name;
            Container.Persist(ref htc);
            return htc;
        }

        private NotContributedTestClass1 CreateNotContributedTest1(string name) {
            var nctc1 = Container.NewTransientInstance<NotContributedTestClass1>();
            nctc1.Name = name;
            Container.Persist(ref nctc1);
            return nctc1;
        }

        private NotContributedTestClass2 CreateNotContributedTest2(string name) {
            var nctc2 = Container.NewTransientInstance<NotContributedTestClass2>();
            nctc2.Name = name;
            Container.Persist(ref nctc2);
            return nctc2;
        }

        private BoolTestClass CreateBoolTest(string name) {
            var btc = Container.NewTransientInstance<BoolTestClass>();
            btc.Name = name;
            Container.Persist(ref btc);
            return btc;
        }

        private EnumTestClass CreateEnumTest(string name) {
            var etc = Container.NewTransientInstance<EnumTestClass>();
            etc.Name = name;
            Container.Persist(ref etc);
            return etc;
        }
       
    }
}