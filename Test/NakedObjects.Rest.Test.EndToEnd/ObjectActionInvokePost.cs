// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class ObjectActionInvokePost : AbstractActionInvokePost {
    protected override string BaseUrl => Urls.Objects + Urls.WithActionObject1 + Urls.Actions;

    protected override string FilePrefix => "Object-Action-Invoke-Post-";

    [TestMethod]
    public void AnActionReturnsVoidWithParameters() {
        DoAnActionReturnsVoidWithParameters();
    }

    [TestMethod]
    public void SyntacticallyMalformedParameters() {
        DoSyntacticallyMalformedParameters();
    }

    [TestMethod]
    public void SemanticallyMalformedParameters() {
        DoSemanticallyMalformedParameters();
    }

    [TestMethod]
    public void ADisabledAction() {
        DoADisabledAction();
    }

    [TestMethod]
    public void ADisabledCollectionAction() {
        DoADisabledCollectionAction();
    }

    [TestMethod]
    public void AnAction() {
        DoAnAction();
    }

    [TestMethod]
    public void AttemptInvokePostActionWithGet() {
        DoAttemptInvokePostActionWithGet();
    }

    [TestMethod]
    public void AttemptInvokePostActionWithPut() {
        DoAttemptInvokePostActionWithPut();
    }

    [TestMethod]
    public void AttemptInvokePutActionWithPost() {
        DoAttemptInvokePutActionWithPost();
    }

    [TestMethod]
    public void AttemptInvokeGetActionWithPost() {
        DoAttemptInvokeGetActionWithPost();
    }

    [TestMethod]
    public void AnActionReturnsCollection() {
        DoAnActionReturnsCollection();
    }

    [TestMethod]
    public void AnActionReturnsCollectionEmpty() {
        DoAnActionReturnsCollectionEmpty();
    }

    [TestMethod]
    public void AnActionReturnsCollectionNull() {
        DoAnActionReturnsCollectionNull();
    }

    [TestMethod]
    public void AnActionReturnsCollectionWithParameters() {
        DoAnActionReturnsCollectionWithParameters();
    }

    [TestMethod]
    public void AnActionReturnsCollectionWithScalarParameters() {
        DoAnActionReturnsCollectionWithScalarParameters();
    }

    [TestMethod]
    public void AnActionReturnsNull() {
        DoAnActionReturnsNull();
    }

    [TestMethod]
    public void AnActionReturnsObjectWithParameters() {
        DoAnActionReturnsObjectWithParameters();
    }

    [TestMethod]
    public void AnActionReturnsScalar() {
        DoAnActionReturnsScalar();
    }

    [TestMethod] // Must FIX !!
    public void AnActionReturnsScalarEmpty() {
        DoAnActionReturnsScalarEmpty();
    }

    [TestMethod]
    public void AnActionReturnsScalarNull() {
        DoAnActionReturnsScalarNull();
    }

    [TestMethod]
    public void AnActionReturnsScalarWithParameters() {
        DoAnActionReturnsScalarWithParameters();
    }

    [TestMethod]
    public void AnActionReturnsVoid() {
        DoAnActionReturnsVoid();
    }

    [TestMethod]
    public void AnActionWithDateTimeParm() {
        DoAnActionWithDateTimeParm();
    }

    [TestMethod]
    public void AnActionWithOptionalParm() {
        DoAnActionWithOptionalParm();
    }

    [TestMethod]
    public void AnActionWithReferenceParameter() {
        DoAnActionWithReferenceParameter();
    }

    [TestMethod]
    public void AnActionWithValueParameter() {
        DoAnActionWithValueParameter();
    }

    [TestMethod]
    public void AnActionWithValueParameterWithChoices() {
        DoAnActionWithValueParameterWithChoices();
    }

    [TestMethod]
    public void AnError() {
        DoAnError();
    }

    [TestMethod]
    public void AnErrorCollection() {
        DoAnErrorCollection();
    }

    [TestMethod]
    public void ParameterValidateOnlyGood() {
        DoParameterValidateOnlyGood();
    }

    [TestMethod]
    public void ParameterValidateOnlyBad() {
        DoParameterValidateOnlyBad();
    }

    [TestMethod]
    public void AttemptInvalidParameters() {
        DoAttemptInvalidParameters();
    }

    [TestMethod]
    public void AttemptInvalidJson() {
        DoAttemptInvalidJson();
    }

    [TestMethod]
    public void ValidateOnlyParameters() {
        DoValidateOnlyParameters();
    }
}