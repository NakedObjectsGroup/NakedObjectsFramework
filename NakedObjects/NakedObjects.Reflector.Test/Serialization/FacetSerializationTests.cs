// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.Audit;
using NakedFramework.Metamodel.Test.Serialization;
using NakedObjects.Reflector.Facet;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedObjects.Reflector.Test.Serialization;

public static class FacetSerializationTests {
    public static void TestSerializeAuditActionInvocationFacet(Func<AuditActionInvocationFacet, AuditActionInvocationFacet> roundTripper) {
        var m = GetMethod();
        var af = new ActionInvocationFacetViaMethod(m, m.DeclaringType, m.ReturnType, typeof(TestSerializationClass), true, null);

        var f = new AuditActionInvocationFacet(af, new IdentifierImpl(nameof(TestSerializationClass), nameof(TestSerializationClass.InvokeTest)));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.OnType, dsf.OnType);
        Assert.AreEqual(f.ReturnType, dsf.ReturnType);
        Assert.AreEqual(f.ElementType, dsf.ElementType);
        Assert.AreEqual(f.IsQueryOnly, dsf.IsQueryOnly);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeAuditPersistedFacet(Func<AuditPersistedFacet, AuditPersistedFacet> roundTripper) {
        var f = new AuditPersistedFacet(PersistedCallbackFacetNull.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeAuditUpdatedFacet(Func<AuditUpdatedFacet, AuditUpdatedFacet> roundTripper) {
        var f = new AuditUpdatedFacet(UpdatedCallbackFacetNull.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeActionChoicesFacetViaMethod(Func<ActionChoicesFacetViaMethod, ActionChoicesFacetViaMethod> roundTripper) {
        var m = GetChoicesMethod();
        var f = new ActionChoicesFacetViaMethod(m, m.GetParameters().Select(p => (p.Name, p.ParameterType)).ToArray(), null, false);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMultiple, dsf.IsMultiple);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionDefaultsFacetViaMethod(Func<ActionDefaultsFacetViaMethod, ActionDefaultsFacetViaMethod> roundTripper) {
        var f = new ActionDefaultsFacetViaMethod(GetDefaultMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionInvocationFacetViaMethod(Func<ActionInvocationFacetViaMethod, ActionInvocationFacetViaMethod> roundTripper) {
        var m = GetMethod();
        var f = new ActionInvocationFacetViaMethod(m, m.DeclaringType, m.ReturnType, typeof(TestSerializationClass), true, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.OnType, dsf.OnType);
        Assert.AreEqual(f.ReturnType, dsf.ReturnType);
        Assert.AreEqual(f.ElementType, dsf.ElementType);
        Assert.AreEqual(f.IsQueryOnly, dsf.IsQueryOnly);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionParameterValidation(Func<ActionParameterValidation, ActionParameterValidation> roundTripper) {
        var f = new ActionParameterValidation(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionValidationFacet(Func<ActionValidationFacet, ActionValidationFacet> roundTripper) {
        var f = new ActionValidationFacet(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeAutoCompleteFacet(Func<AutoCompleteFacet, AutoCompleteFacet> roundTripper) {
        var f = new AutoCompleteFacet(GetMethod(), 1, 2, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeCreatedCallbackFacetViaMethod(Func<CreatedCallbackFacetViaMethod, CreatedCallbackFacetViaMethod> roundTripper) {
        var f = new CreatedCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeDeletedCallbackFacetNull(Func<DeletedCallbackFacetNull, DeletedCallbackFacetNull> roundTripper) {
        var f = DeletedCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeDeletedCallbackFacetViaMethod(Func<DeletedCallbackFacetViaMethod, DeletedCallbackFacetViaMethod> roundTripper) {
        var f = new DeletedCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeDeletingCallbackFacetNull(Func<DeletingCallbackFacetNull, DeletingCallbackFacetNull> roundTripper) {
        var f = DeletingCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeDeletingCallbackFacetViaMethod(Func<DeletingCallbackFacetViaMethod, DeletingCallbackFacetViaMethod> roundTripper) {
        var f = new DeletingCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }
}