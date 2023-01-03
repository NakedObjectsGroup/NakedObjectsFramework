using System;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Records;

namespace ROSI.Test.ApiTests;

internal class EndToEndTests {
    [SetUp]
    public void SetUp() { }

    [Test]
    public void TestGetObject() {
        var jo = ROSIApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"), new InvokeOptions()).Result;
        Assert.IsNotNull(jo);
        Assert.AreEqual("373", jo.Wrapped["instanceId"].ToString());
    }

    [Test]
    public void TestInvokeActionReturnsCollection() {
        var jo = ROSIApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"), new InvokeOptions()).Result;
        Assert.IsNotNull(jo);

        var action = jo.GetAction("OpenPurchaseOrdersForProduct");

        var ar = action.Invoke(new InvokeOptions()).Result;

        Assert.AreEqual(ActionResultApi.ResultType.list, ar.GetResultType());
    }

    [Test]
    public void TestInvokeActionWithValueParams() {
        var (jo, tag) = ROSIApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/426"), new InvokeOptions()).Result;
        Assert.IsNotNull(jo);

        var action = jo.GetAction("EditStyle");

        var ar = action.Invoke(new InvokeOptions { Tag = tag }, "U").Result;

        Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());
    }

    //[Test]
    //public void TestInvokeActionWithRefObjectParams() {
    //    var (soh, tag) = ROSIApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.SalesOrderHeader/75124"), new InvokeOptions()).Result;
    //    Assert.IsNotNull(soh);

    //    var p = ROSIApi.GetObject(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/771"), new InvokeOptions()).Result;
    //    Assert.IsNotNull(p);

    //    var action = soh.GetAction("AddNewDetail");

    //    var ar = action.Invoke(new InvokeOptions { Tag = tag }, p, 1).Result;

    //    Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());
    //}

    //[Test]
    //public void TestInvokeActionWithRefLinkParams() {
    //    var (soh, tag) = ROSIApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.SalesOrderHeader/75124"), new InvokeOptions()).Result;
    //    Assert.IsNotNull(soh);

    //    var p = ROSIApi.GetObject(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/771"), new InvokeOptions()).Result;
    //    Assert.IsNotNull(p);
    //    var l = p.GetLinks().GetSelfLink();

    //    var action = soh.GetAction("AddNewDetail");

    //    var ar = action.Invoke(new InvokeOptions { Tag = tag }, l, 1).Result;

    //    Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());
    //}
}