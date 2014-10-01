// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    [TestFixture]
    public class Reflector_ValidateTest : AbstractDotNetReflectorTest {
        protected override NakedObjectSpecification LoadSpecification(DotNetReflector reflector) {
            return (NakedObjectSpecification) reflector.LoadSpecification(typeof (Product));
        }

        [Test]
        public void TestSetup() {
            Assert.AreEqual("Product", specification.ShortName);
            Assert.AreEqual(3, specification.Properties.Length);
        }

        [Test]
        public void ValidateMethodThatDontMatchAreIgnored() {
            INakedObjectAction[] actions = specification.GetObjectActions();
            Assert.AreEqual(4, actions.Length);
        }

        [Test]
        public void ValidateMethodsDetected() {
            INakedObjectValidation[] validation = specification.ValidateMethods();
            Assert.AreEqual(2, validation.Length);
        }

        [Test]
        public void ValidateMethodsRun() {
            INakedObjectValidation[] validation = specification.ValidateMethods();
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