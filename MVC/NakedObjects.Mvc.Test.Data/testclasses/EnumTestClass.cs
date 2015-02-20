// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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

        [Hidden, Key]
        public int Id { get; set; }

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