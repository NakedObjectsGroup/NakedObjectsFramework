// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class ViewModelGet : ObjectAbstract {
    [TestMethod]
    public void MostSimpleViewModel() {
        Object(Urls.VmMostSimple + Key1, "MostSimpleViewModel");
    }

    [TestMethod]
    public void WithFormalDomainModel() {
        Object(Urls.VmMostSimple + Key1 + JsonRep.FormalDomainModeAsQueryString, "WithFormalDomainModel");
    }

    [TestMethod]
    public void WithSimpleDomainModel() {
        Object(Urls.VmMostSimple + Key1 + JsonRep.SimpleDomainModeAsQueryString, "WithSimpleDomainModel");
    }

    [TestMethod] //http://restfulobjects.codeplex.com/workitem/26
    public void AttemptWithMalformedDomainModel() {
        var url = Urls.Objects + Urls.VmMostSimple + Key1 + JsonRep.DomainModeQueryStringMalformed;
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    [TestMethod]
    public void WithGenericAcceptHeader() {
        Object(Urls.VmMostSimple + Key1, "MostSimpleViewModel", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
    }

    [TestMethod]
    public void WithProfileAcceptHeader() {
        Object(Urls.VmMostSimple + Key1, "MostSimpleViewModel", null, Methods.Get, Codes.Succeeded, MediaTypes.ObjectProfile);
    }

    [TestMethod]
    public void AttemptWithInvalidProfileAcceptHeader() {
        Object(Urls.VmMostSimple + Key1, null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.Homepage);
    }

    [TestMethod]
    public void AttemptMostSimpleInvalidKey() {
        Object($"{Urls.VmMostSimple}notAnInteger", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void WithValue() {
        var ticks = new DateTime(2012, 1, 1).Ticks;
        var tsticks = new TimeSpan(1, 2, 3, 4, 5).Ticks;
        Object($"{Urls.VmWithValue}1--2--3--4--5--foo--{ticks}--{tsticks}--6--7", "WithValue");
    }

    [TestMethod]
    public void AttemptWithValueInvalidKey1() {
        var ticks = new DateTime(2012, 1, 1).Ticks;
        Object($"{Urls.VmWithValue}1--2--3--4--5--foo--{ticks}--6--bar", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void AttemptWithValueInvalidKey2() {
        var ticks = new DateTime(2012, 1, 1).Ticks;
        Object($"{Urls.VmWithValue}1--2--3--4--5", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void WithReference() {
        Object($"{Urls.VmWithReference}1--2--2--3", "WithReference");
    }

    [TestMethod]
    public void WithReferenceFollowingAPropertyLink() {
        Object($"{Urls.VmWithReference}1--2--2--3/properties/AReference", "WithReference-Property");
    }

    [TestMethod]
    public void AttemptWithReferenceInvalidKey1() {
        Object($"{Urls.VmWithReference}1--2--2", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void AttemptWithReferenceInvalidKey2() {
        Object($"{Urls.VmWithReference}1--2--2--foo", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void WithCollection() {
        Object($"{Urls.VmWithCollection}1--3", "WithCollection");
    }

    [TestMethod]
    public void WithCollectionFollowingACollectionLink() {
        Object($"{Urls.VmWithCollection}1--3/collections/ACollection", "WithCollection-ACollection");
    }

    [TestMethod]
    public void AttemptWithCollectionInvalidKey1() {
        Object($"{Urls.VmWithCollection}1", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void AttemptWithCollectionInvalidKey2() {
        Object($"{Urls.VmWithCollection}1-foo", null, null, Methods.Get, Codes.NotFound);
    }

    //TODO:  Need tests for accessing the collections on the returned VM

    [TestMethod]
    // fails after contributed action change
    public void WithAction() {
        Object(Urls.VmWithAction + 1, "WithAction");
    }

    [TestMethod]
    public void WithActionFollowingActionDetails() {
        Object($"{Urls.VmWithAction}{1}/actions/AnActionReturnsNull", "WithAction-ActionDetails");
    }

    //AnActionReturnsNull

    [TestMethod]
    public void AttemptWithActionInvalidKey1() {
        Object($"{Urls.VmWithAction}foo", null, null, Methods.Get, Codes.NotFound);
    }

    #region Helpers

    protected override string FilePrefix => "ViewModel-Get-";

    private const string Key1 = "31459"; //An arbitrarily large Id

    #endregion
}