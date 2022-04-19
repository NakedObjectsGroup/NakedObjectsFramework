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
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Profile;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.Test.Serialization;
using NakedFramework.Profile;
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
        Assert.AreEqual(f.ParameterNamesAndTypes[0], dsf.ParameterNamesAndTypes[0]);
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

    public static void TestSerializeDisableForContextFacet(Func<DisableForContextFacet, DisableForContextFacet> roundTripper) {
        var f = new DisableForContextFacet(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeHideForContextFacet(Func<HideForContextFacet, HideForContextFacet> roundTripper) {
        var f = new HideForContextFacet(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeLoadedCallbackFacetNull(Func<LoadedCallbackFacetNull, LoadedCallbackFacetNull> roundTripper) {
        var f = LoadedCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeLoadedCallbackFacetViaMethod(Func<LoadedCallbackFacetViaMethod, LoadedCallbackFacetViaMethod> roundTripper) {
        var f = new LoadedCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeLoadingCallbackFacetNull(Func<LoadingCallbackFacetNull, LoadingCallbackFacetNull> roundTripper) {
        var f = LoadingCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeLoadingCallbackFacetViaMethod(Func<LoadingCallbackFacetViaMethod, LoadingCallbackFacetViaMethod> roundTripper) {
        var f = new LoadingCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeOnPersistingErrorCallbackFacetViaMethod(Func<OnPersistingErrorCallbackFacetViaMethod, OnPersistingErrorCallbackFacetViaMethod> roundTripper) {
        var f = new OnPersistingErrorCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeOnUpdatingErrorCallbackFacetViaMethod(Func<OnUpdatingErrorCallbackFacetViaMethod, OnUpdatingErrorCallbackFacetViaMethod> roundTripper) {
        var f = new OnUpdatingErrorCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePersistedCallbackFacetNull(Func<PersistedCallbackFacetNull, PersistedCallbackFacetNull> roundTripper) {
        var f = PersistedCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializePersistedCallbackFacetViaMethod(Func<PersistedCallbackFacetViaMethod, PersistedCallbackFacetViaMethod> roundTripper) {
        var f = new PersistedCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePersistingCallbackFacetNull(Func<PersistingCallbackFacetNull, PersistingCallbackFacetNull> roundTripper) {
        var f = PersistingCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializePersistingCallbackFacetViaMethod(Func<PersistingCallbackFacetViaMethod, PersistingCallbackFacetViaMethod> roundTripper) {
        var f = new PersistingCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyAccessorFacetViaContributedAction(Func<PropertyAccessorFacetViaContributedAction, PropertyAccessorFacetViaContributedAction> roundTripper) {
        var f = new PropertyAccessorFacetViaContributedAction(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyAccessorFacetViaMethod(Func<PropertyAccessorFacetViaMethod, PropertyAccessorFacetViaMethod> roundTripper) {
        var f = new PropertyAccessorFacetViaMethod(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyChoicesFacet(Func<PropertyChoicesFacet, PropertyChoicesFacet> roundTripper) {
        var f = new PropertyChoicesFacet(GetMethod(), new[] { ("name", typeof(TestSerializationClass)) }, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.ParameterNamesAndTypes[0], dsf.ParameterNamesAndTypes[0]);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyDefaultFacetViaMethod(Func<PropertyDefaultFacetViaMethod, PropertyDefaultFacetViaMethod> roundTripper) {
        var f = new PropertyDefaultFacetViaMethod(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertySetterFacetViaModifyMethod(Func<PropertySetterFacetViaModifyMethod, PropertySetterFacetViaModifyMethod> roundTripper) {
        var f = new PropertySetterFacetViaModifyMethod(GetCallbackMethod(), "name", null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.PropertyName, dsf.PropertyName);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyValidateFacetViaMethod(Func<PropertyValidateFacetViaMethod, PropertyValidateFacetViaMethod> roundTripper) {
        var f = new PropertyValidateFacetViaMethod(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeTitleFacetViaProperty(Func<TitleFacetViaProperty, TitleFacetViaProperty> roundTripper) {
        var f = new TitleFacetViaProperty(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeTitleFacetViaTitleMethod(Func<TitleFacetViaTitleMethod, TitleFacetViaTitleMethod> roundTripper) {
        var f = new TitleFacetViaTitleMethod(GetMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeUpdatedCallbackFacetNull(Func<UpdatedCallbackFacetNull, UpdatedCallbackFacetNull> roundTripper) {
        var f = UpdatedCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeUpdatedCallbackFacetViaMethod(Func<UpdatedCallbackFacetViaMethod, UpdatedCallbackFacetViaMethod> roundTripper) {
        var f = new UpdatedCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeUpdatingCallbackFacetNull(Func<UpdatingCallbackFacetNull, UpdatingCallbackFacetNull> roundTripper) {
        var f = UpdatingCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeUpdatingCallbackFacetViaMethod(Func<UpdatingCallbackFacetViaMethod, UpdatingCallbackFacetViaMethod> roundTripper) {
        var f = new UpdatingCallbackFacetViaMethod(GetCallbackMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeViewModelEditFacetConvention(Func<ViewModelEditFacetConvention, ViewModelEditFacetConvention> roundTripper) {
        var f = ViewModelEditFacetConvention.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeViewModelFacetConvention(Func<ViewModelFacetConvention, ViewModelFacetConvention> roundTripper) {
        var f = ViewModelFacetConvention.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeViewModelSwitchableFacetConvention(Func<ViewModelSwitchableFacetConvention, ViewModelSwitchableFacetConvention> roundTripper) {
        var f = ViewModelSwitchableFacetConvention.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeProfileActionInvocationFacet(Func<ProfileActionInvocationFacet, ProfileActionInvocationFacet> roundTripper) {
        var m = GetMethod();
        var af = new ActionInvocationFacetViaMethod(m, m.DeclaringType, m.ReturnType, typeof(TestSerializationClass), true, null);

        var f = new ProfileActionInvocationFacet(af, new IdentifierImpl(nameof(TestSerializationClass), nameof(TestSerializationClass.InvokeTest)));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.OnType, dsf.OnType);
        Assert.AreEqual(f.ReturnType, dsf.ReturnType);
        Assert.AreEqual(f.ElementType, dsf.ElementType);
        Assert.AreEqual(f.IsQueryOnly, dsf.IsQueryOnly);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeProfileCallbackFacet(Func<ProfileCallbackFacet, ProfileCallbackFacet> roundTripper) {
        var m = GetCallbackMethod();
        var af = new UpdatedCallbackFacetViaMethod(m, null);

        var f = new ProfileCallbackFacet(ProfileEvent.Updated, af);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeProfilePropertySetterFacet(Func<ProfilePropertySetterFacet, ProfilePropertySetterFacet> roundTripper) {
        var p = GetProperty();
        var ps = new PropertySetterFacetViaSetterMethod(p, null);

        var f = new ProfilePropertySetterFacet(ps);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.PropertyName, dsf.PropertyName);
    }
}