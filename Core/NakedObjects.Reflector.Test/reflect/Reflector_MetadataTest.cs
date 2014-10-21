// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Spec;
using NakedObjects.Test;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    [TestFixture]
    public class Reflector_MetaDataTest : AbstractDotNetReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return  reflector.LoadSpecification(typeof (TestDomainObject2));
        }


        [Test]
        public void TestAnnotationPickUpFromBuddyClass() {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof (TestDomainObject2));
            Assert.That(properties.Find("Count", true).Attributes.OfType<TitleAttribute>().Count(), Is.EqualTo(1));
        }

        [Test]
        public void TestNonBuddiedMehtodHasNoAnnotation() {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof (TestDomainObject2));
            Assert.That(properties.Find("Name", true).Attributes.OfType<TitleAttribute>().Count(), Is.EqualTo(0));
        }

        [Test]
        public void TestPropertyCountReflectsBothClasses() {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof (TestDomainObject2));
            Assert.That(properties.Count, Is.EqualTo(2));
        }
    }

    [MetadataType(typeof (TestDomainObject2_Metadata))]
    public class TestDomainObject2 : TestDomainObject {
        public int Count { get; set; }

        public string Name { get; set; }
    }

    internal class TestDomainObject2_Metadata {
        [Title]
        public int Count { get; set; }
    }


    // Copyright (c) Naked Objects Group Ltd.
}