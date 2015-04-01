// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Web.Routing;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Web.Mvc.Html {
    public abstract class ObjectContext {
        protected IIdHelper IdHelper { get; set; }

        protected ObjectContext(IIdHelper idHelper ,ObjectContext otherContext) {
            IdHelper = idHelper;
            Target = otherContext.Target;
        }

        protected ObjectContext(IIdHelper idHelper ,INakedObjectAdapter target) {
            IdHelper = idHelper;
            Target = target;
        }

        public INakedObjectAdapter Target { get; set; }
    }

    public abstract class FeatureContext : ObjectContext {
        protected FeatureContext(IIdHelper idHelper ,ObjectContext otherContext) : base(idHelper, otherContext) { }
        protected FeatureContext(IIdHelper idHelper ,INakedObjectAdapter target) : base(idHelper, target) {}
        public abstract IFeatureSpec Feature { get; }
    }

    internal class PropertyContext : FeatureContext {

        public PropertyContext(IIdHelper idHelper, PropertyContext otherContext)
            : base(idHelper ,otherContext) {
            ParentContext = otherContext.ParentContext;
        }

        public PropertyContext(IIdHelper idHelper, INakedObjectAdapter target, IAssociationSpec property, bool isEdit, PropertyContext parentContext = null)
            : base(idHelper ,target) {
            Property = property;
            IsPropertyEdit = isEdit;
            IsEdit = isEdit;
            ParentContext = parentContext;
        }

        public INakedObjectAdapter OriginalTarget {
            get { return ParentContext == null ? Target : ParentContext.OriginalTarget; }
        }

        public PropertyContext ParentContext { get; set; }

        public IAssociationSpec Property { get; set; }

        public override IFeatureSpec Feature {
            get { return Property; }
        }

        public INakedObjectAdapter GetValue(INakedObjectsFramework framework) {
            return Property.GetNakedObject(Target);
        }

        public bool IsEdit { get; private set; }

        public bool IsPropertyEdit { get; set; }

        public bool IsFindMenuEnabled() {
            if (Property is IOneToOneAssociationSpec) {
                return (Property as IOneToOneAssociationSpec).IsFindMenuEnabled;
            }
            return false;
        }

        public ObjectContext OuterContext {
            get { return this; }
        }

        public string GetFieldInputId() {
            return ParentContext == null ? IdHelper.GetFieldInputId(Target, Property) : IdHelper.GetInlineFieldInputId(ParentContext.Property, Target, Property);
        }

        public string GetAutoCompleteFieldId() {
            // append autocomplete suffix if reference field only to differentiate for hidden input field with actual value 
            return IdHelper.GetFieldAutoCompleteId(GetFieldInputId(), Target, Property);
        }

        public string GetFieldId() {
            var thisFieldId = IdHelper.GetFieldId(Target, Property);
            return ParentContext == null ? thisFieldId : IdHelper.MakeId(ParentContext.GetFieldId(), thisFieldId);
        }

        private string GetPresentationHint() {
            var facet = Property.GetFacet<IPresentationHintFacet>();
            return facet == null ? "" : " " + facet.Value;
        }

        public string GetFieldClass() {
            return IdConstants.FieldName + GetPresentationHint();
        }

        public string GetConcurrencyFieldInputId() {
            return ParentContext == null ? IdHelper.GetConcurrencyFieldInputId(Target, Property) : IdHelper.GetInlineConcurrencyFieldInputId(ParentContext.Property, Target, Property);
        }
    }

    public class ActionContext : FeatureContext {
        public ActionContext(IIdHelper idHelper, ActionContext otherContext)
            : base(idHelper, otherContext) {
            EmbeddedInObject = otherContext.EmbeddedInObject;
            Action = otherContext.Action;
        }

        public ActionContext(IIdHelper idHelper, INakedObjectAdapter target, IActionSpec action)
            : base(idHelper, target) {
            EmbeddedInObject = false;
            Action = action;
        }

        public ActionContext(IIdHelper idHelper, bool embeddedInObject, INakedObjectAdapter target, IActionSpec action)
            : base(idHelper, target) {
            EmbeddedInObject = embeddedInObject;
            Action = action;
        }

        private Func<IActionParameterSpec, bool> filter;

        private ParameterContext[] parameterContexts;

        public Func<IActionParameterSpec, bool> Filter {
            get {
                if (filter == null) {
                    return x => true;
                }
                return filter;
            }
            set { filter = value; }
        }

        public bool EmbeddedInObject { get; set; }

        public IActionSpec Action { get; set; }

        public RouteValueDictionary ParameterValues { get; set; }

        public ParameterContext[] GetParameterContexts(INakedObjectsFramework framework) {
            if (parameterContexts == null) {
                parameterContexts = Action.Parameters.Where(Filter).Select(p => new ParameterContext(IdHelper, EmbeddedInObject, Target, Action, p, true)).ToArray();

                if (ParameterValues != null) {
                    foreach (ParameterContext pc in parameterContexts) {
                        object value;
                        if (ParameterValues.TryGetValue(pc.Parameter.Id, out value)) {
                            pc.IsHidden = true;
                            pc.CustomValue = framework.GetNakedObject(value);
                        }
                    }
                }
            }

            return parameterContexts;
        }

        public override IFeatureSpec Feature {
            get { return Action; }
        }

        public string GetConcurrencyActionInputId(IAssociationSpec nakedObjectAssociation) {
            return IdHelper.GetConcurrencyActionInputId(Target, Action, nakedObjectAssociation);
        }

        public string GetActionId() {
            return IdHelper.GetActionId(Target, Action);
        }

        private string GetPresentationHint() {
            var facet = Action == null ? null : Action.GetFacet<IPresentationHintFacet>();
            return facet == null ? "" : " " + facet.Value;
        }

        private bool IsFileActionNoParms(INakedObjectsFramework framework) {
            return Action != null && Action.ReturnSpec.IsFile(framework) && !Action.Parameters.Any();
        }

        public string GetActionClass(INakedObjectsFramework framework) {
            return (IsFileActionNoParms(framework) ? IdConstants.ActionNameFile : IdConstants.ActionName) + GetPresentationHint();
        }

        public string GetSubMenuId() {
            return IdHelper.GetSubMenuId(Target, Action);
        }

        public string GetActionDialogId() {
            return IdHelper.GetActionDialogId(Target, Action);
        }

        public string GetFindMenuId(string propertyName) {
            return IdHelper.GetFindMenuId(Target, Action, propertyName);
        }
    }

    public class ParameterContext : ActionContext {
        public ParameterContext(IIdHelper idhelper, ParameterContext otherContext) : base(idhelper, otherContext) {
            Parameter = otherContext.Parameter;
        }

        public ParameterContext(IIdHelper idhelper, bool embeddedInObject, INakedObjectAdapter target, IActionSpec action, IActionParameterSpec parameter, bool isEdit)
            : base(idhelper, embeddedInObject, target, action) {
            Parameter = parameter;
            IsParameterEdit = isEdit;
        }

        public bool IsHidden { get; set; }

        public bool IsFindMenuEnabled() {
            if (Parameter is IOneToOneActionParameterSpec) {
                var p = Parameter as IOneToOneActionParameterSpec;
                return p.IsFindMenuEnabled && (!p.Action.IsContributedMethod || !Target.Spec.IsOfType(p.Spec));
            }
            return false;
        }

        public IActionParameterSpec Parameter { get; set; }

        public INakedObjectAdapter CustomValue { get; set; }

        public override IFeatureSpec Feature {
            get { return Parameter; }
        }

        public bool IsParameterEdit { get; set; }

        public string GetParameterInputId() {
            return IdHelper.GetParameterInputId(Action, Parameter);
        }

        public string GetParameterAutoCompleteId() {
            return IdHelper.GetParameterAutoCompleteId(Action, Parameter);
        }

        public string GetParameterId() {
            return IdHelper.GetParameterId(Action, Parameter);
        }

        private string GetPresentationHint() {
            var facet = Parameter == null ? null : Parameter.GetFacet<IPresentationHintFacet>();
            return facet == null ? "" : " " + facet.Value;
        }

        public string GetParameterClass() {
            return IdConstants.ParamName + GetPresentationHint();
        }
    }
}