using System;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Records;

namespace ROSI.Test.ApiTests;

internal class EndToEndTests {
    [Test]
    public void TestGetObject() {
        var jo = ObjectApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"));
        Assert.IsNotNull(jo);
        Assert.AreEqual("373", jo.Wrapped["instanceId"].ToString());
    }

    [Test]
    public void TestInvokeAction() {
        var jo = ObjectApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"));
        Assert.IsNotNull(jo);

        var action = jo.GetAction("OpenPurchaseOrdersForProduct");

        var ar = action.Invoke();

        Assert.IsNotNull(ar);
    }

    [Test]
    public void TestInvokeActionWithValueParams() {
        var (jo, tag) = ObjectApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/426"));
        Assert.IsNotNull(jo);

        var action = jo.GetAction("EditStyle");

        var ar = action.Invoke(new InvokeOptions() {Tag = tag}, "U");

        Assert.IsNotNull(ar);
    }

    [Test]
    public void TestInvokeActionWithRefObjectParams() {
        var (soh, tag) = ObjectApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.SalesOrderHeader/75124"));
        Assert.IsNotNull(soh);

        var p = ObjectApi.GetObject(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/771"));
        Assert.IsNotNull(p);


        var action = soh.GetAction("AddNewDetail");

        var ar = action.Invoke(new InvokeOptions() {Tag = tag}, p, 1);

        Assert.IsNotNull(ar);
    }

    [Test]
    public void TestInvokeActionWithRefLinkParams() {
        var (soh, tag) = ObjectApi.GetObjectWithTag(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.SalesOrderHeader/75124"));
        Assert.IsNotNull(soh);

        var p = ObjectApi.GetObject(new Uri("https://nakedfunctionsdemo.azurewebsites.net/objects/AW.Types.Product/771"));
        Assert.IsNotNull(p);
        var l = p.GetLinks().GetSelfLink();


        var action = soh.GetAction("AddNewDetail");

        var ar = action.Invoke(new InvokeOptions() {Tag = tag}, l, 1);

        Assert.IsNotNull(ar);
    }
}