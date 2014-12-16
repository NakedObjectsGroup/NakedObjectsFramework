// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Reflect.Test {
    [TestClass]
    public class ReflectorValidateTest : AbstractReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return reflector.LoadSpecification(typeof (Product));
        }

        [TestMethod]
        public void TestSetup() {
            Assert.AreEqual("Product", Specification.ShortName);
            Assert.AreEqual(3, Specification.Fields.Count);
        }

        [TestMethod, Ignore] // fix with new validation factory
        public void ValidateMethodThatDontMatchAreIgnored() {
            var actions = Specification.ObjectActions;
            Assert.AreEqual(4, actions.Count);
        }

        [TestMethod, Ignore] // fix with new validation factory
        public void ValidateMethodsDetected() {
            //var validation = Specification.ValidationMethods;
            //Assert.AreEqual(2, validation.Length);
        }

        [TestMethod, Ignore] // fix with new validation factory
        public void ValidateMethodsRun() {
            //INakedObjectValidation[] validation = Specification.ValidationMethods;
        }
    }

    public class Product {
        // has the wrong number of parameters
        public string Name { get; set; }

        public DateTime When { get; set; }

        public int Count { get; set; }

        public string Validate(string name) {
            return null;
        }

        // has the wrong return type
        public int Validate(string name, string count) {
            return 0;
        }

        // has the wrong name
        public string ValidateObject(string name, int count) {
            return null;
        }

        // has unmatched parameter name
        public string Validate(DateTime date, int count) {
            return null;
        }

        public string Validate(string name, int count) {
            if (name == null) {
                return "no name";
            }
            if (name.Length != count)
                return "invalid length";

            return null;
        }

        public string Validate(string name, DateTime when) {
            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}