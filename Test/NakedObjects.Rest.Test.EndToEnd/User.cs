// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class User : AbstractSecondaryResource {
    protected override string Filename() => "User";

    protected override string ResourceUrl() => Urls.User;

    protected override string ProfileType() => MediaTypes.User;

    [TestMethod]
    public override void GetResource() {
        DoGetResource();
    }

    [TestMethod]
    public override void WithGenericAcceptHeader() {
        DoWithGenericAcceptHeader();
    }

    [TestMethod]
    public override void WithProfileAcceptHeader() {
        DoWithProfileAcceptHeader();
    }

    [TestMethod]
    public override void AttemptWithInvalidProfileAcceptHeader() {
        DoAttemptWithInvalidProfileAcceptHeader();
    }

    [TestMethod]
    public override void AttemptPost() {
        DoAttemptPost();
    }

    [TestMethod]
    public override void AttemptPut() {
        DoAttemptPut();
    }

    [TestMethod]
    public override void AttemptDelete() {
        DoAttemptDelete();
    }

    [TestMethod]
    public override void WithFormalDomainModel() {
        DoWithFormalDomainModel();
    }

    [TestMethod]
    public override void WithSimpleDomainModel() {
        DoWithSimpleDomainModel();
    }

    [TestMethod]
    public override void AttemptWithDomainModelMalformed() {
        DoAttemptWithDomainModelMalformed();
    }
}