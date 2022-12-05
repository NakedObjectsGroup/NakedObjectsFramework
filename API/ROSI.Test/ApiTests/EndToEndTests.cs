using System;
using NUnit.Framework;
using ROSI.Apis;

namespace ROSI.Test.ApiTests;

internal class EndToEndTests {
    [Test]
    public void TestGetObject() {
        var jo = ObjectApi.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"));
        Assert.IsNotNull(jo);
        Assert.AreEqual("373", jo["instanceId"].ToString());
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

        var ar = action.Invoke(tag, null, new object[] {"U"});

        Assert.IsNotNull(ar);
    }
}