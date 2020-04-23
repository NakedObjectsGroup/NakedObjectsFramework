// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Value;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Value {
    [TestFixture]
    public class FileAttachmentTest {
        [Test]
        public void TestConstructors() {
            var resource = new byte[] { };

            var fa = new FileAttachment(resource);

            Assert.AreEqual(resource, fa.GetResourceAsByteArray());

            fa = new FileAttachment(resource, "MyName");

            Assert.AreEqual(resource, fa.GetResourceAsByteArray());
            Assert.AreEqual("MyName", fa.Name);

            fa = new FileAttachment(resource, "MyName2", "Word Doc");

            Assert.AreEqual(resource, fa.GetResourceAsByteArray());
            Assert.AreEqual("MyName2", fa.Name);
            Assert.AreEqual("Word Doc", fa.MimeType);
        }
    }
}