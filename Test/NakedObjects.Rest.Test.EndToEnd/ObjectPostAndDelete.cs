// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class ObjectPostAndDelete : AbstractActionInvokePost {
    #region Helpers

    protected override string BaseUrl => Urls.Objects;

    #endregion

    [TestMethod]
    public void AttemptPost() {
        Helpers.Helpers.TestResponse($@"{BaseUrl}RestfulObjects.Test.Data.MostSimple/1", null, JsonRep.Empty(), Methods.Post, Codes.MethodNotValid);
    }

    [TestMethod]
    public void AttemptDelete() {
        Helpers.Helpers.TestResponse($@"{BaseUrl}RestfulObjects.Test.Data.MostSimple/1", null, null, Methods.Delete, Codes.MethodNotValid);
    }
}