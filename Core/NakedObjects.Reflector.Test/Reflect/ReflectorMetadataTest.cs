// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects.Architecture.SpecImmutable;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test {
    [TestFixture]
    public class ReflectorMetaDataTest : AbstractReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return reflector.LoadSpecification(typeof (TestDomainObject2));
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

    public class TestDomainObject {}

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