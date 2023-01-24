// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class ErrorApiTests : AbstractApiTests {
    [Test]
    public void TestGetStatusCode() {
        try {
            var objectRep = GetObject(FullName<ClassWithActions>(), "100");
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpRosiException hre) {
                Assert.AreEqual(HttpStatusCode.NotFound, hre.StatusCode);
            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }
    }

    [Test]
    public void TestErrorException() {
        try {
            var objectRep = GetObject(FullName<ClassWithActions>(), "1");
            var sink = objectRep.GetAction(nameof(ClassWithActions.ActionThrowsException)).Invoke(TestInvokeOptions()).Result;
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpErrorRosiException hre) {
                Assert.AreEqual(HttpStatusCode.InternalServerError, hre.StatusCode);
                Assert.AreEqual("Exception 1", hre.Error?.GetMessage());

                var st = hre.Error?.GetStackTrace();
                Assert.IsNotNull(st);
                Assert.AreEqual(1, st.Count());
                Assert.IsTrue(st.First().StartsWith("   at ROSI.Test.Data.ClassWithActions.ActionThrowsException() in C:\\GitHub\\NakedObjectsFramework\\API\\ROSI.Test.Data\\TestClasses.cs"));
            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }
    }
}