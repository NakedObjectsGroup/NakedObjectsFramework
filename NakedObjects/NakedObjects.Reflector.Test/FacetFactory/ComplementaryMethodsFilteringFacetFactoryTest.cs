// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class ComplementaryMethodsFilteringFacetFactoryTest : AbstractFacetFactoryTest {
        private ComplementaryMethodsFilteringFacetFactory facetFactory;

        protected override Type[] SupportedTypes => Array.Empty<Type>();

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestFiltersAddTo() {
            var actionMethod = FindMethodIgnoreParms(typeof(CollectionClass), "AddToACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersChoices() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "ChoicesAProperty");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersDefault() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "DefaultAProperty");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersDisable() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "DisableAProperty");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersDisableAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "DisableAnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersHide() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "HideAProperty");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersHideAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "HideAnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersModify() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "ModifyAProperty");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersParameterChoices() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "ChoicesAnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersParameterDefault() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "DefaultAnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersParameterIndexChoices() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "Choices0AnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersParameterIndexDefault() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "Default0AnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersParameterIndexValidate() {
            var actionMethod = FindMethodIgnoreParms(typeof(ActionClass), "Validate0AnAction");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersParameterValidate() {
            var actionMethod = FindMethod(typeof(ActionClass), "ValidateAnAction", new[] {typeof(string)});
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersRemoveFrom() {
            var actionMethod = FindMethodIgnoreParms(typeof(CollectionClass), "RemoveFromACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersValidate() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "ValidateAProperty");
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersValidateAction() {
            var actionMethod = FindMethod(typeof(ActionClass), "ValidateAnAction", Array.Empty<Type>());
            Assert.IsTrue(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersValidateAddTo() {
            var actionMethod = FindMethodIgnoreParms(typeof(CollectionClass1), "ValidateAddToACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestFiltersValidateRemoveFrom() {
            var actionMethod = FindMethodIgnoreParms(typeof(CollectionClass1), "ValidateRemoveFromACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesAddToIfNoBaseCollection() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoCollectionClass), "AddToACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesChoicesIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "ChoicesAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesClearIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "ClearAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesDefaultIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "DefaultAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesDisableActionIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "DisableAnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesDisableIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "DisableAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesHideActionIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "HideAnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesHideIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "HideAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesMeaninglessPrefix() {
            var actionMethod = FindMethodIgnoreParms(typeof(PropertyClass), "MeaninglessPrefixAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesMeaninglessPrefixIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "MeaninglessPrefixAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesModifyIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "ModifyAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesParameterChoicesIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "ChoicesAnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesParameterDefaultIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "DefaultAnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesParameterIndexChoicesIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "Choices0AnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesParameterIndexDefaultIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "Default0AnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesParameterIndexValidateIfNoAction() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoActionClass), "Validate0AnAction");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesParameterValidateIfNoAction() {
            var actionMethod = FindMethod(typeof(NoActionClass), "ValidateAnAction", new[] {typeof(string)});
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesRemoveFromIfNoBaseCollection() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoCollectionClass), "RemoveFromACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesValidateActionIfNoAction() {
            var actionMethod = FindMethod(typeof(NoActionClass), "ValidateAnAction", Array.Empty<Type>());
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesValidateAddToIfNoBaseCollection() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoCollectionClass1), "ValidateAddToACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesValidateIfNoBaseProperty() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoPropertyClass), "ValidateAProperty");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        [TestMethod]
        public void TestLeavesValidateRemoveFromIfNoBaseCollection() {
            var actionMethod = FindMethodIgnoreParms(typeof(NoCollectionClass1), "ValidateRemoveFromACollection");
            Assert.IsFalse(facetFactory.Filters(actionMethod, null));
        }

        #region Nested type: ActionClass

        private class ActionClass : BaseActionClass {
            public override string ValidateAnAction()
            {
                return string.Empty;
            }

            public override string DisableAnAction()
            {
                return null;
            }

            public override bool HideAnAction()
            {
                return false;
            }

            public override IList<string> Choices0AnAction()
            {
                return null;
            }

            public override string Default0AnAction()
            {
                return null;
            }

            public override string Validate0AnAction(string parm)
            {
                return null;
            }

            public override IList<string> ChoicesAnAction(string parm)
            {
                return null;
            }

            public override string DefaultAnAction(string parm)
            {
                return null;
            }

            public override string ValidateAnAction(string parm)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: BaseActionClass

        private class BaseActionClass {
// ReSharper disable once UnusedMember.Local
// ReSharper disable once UnusedParameter.Local
            public void AnAction(string parm) { }

            public virtual string ValidateAnAction()
            {
                return string.Empty;
            }

            public virtual string DisableAnAction()
            {
                return null;
            }

            public virtual bool HideAnAction()
            {
                return false;
            }

            public virtual IList<string> Choices0AnAction()
            {
                return null;
            }

            public virtual string Default0AnAction()
            {
                return null;
            }

            public virtual string Validate0AnAction(string parm)
            {
                return null;
            }

            public virtual IList<string> ChoicesAnAction(string parm)
            {
                return null;
            }

            public virtual string DefaultAnAction(string parm)
            {
                return null;
            }

            public virtual string ValidateAnAction(string parm)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: BaseCollectionClass

        private class BaseCollectionClass {
            public virtual ICollection<string> ACollection { get; set; }
            public virtual void AddToACollection(string value) { }
            public virtual void RemoveFromACollection(string value) { }
        }

        #endregion

        #region Nested type: BaseCollectionClass1

        private class BaseCollectionClass1 {
            public virtual ICollection<string> ACollection { get; set; }

            public virtual string ValidateAddToACollection(string value)
            {
                return null;
            }

            public virtual string ValidateRemoveFromACollection(string value)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: BaseNoActionClass

        private class BaseNoActionClass {
            public virtual string ValidateAnAction()
            {
                return string.Empty;
            }

            public virtual string DisableAnAction()
            {
                return null;
            }

            public virtual bool HideAnAction()
            {
                return false;
            }

            public virtual IList<string> Choices0AnAction()
            {
                return null;
            }

            public virtual string Default0AnAction()
            {
                return null;
            }

            public virtual string Validate0AnAction(string parm)
            {
                return null;
            }

            public virtual IList<string> ChoicesAnAction(string parm)
            {
                return null;
            }

            public virtual string DefaultAnAction(string parm)
            {
                return null;
            }

            public virtual string ValidateAnAction(string parm)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: BaseNoCollectionClass

        private class BaseNoCollectionClass {
            public virtual void AddToACollection(string value) { }
            public virtual void RemoveFromACollection(string value) { }
        }

        #endregion

        #region Nested type: BaseNoCollectionClass1

        private class BaseNoCollectionClass1 {
            public virtual string ValidateAddToACollection(string value)
            {
                return null;
            }

            public virtual string ValidateRemoveFromACollection(string value)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: BaseNoPropertyClass

        private class BaseNoPropertyClass {
            public virtual bool MeaninglessPrefixAProperty()
            {
                return false;
            }

            public virtual void ModifyAProperty(Type value) { }
            public virtual void ClearAProperty() { }

            public virtual IList<string> ChoicesAProperty()
            {
                return null;
            }

            public virtual string DefaultAProperty()
            {
                return string.Empty;
            }

            public virtual string ValidateAProperty(string prop)
            {
                return string.Empty;
            }

            public virtual string DisableAProperty()
            {
                return null;
            }

            public virtual bool HideAProperty()
            {
                return false;
            }
        }

        #endregion

        #region Nested type: BasePropertyClass

        private class BasePropertyClass {
            public virtual string AProperty { get; set; }

            public virtual bool MeaninglessPrefixAProperty()
            {
                return false;
            }

            public virtual void ModifyAProperty(Type value) { }
            public virtual void ClearAProperty() { }

            public virtual IList<string> ChoicesAProperty()
            {
                return null;
            }

            public virtual string DefaultAProperty()
            {
                return string.Empty;
            }

            public virtual string ValidateAProperty(string prop)
            {
                return string.Empty;
            }

            public virtual string DisableAProperty()
            {
                return null;
            }

            public virtual bool HideAProperty()
            {
                return false;
            }
        }

        #endregion

        #region Nested type: CollectionClass

        private class CollectionClass : BaseCollectionClass {
            public override void AddToACollection(string value) { }
            public override void RemoveFromACollection(string value) { }
        }

        #endregion

        #region Nested type: CollectionClass1

        private class CollectionClass1 : BaseCollectionClass1 {
            public override string ValidateAddToACollection(string value)
            {
                return null;
            }

            public override string ValidateRemoveFromACollection(string value)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: NoActionClass

        private class NoActionClass : BaseNoActionClass {
            public override string ValidateAnAction()
            {
                return string.Empty;
            }

            public override string DisableAnAction()
            {
                return null;
            }

            public override bool HideAnAction()
            {
                return false;
            }

            public override IList<string> Choices0AnAction()
            {
                return null;
            }

            public override string Default0AnAction()
            {
                return null;
            }

            public override string Validate0AnAction(string parm)
            {
                return null;
            }

            public override IList<string> ChoicesAnAction(string parm)
            {
                return null;
            }

            public override string DefaultAnAction(string parm)
            {
                return null;
            }

            public override string ValidateAnAction(string parm)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: NoCollectionClass

        private class NoCollectionClass : BaseNoCollectionClass {
            public override void AddToACollection(string value) { }
            public override void RemoveFromACollection(string value) { }
        }

        #endregion

        #region Nested type: NoCollectionClass1

        private class NoCollectionClass1 : BaseNoCollectionClass1 {
            public override string ValidateAddToACollection(string value)
            {
                return null;
            }

            public override string ValidateRemoveFromACollection(string value)
            {
                return null;
            }
        }

        #endregion

        #region Nested type: NoPropertyClass

        private class NoPropertyClass : BaseNoPropertyClass {
            public override bool MeaninglessPrefixAProperty()
            {
                return false;
            }

            public override void ModifyAProperty(Type value) { }
            public override void ClearAProperty() { }

            public override IList<string> ChoicesAProperty()
            {
                return null;
            }

            public override string DefaultAProperty()
            {
                return string.Empty;
            }

            public override string ValidateAProperty(string prop)
            {
                return string.Empty;
            }

            public override string DisableAProperty()
            {
                return null;
            }

            public override bool HideAProperty()
            {
                return false;
            }
        }

        #endregion

        #region Nested type: PropertyClass

        private class PropertyClass : BasePropertyClass {
            public override bool MeaninglessPrefixAProperty()
            {
                return false;
            }

            public override void ModifyAProperty(Type value) { }
            public override void ClearAProperty() { }

            public override IList<string> ChoicesAProperty()
            {
                return null;
            }

            public override string DefaultAProperty()
            {
                return string.Empty;
            }

            public override string ValidateAProperty(string prop)
            {
                return string.Empty;
            }

            public override string DisableAProperty()
            {
                return null;
            }

            public override bool HideAProperty()
            {
                return false;
            }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ComplementaryMethodsFilteringFacetFactory(GetOrder<ComplementaryMethodsFilteringFacetFactory>(), LoggerFactory);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}