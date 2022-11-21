using System;
using NUnit.Framework;

namespace ROSI.Test;

internal class EndToEndTests {

    [Test]
    public void GetObject() {

        var jo = API.GetObject(new Uri("https://nakedobjectsrodemo.azurewebsites.net/objects/AdventureWorksModel.Product/373"));
        Assert.IsNotNull(jo);
        Assert.AreEqual("373", jo["instanceId"].ToString());
    }
}