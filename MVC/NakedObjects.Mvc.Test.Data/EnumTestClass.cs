// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    public enum TestEnum {
        London,
        Paris,
        NewYork
    };

    [Bounded]
    public class EnumTestClass {
        public IDomainObjectContainer Container { protected get; set; }

        [Title]
        public string Name { get; set; }

        public virtual TestEnum TestActualEnum { get; set; }

        [EnumDataType(typeof (TestEnum))]
        public virtual int TestAnnotationEnum { get; set; }

        public virtual TestEnum TestActualEnumChoices { get; set; }

        [EnumDataType(typeof (TestEnum))]
        public virtual int TestAnnotationEnumChoices { get; set; }

        public TestEnum DefaultTestActualEnum() {
            return TestEnum.NewYork;
        }

        public int DefaultTestAnnotationEnum() {
            return (int) TestEnum.Paris;
        }

        public virtual void TestActualEnumParm(TestEnum parm) {}

        public virtual void TestMultipleActualEnumParm(IEnumerable<TestEnum> parm) {}

        public virtual void TestAnnotationEnumParm([EnumDataType(typeof (TestEnum))] int parm) {}

        public virtual TestEnum[] ChoicesTestActualEnumChoices() {
            return new[] {TestEnum.Paris};
        }

        public virtual int[] ChoicesTestAnnotationEnumChoices() {
            return new[] {(int) TestEnum.Paris};
        }

        public virtual void TestActualEnumParmChoices(TestEnum parm) {}

        public TestEnum[] Choices0TestActualEnumParmChoices(TestEnum parm) {
            return new[] {TestEnum.Paris};
        }

        public virtual void TestAnnotationEnumParmChoices([EnumDataType(typeof (TestEnum))] int parm) {}

        public int[] Choices0TestAnnotationEnumParmChoices(int parm) {
            return new[] {(int) TestEnum.Paris};
        }
    }
}