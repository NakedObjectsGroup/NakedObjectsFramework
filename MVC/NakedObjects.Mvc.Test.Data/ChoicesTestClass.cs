// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using Expenses.ExpenseClaims;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [Bounded]
    public class ChoicesTestClass {

        public IDomainObjectContainer Container { protected get; set; }

        [Title]
        public string Name { get; set; }

        public ChoicesTestClass TestChoicesProperty { get; set; }

        public string TestChoicesStringProperty { get; set; }

        public IEnumerable<ChoicesTestClass> ChoicesTestChoicesProperty() {
            return Container.Instances<ChoicesTestClass>().ToList();
        }

        public IEnumerable<string> ChoicesTestChoicesStringProperty() {
            return new List<string> {"test1", "test2"};
        }

        public void TestChoicesAction(ChoicesTestClass parm1, string parm2) {}

        public IEnumerable<ChoicesTestClass> Choices0TestChoicesAction() {
            return Container.Instances<ChoicesTestClass>().ToList();
        }

        public IEnumerable<string> Choices1TestChoicesAction() {
            return new List<string> {"test1", "test2"};
        }

        public void TestChoicesAction4(ChoicesTestClass parm1, string parm2) {}

        public IEnumerable<ChoicesTestClass> Choices0TestChoicesAction4() {
            return Container.Instances<ChoicesTestClass>().ToList();
        }

        public IEnumerable<string> Choices1TestChoicesAction4() {
            return new List<string> {"test1", "test2"};
        }

        public ChoicesTestClass Default0TestChoicesAction4() {
            return Container.Instances<ChoicesTestClass>().First();
        }

        public string Default1TestChoicesAction4() {
            return "test1";
        }


        public void TestChoicesAction2(ChoicesTestClass parm1, string parm2) {}

        public IEnumerable<ChoicesTestClass> ChoicesTestChoicesAction2(ChoicesTestClass parm1) {
            return Container.Instances<ChoicesTestClass>().ToList();
        }

        public IEnumerable<string> ChoicesTestChoicesAction2(string parm2) {
            return new List<string> {"test1", "test2"};
        }

        public void TestChoicesAction3(ChoicesTestClass parm1, string parm2) {}

        public IEnumerable<ChoicesTestClass> ChoicesTestChoicesAction3(ChoicesTestClass parm1) {
            return Container.Instances<ChoicesTestClass>().ToList();
        }

        public IEnumerable<string> ChoicesTestChoicesAction3(string parm2) {
            return new List<string> {"test1", "test2"};
        }

        public ChoicesTestClass DefaultTestChoicesAction3(ChoicesTestClass parm1) {
            return Container.Instances<ChoicesTestClass>().First();
        }

        public string DefaultTestChoicesAction3(string parm2) {
            return "test1";
        }

        public void TestMultipleChoicesDomainObject1(IEnumerable<Claim> parm1) {}

        public IEnumerable<Claim> Choices0TestMultipleChoicesDomainObject1() {
            return Container.Instances<Claim>().ToList();
        }

        public void TestMultipleChoicesDomainObject2(IQueryable<Claim> parm1) {}

        public IEnumerable<Claim> Choices0TestMultipleChoicesDomainObject2() {
            return Container.Instances<Claim>().ToList();
        }

        public void TestMultipleChoicesString(IEnumerable<string> parm1) {}

        public IEnumerable<string> Choices0TestMultipleChoicesString() {
            return new[] {"s1", "s2"};
        }

        public void TestMultipleChoicesInt(IEnumerable<int> parm1) {}

        public IEnumerable<int> Choices0TestMultipleChoicesInt() {
            return new[] {1, 2};
        }

        public void TestMultipleChoicesAction4(IEnumerable<ChoicesTestClass> parm1, IEnumerable<string> parm2) {}

        public IEnumerable<ChoicesTestClass> Choices0TestMultipleChoicesAction4() {
            return Container.Instances<ChoicesTestClass>().ToList();
        }

        public IEnumerable<string> Choices1TestMultipleChoicesAction4() {
            return new List<string> {"test1", "test2"};
        }

        public IEnumerable<ChoicesTestClass> Default0TestMultipleChoicesAction4() {
            return Container.Instances<ChoicesTestClass>();
        }

        public IEnumerable<string> Default1TestMultipleChoicesAction4() {
            return new[] {"test1", "test2"};
        }


        public void TestMultipleChoicesBounded(IEnumerable<ChoicesTestClass> parm1) {}

        public void TestQueryableAction(IQueryable<ViewModelTestClass> parm1) {}

        public void TestEnumerableAction(IEnumerable<ViewModelTestClass> parm1) {}
    }
}