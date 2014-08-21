// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Core.Context;
using NUnit.Framework;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;
using NakedObjects.TestSystem;

namespace NakedObjects.Reflector.spec {
    /// <summary>
    ///     Summary description for OneToManyAssociationTest
    /// </summary>
    [TestFixture]
    public class OneToManyAssociationTest : TestProxyTestCase {
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            nakedObject = system.CreatePersistentTestObject();

            association = new OneToManyAssociationImpl(new DummyOneToManyPeer());
        }

        [TearDown]
        public override void TearDown() {
            base.TearDown();
        }

        private IOneToManyAssociation association;

        private INakedObject nakedObject;

        public class DummyOneToManyPeer : FacetHolderImpl, INakedObjectAssociationPeer {
            private readonly TestProxySpecification specification = new TestProxySpecification(typeof (string));

            #region INakedObjectAssociationPeer Members

            public INakedObjectSpecification Specification {
                get { return specification; }
            }

            public bool IsOneToOne {
                get { throw new NotImplementedException(); }
            }

            public bool IsOneToMany {
                get { throw new NotImplementedException(); }
            }

            #endregion

            public override IIdentifier Identifier {
                get { return new TestProxyIdentifier("Test"); }
            }

            public void AssertActions(int noOfActions) {}
        }


        [Test]
        public void TestCount() {
            Assert.AreEqual(0, association.Count(nakedObject, NakedObjectsContext.ObjectPersistor));
        }
    }
}