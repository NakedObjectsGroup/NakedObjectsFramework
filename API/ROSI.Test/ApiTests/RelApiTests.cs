// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NUnit.Framework;
using ROSI.Apis;

namespace ROSI.Test.ApiTests;

public class RelApiTests : AbstractRosiApiTests {
    [Test]
    public void TestGetRelType() {
        var testRel1 = @"urn:org.restfulobjects:rels/invoke;action=""CreateNewWorkOrder""";
        var testRel2 = @"urn:org.restfulobjects:rels/domain-type;action=""CreateNewWorkOrder""";

        var relType1 = testRel1.GetRelType();

        Assert.AreEqual(RelApi.Rels.invoke, relType1);

        var relType2 = testRel2.GetRelType();

        Assert.AreEqual(RelApi.Rels.domain_type, relType2);
    }
}