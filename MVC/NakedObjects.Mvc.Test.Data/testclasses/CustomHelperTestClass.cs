// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    public class CustomHelperTestClass {
        private ICollection<CustomHelperTestClass> testCollectionOne = new List<CustomHelperTestClass>();
        private ICollection<CustomHelperTestClass> testCollectionTwo = new List<CustomHelperTestClass>();
        private InlineTestClass testInline = new InlineTestClass();

        [NakedObjectsIgnore, Key]
        public int Id { get; set; }

        public int TestInt { get; set; }
        public string TestString { get; set; }

        [FindMenu]
        public CustomHelperTestClass TestRef { get; set; }

        public int TestIntDefault { get; set; }

        [NakedObjectsIgnore]
        public int HiddenTestInt { get; set; }

        [Disabled]
        public int DisabledTestInt { get; set; }

        public InlineTestClass TestInline {
            get { return testInline; }
            set { testInline = value; }
        }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        public InlineTestClass TestInlineEager {
            get { return testInline; }
            set { testInline = value; }
        }

        public ICollection<CustomHelperTestClass> TestCollectionOne {
            get { return testCollectionOne; }
            set { testCollectionOne = value; }
        }

        public ICollection<CustomHelperTestClass> TestCollectionTwo {
            get { return testCollectionTwo; }
            set { testCollectionTwo = value; }
        }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        public ICollection<CustomHelperTestClass> TestCollectionEager {
            get { return testCollectionOne; }
            set { testCollectionOne = value; }
        }

        public int DefaultTestIntDefault() {
            return 0;
        }

        [NakedObjectsIgnore]
        public void HiddenAction() {}

        [Disabled]
        public void DisabledAction() {}

        public void NoParameterAction() {}

        public int NoValueParameterFunction() {
            return 0;
        }

        public void OneValueParameterAction(int parm) {}
        public void TwoValueParametersAction(int parm1, int parm2) {}
        public void ThreeValueParametersAction(int parm1, int parm2, int parm3) {}
        public void FourValueParametersAction(int parm1, int parm2, int parm3, int parm4) {}

        public int OneValueParameterFunction(int parm) {
            return 0;
        }

        public int TwoValueParametersFunction(int parm1, int parm2) {
            return 0;
        }

        public int ThreeValueParametersFunction(int parm1, int parm2, int parm3) {
            return 0;
        }

        public int FourValueParametersFunction(int parm1, int parm2, int parm3, int parm4) {
            return 0;
        }

        [NakedObjectsIgnore]
        public void HiddenOneRefParameterAction(CustomHelperTestClass parm) {}

        [Disabled]
        public void DisabledOneRefParameterAction(CustomHelperTestClass parm) {}

        public void OneRefParameterAction(CustomHelperTestClass parm) {}
        public void TwoRefParametersAction(CustomHelperTestClass parm1, CustomHelperTestClass parm2) {}
        public void ThreeRefParametersAction(CustomHelperTestClass parm1, CustomHelperTestClass parm2, CustomHelperTestClass parm3) {}
        public void FourRefParametersAction(CustomHelperTestClass parm1, CustomHelperTestClass parm2, CustomHelperTestClass parm3, CustomHelperTestClass parm4) {}

        public int OneRefParameterFunction(CustomHelperTestClass parm) {
            return 0;
        }

        public int TwoRefParametersFunction(CustomHelperTestClass parm1, CustomHelperTestClass parm2) {
            return 0;
        }

        public int ThreeRefParametersFunction(CustomHelperTestClass parm1, CustomHelperTestClass parm2, CustomHelperTestClass parm3) {
            return 0;
        }

        public int FourRefParametersFunction(CustomHelperTestClass parm1, CustomHelperTestClass parm2, CustomHelperTestClass parm3, CustomHelperTestClass parm4) {
            return 0;
        }

        public void OneCollectionParameterAction(List<CustomHelperTestClass> collection) {}

        public int OneCollectionParameterFunction(List<CustomHelperTestClass> collection) {
            return 0;
        }
    }

    [DisplayName("Test Display Name")]
    [DescribedAs("aDescription")]
    public class DescribedCustomHelperTestClass {
        private ICollection<CustomHelperTestClass> testCollectionOne = new List<CustomHelperTestClass>();
        private ICollection<CustomHelperTestClass> testCollectionTwo = new List<CustomHelperTestClass>();

        [NakedObjectsIgnore, Key]
        public int Id { get; set; }

        [DescribedAs("aDescription")]
        public int TestInt { get; set; }

        public string TestString { get; set; }
        public CustomHelperTestClass TestRef { get; set; }

        [MultiLine(NumberOfLines = 2)]
        public string TestMultiLineString { get; set; }

        // for client validation testing

        [Optionally]
        public string TestOptional { get; set; }

        [Range(0, 10)]
        public int TestRange { get; set; }

        [RegEx(Message = "Test Regex", Validation = ".@.")]
        public int TestRegex { get; set; }

        [StringLength(10)]
        public string TestLength { get; set; }

        public ICollection<CustomHelperTestClass> TestCollectionOne {
            get { return testCollectionOne; }
            set { testCollectionOne = value; }
        }

        public ICollection<CustomHelperTestClass> TestCollectionTwo {
            get { return testCollectionTwo; }
            set { testCollectionTwo = value; }
        }

        public void NoParameterAction() {}
        public void NoValueParameterFunction() {}
        public void OneValueParameterAction([DescribedAs("aDescription")] int parm) {}
        public void TwoValueParametersAction([DescribedAs("aDescription")] int parm1, int parm2) {}
        public void ThreeValueParametersAction([DescribedAs("aDescription")] int parm1, int parm2, int parm3) {}
        public void FourValueParametersAction([DescribedAs("aDescription")] int parm1, int parm2, int parm3, int parm4) {}

        public int OneValueParameterFunction([DescribedAs("aDescription")] int parm) {
            return 0;
        }

        public int TwoValueParametersFunction([DescribedAs("aDescription")] int parm1, int parm2) {
            return 0;
        }

        public int ThreeValueParametersFunction([DescribedAs("aDescription")] int parm1, int parm2, int parm3) {
            return 0;
        }

        public int FourValueParametersFunction([DescribedAs("aDescription")] int parm1, int parm2, int parm3, int parm4) {
            return 0;
        }

        public void TestMultiLineFunction([MultiLine(NumberOfLines = 2)] string parm) {}

        public void TestClientValidationFunction([Optionally] string testOptional,
                                                 [Range(0, 10)] int testRange,
                                                 [RegEx(Message = "Test Regex", Validation = ".@.")] string testRegex,
                                                 [StringLength(10)] string testLength) {}
    }
}