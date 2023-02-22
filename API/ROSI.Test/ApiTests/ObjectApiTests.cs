// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Records;
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class ObjectApiTests : AbstractRosiApiTests {
    [Test]
    public void TestObject() {
        var objectRep = GetObject(FullName<Class>(), "1");
        Assert.AreEqual(FullName<Class>(), objectRep.GetDomainType());
        Assert.AreEqual("1", objectRep.GetInstanceId());
        Assert.AreEqual("Class:1", objectRep.GetTitle());
    }

    [Test]
    public void TestGetCollections() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetCollections();

        Assert.AreEqual(2, val.Count());
    }

    [Test]
    public void TestGetActions() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetActions();

        Assert.AreEqual(2, val.Count());
    }

    [Test]
    public void TestGetProperties() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetProperties();

        Assert.AreEqual(8, val.Count());
    }

    [Test]
    public void TestGetProperty() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetProperty("Property1");

        Assert.IsNotNull(val);
    }

    [Test]
    public void TestGetAction() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetAction("Action1");

        Assert.IsNotNull(val);
    }

    [Test]
    public void TestGetCollection() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetCollection("Collection1");

        Assert.IsNotNull(val);
    }

    [Test]
    public void TestGetAndSaveTransient() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        var service = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        var transient = service.GetAction(nameof(SimpleService.GetTransient)).Invoke(TestInvokeOptions()).Result.GetObject();

        var uniqueName = Guid.NewGuid().ToString();
        var persisted = transient.Persist(TestInvokeOptions(null, transient.Tag), 0, uniqueName, null).Result;

        Assert.AreEqual(uniqueName, persisted.GetProperty("Name")?.GetValue<string>());
    }

    [Test]
    public void TestGetAndSaveTransientWithNamedParams()
    {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        var service = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        var transient = service.GetAction(nameof(SimpleService.GetTransient)).Invoke(TestInvokeOptions()).Result.GetObject();

        var uniqueName = Guid.NewGuid().ToString();
        var persisted = transient.PersistWithNamedParams(TestInvokeOptions(null, transient.Tag), new() { { nameof(ClassToPersist.Id), 0 }, { nameof(ClassToPersist.Name), uniqueName }, { nameof(ClassToPersist.RefClassToPersist), null } }).Result;

        Assert.AreEqual(uniqueName, persisted.GetProperty("Name")?.GetValue<string>());
    }

    [Test]
    public void TestGetAndValidateTransient() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        var service = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        var transient = service.GetAction(nameof(SimpleService.GetTransient)).Invoke(TestInvokeOptions()).Result.GetObject();

        var uniqueName = Guid.NewGuid().ToString();
        transient.ValidatePersist(TestInvokeOptions(null, transient.Tag), 0, uniqueName, null).Wait();

       
    }

    [Test]
    public void TestGetAndValidateTransientWithNamedParams()
    {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        var service = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        var transient = service.GetAction(nameof(SimpleService.GetTransient)).Invoke(TestInvokeOptions()).Result.GetObject();

        var uniqueName = Guid.NewGuid().ToString();
        transient.ValidatePersistWithNamedParams(TestInvokeOptions(null, transient.Tag), new() { { nameof(ClassToPersist.Id), 0 }, { nameof(ClassToPersist.Name), uniqueName }, { nameof(ClassToPersist.RefClassToPersist), null } }).Wait();

    }

    [Test]
    public void TestGetAndValidateFailTransient() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        var service = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        var transient = service.GetAction(nameof(SimpleService.GetTransient)).Invoke(TestInvokeOptions()).Result.GetObject();

        var uniqueName = Guid.NewGuid().ToString();

        try {
            transient.ValidatePersist(TestInvokeOptions(null, transient.Tag), null, uniqueName, null ).Wait();
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.IsNotNull(hre.Content);

                Assert.AreEqual(FullName<ClassToPersist>(), hre.Content.GetDomainType());

                var args = hre.Content.GetMembers();
                
                Assert.AreEqual(3, args.Count);
                Assert.AreEqual(nameof(ClassToPersist.Id), args.First().Key);
                Assert.IsNull(args.First().Value.GetValue());
                Assert.AreEqual("Mandatory", args.First().Value.GetInvalidReason());

                Assert.AreEqual(nameof(ClassToPersist.Name), args.Skip(1).First().Key);
                Assert.AreEqual(nameof(ClassToPersist.RefClassToPersist), args.Last().Key);

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
    }

    [Test]
    public void TestGetAndSaveTransientWithHelper()
    {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        var service = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        var uniqueName = Guid.NewGuid().ToString();

        var transient = service.GetAction(nameof(SimpleService.GetPrePopulatedTransient)).Invoke(TestInvokeOptions(), uniqueName).Result.GetObject();
        var map = transient.GetPropertyMap();
        map["Id"] = 0;

        var persisted = transient.PersistWithNamedParams(TestInvokeOptions(null, transient.Tag), map).Result;

        Assert.AreEqual(uniqueName, persisted.GetProperty("Name")?.GetValue<string>());
    }

    [Test]
    public void TestUpdate() {
        var objectRep = GetObject(FullName<ClassToPersist>(), "1");

        var uniqueName = Guid.NewGuid().ToString();
        var updated = objectRep.Update(TestInvokeOptions(), 1, uniqueName, null).Result;

        Assert.AreEqual(uniqueName, updated.GetProperty("Name")?.GetValue<string>());
    }

    [Test]
    public void TestUpdateWithNamedParameters() {
        var objectRep = GetObject(FullName<ClassToPersist>(), "1");

        var uniqueName = Guid.NewGuid().ToString();
        var updated = objectRep.UpdateWithNamedParams(TestInvokeOptions(), new () { { nameof(ClassToPersist.Name), uniqueName } }).Result;

        Assert.AreEqual(uniqueName, updated.GetProperty("Name")?.GetValue<string>());
    }

    [Test]
    public void TestIsSubtypeOf() {
        var objectRep = GetObject(FullName<ClassToPersist>(), "1");
        var tar = objectRep.IsSubtypeOf(TestInvokeOptions(), FullName<ClassToPersist>()).Result;
        Assert.IsTrue(tar.GetValue());
        Assert.AreEqual("isSubtypeOf", tar.GetId());
    }

    [Test]
    public void TestIsSupertypeOf() {
        var objectRep = GetObject(FullName<ClassToPersist>(), "1");
        var tar = objectRep.IsSupertypeOf(TestInvokeOptions(), FullName<ClassToPersist>()).Result;
        Assert.IsTrue(tar.GetValue());
        Assert.AreEqual("isSupertypeOf", tar.GetId());
    }

    [Test]
    public void TestMissingProperties() {
        var objectRep = GetObject(FullName<Class>(), "1");

        Assert.IsNull(objectRep.GetServiceId());

        var empty = new DomainObject(new JObject());

        try {
            var t = empty.GetTitle();
            Assert.Fail("expect exception");
        }
        catch (NoSuchPropertyRosiException e) {
            Assert.AreEqual("No property: title in: ROSI.Records.DomainObject", e.Message);
        }
    }

    [Test]
    public void TestObjectError() {
        try {
            var objectRep = ROSIApi.GetObject(new Uri($"http://localhost/objects/{FullName<Class>()}/100"), TestInvokeOptions()).Result;
            Assert.Fail("expect exception");
        }
        catch (AggregateException e) {
            foreach (var exception in e.InnerExceptions) {
                if (exception is HttpRequestException he) {
                    Assert.AreEqual(HttpStatusCode.NotFound, he.StatusCode);
                }
            }
        }
    }

    [Test]
    public void TestGetAsPoco() {
        var objectRep = GetObject(FullName<ClassWithScalars>(), "1");
        var poco = objectRep.GetAsPoco<TestClassPoco>();

        Assert.AreEqual(true, poco.Bool1);
        Assert.AreEqual(true, poco.Bool2);
        Assert.AreEqual(null, poco.Bool3);

        Assert.AreEqual(1, poco.Short1);
        Assert.AreEqual(2, poco.Short2);
        Assert.AreEqual(null, poco.Short3);

        Assert.AreEqual(3, poco.Int1);
        Assert.AreEqual(4, poco.Int2);
        Assert.AreEqual(null, poco.Int3);

        Assert.AreEqual(5, poco.Long1);
        Assert.AreEqual(6, poco.Long2);
        Assert.AreEqual(null, poco.Long3);

        Assert.AreEqual(7.1, poco.Double1);
        Assert.AreEqual(8.2, poco.Double2);
        Assert.AreEqual(null, poco.Double3);

        Assert.AreEqual(9.1, poco.Decimal1);
        Assert.AreEqual(10.2, poco.Decimal2);
        Assert.AreEqual(null, poco.Decimal3);

        Assert.AreEqual(new DateTime(2023, 01, 11), poco.DateTime1);
        Assert.AreEqual(new DateTime(2024, 02, 12), poco.DateTime2);
        Assert.AreEqual(null, poco.DateTime3);

        Assert.AreEqual(new DateTime(2023, 01, 11, 11, 54, 00), poco.DateTime4);
        Assert.AreEqual(new DateTime(2024, 02, 12, 13, 54, 00), poco.DateTime5);
        Assert.AreEqual(null, poco.DateTime6);

        Assert.AreEqual(new TimeSpan(0, 1, 2, 3), poco.TimeSpan1);
        Assert.AreEqual(new TimeSpan(0, 23, 59, 59), poco.TimeSpan2);
        Assert.AreEqual(null, poco.TimeSpan3);

        Assert.AreEqual("A String", poco.String);
    }

    [Test]
    public void TestGetAsPocoError1() {
        var objectRep = GetObject(FullName<ClassWithScalars>(), "1");

        try {
            var poco = objectRep.GetAsPoco<TestClassPocoError1>();
            Assert.Fail("expect exception");
        }
        catch (Exception e) {
            Assert.AreEqual("Invalid cast from 'Boolean' to 'DateTime'.", e.Message);
        }
    }

    [Test]
    public void TestGetAsPocoError2() {
        var objectRep = GetObject(FullName<ClassWithScalars>(), "1");
        var poco = objectRep.GetAsPoco<TestClassPocoError2>();
        Assert.AreEqual(false, poco.Bool3);
    }

    [Test]
    public void TestGetAsPocoError3() {
        var objectRep = GetObject(FullName<ClassWithScalars>(), "1");

        try {
            var poco = objectRep.GetAsPoco<TestClassPocoError3>();
            Assert.Fail("expect exception");
        }
        catch (Exception e) {
            Assert.AreEqual("Unrecognized generic property type: System.Collections.Generic.ICollection`1[System.Boolean]", e.Message);
        }
    }

    private class TestClassPoco {
        public bool Bool1 { get; set; }
        public bool? Bool2 { get; set; }
        public bool? Bool3 { get; set; }

        public short Short1 { get; set; }
        public short? Short2 { get; set; }
        public short? Short3 { get; set; }

        public int Int1 { get; set; }
        public int? Int2 { get; set; }
        public int? Int3 { get; set; }

        public long Long1 { get; set; }
        public long? Long2 { get; set; }
        public long? Long3 { get; set; }

        public double Double1 { get; set; }
        public double? Double2 { get; set; }
        public double? Double3 { get; set; }

        public decimal Decimal1 { get; set; }
        public decimal? Decimal2 { get; set; }
        public decimal? Decimal3 { get; set; }

        public DateTime DateTime1 { get; set; }
        public DateTime? DateTime2 { get; set; }
        public DateTime? DateTime3 { get; set; }

        public DateTime DateTime4 { get; set; }
        public DateTime? DateTime5 { get; set; }
        public DateTime? DateTime6 { get; set; }

        public TimeSpan TimeSpan1 { get; set; }
        public TimeSpan? TimeSpan2 { get; set; }
        public TimeSpan? TimeSpan3 { get; set; }

        public string String { get; set; }
    }

    private class TestClassPocoError1 {
        public DateTime Bool1 { get; set; }
    }

    private class TestClassPocoError2 {
        public bool Bool3 { get; set; }
    }

    private class TestClassPocoError3 {
        public ICollection<bool> Bool1 { get; set; }
    }
}