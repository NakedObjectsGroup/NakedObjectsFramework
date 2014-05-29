// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects {
    [TestClass]
    public class ReasonBuilderTest {
        private ReasonBuilder builder;

        [TestInitialize]
        public void SetUp() {
            builder = new ReasonBuilder();
        }

        private void AssertMessageIs(string expected) {
            Assert.AreEqual(expected, builder.Reason);
        }

        [TestMethod]
        public void WithNothingAppendedReasonReturnsNull() {
            Assert.IsNull(builder.Reason);
            builder.Append("Reason 1");
            Assert.IsNotNull(builder.Reason);
            AssertMessageIs("Reason 1");
        }

        [TestMethod]
        public void Append() {
            builder.Append("Reason 1");
            AssertMessageIs("Reason 1");
            builder.Append("Reason 2");
            AssertMessageIs("Reason 1; Reason 2");
            builder.Append("Reason 3");
            AssertMessageIs("Reason 1; Reason 2; Reason 3");
        }

        [TestMethod]
        public void AppendOnCondition() {
            builder.AppendOnCondition(false, "Reason 1");
            Assert.IsNull(builder.Reason);
            builder.AppendOnCondition(true, "Reason 2");
            AssertMessageIs("Reason 2");
            builder.AppendOnCondition(false, "Reason 3");
            AssertMessageIs("Reason 2");
            builder.AppendOnCondition(true, "Reason 4");
            AssertMessageIs("Reason 2; Reason 4");
        }
    }
}