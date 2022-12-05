using System;
using System.Net.Http;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Test.ApiTests;

internal class EndToEndTests {
    [SetUp]
    public void SetUp() {
        HttpHelpers.Client = new HttpClient();
    }

    [Test]
    public void TestGetObject() {
        var jo = DomainObjectApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"));
        Assert.IsNotNull(jo);
        Assert.AreEqual("373", jo.Wrapped["instanceId"].ToString());
    }

    [Test]
    public void TestInvokeActionReturnsCollection() {
        var jo = DomainObjectApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"));
        Assert.IsNotNull(jo);

        var action = jo.GetAction("OpenPurchaseOrdersForProduct");

        var ar = action.Invoke();

        Assert.AreEqual(ActionResultApi.ResultType.list, ar.GetResultType());
    }

    [Test]
    public void TestInvokeActionWithValueParams() {
        var (jo, tag) = DomainObjectApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/426"));
        Assert.IsNotNull(jo);

        var action = jo.GetAction("EditStyle");

        var ar = action.Invoke(new InvokeOptions { Tag = tag }, "U");

        Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());
    }

    [Test]
    public void TestInvokeActionWithRefObjectParams() {
        var (soh, tag) = DomainObjectApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.SalesOrderHeader/75124"));
        Assert.IsNotNull(soh);

        var p = DomainObjectApi.GetObject(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/771"));
        Assert.IsNotNull(p);

        var action = soh.GetAction("AddNewDetail");

        var ar = action.Invoke(new InvokeOptions { Tag = tag }, p, 1);

        Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());
    }

    [Test]
    public void TestInvokeActionWithRefLinkParams() {
        var (soh, tag) = DomainObjectApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.SalesOrderHeader/75124"));
        Assert.IsNotNull(soh);

        var p = DomainObjectApi.GetObject(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/771"));
        Assert.IsNotNull(p);
        var l = p.GetLinks().GetSelfLink();

        var action = soh.GetAction("AddNewDetail");

        var ar = action.Invoke(new InvokeOptions { Tag = tag }, l, 1);

        Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());
    }
}