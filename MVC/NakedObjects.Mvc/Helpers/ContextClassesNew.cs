// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Web.Routing;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Web.Mvc.Html {
    internal abstract class ObjectContextNew {
        protected IIdHelper IdHelper { get; set; }

        protected ObjectContextNew(IIdHelper idHelper ,ObjectContextNew otherContext) {
            IdHelper = idHelper;
            Target = otherContext.Target;
        }

        protected ObjectContextNew(IIdHelper idHelper ,INakedObjectSurface target) {
            IdHelper = idHelper;
            Target = target;
        }

        public INakedObjectSurface Target { get; set; }
    }

    internal abstract class FeatureContextNew : ObjectContextNew {
        protected FeatureContextNew(IIdHelper idHelper ,ObjectContextNew otherContext) : base(idHelper, otherContext) { }
        protected FeatureContextNew(IIdHelper idHelper ,INakedObjectSurface target) : base(idHelper, target) {}
        public abstract IScalarPropertyHolder Feature { get; }
    }

    internal class PropertyContextNew : FeatureContextNew {

        public PropertyContextNew(IIdHelper idHelper, PropertyContextNew otherContext)
            : base(idHelper ,otherContext) {
            ParentContext = otherContext.ParentContext;
        }

        public PropertyContextNew(IIdHelper idHelper, INakedObjectSurface target, INakedObjectAssociationSurface property, bool isEdit, PropertyContextNew parentContext = null)
            : base(idHelper ,target) {
            Property = property;
            IsPropertyEdit = isEdit;
            IsEdit = isEdit;
            ParentContext = parentContext;
        }

        public INakedObjectSurface OriginalTarget {
            get { return ParentContext == null ? Target : ParentContext.OriginalTarget; }
        }

        public PropertyContextNew ParentContext { get; set; }

        public INakedObjectAssociationSurface Property { get; set; }

        public override IScalarPropertyHolder Feature {
            get { return Property; }
        }

        public INakedObjectSurface GetValue(INakedObjectsSurface surface) {
            return Property.GetNakedObject(Target);
        }

        public bool IsEdit { get; private set; }

        public bool IsPropertyEdit { get; set; }

        public bool IsFindMenuEnabled() {
            return Property.IsFindMenuEnabled();
        }

        public ObjectContextNew OuterContext {
            get { return this; }
        }

        public string GetFieldInputId() {
            return ParentContext == null ? IdHelper.GetFieldInputId(Target, Property) :
                IdHelper.GetInlineFieldInputId(ParentContext.Property, Target, Property);
        }

        public string GetAutoCompleteFieldId() {
            // append autocomplete suffix if reference field only to differentiate for hidden input field with actual value 
            return IdHelper.GetFieldAutoCompleteId(GetFieldInputId(), Target, Property);
        }

        public string GetFieldId() {
            var thisFieldId = IdHelper.GetFieldId((Target), (Property));
            return ParentContext == null ? thisFieldId : IdHelper.MakeId(ParentContext.GetFieldId(), thisFieldId);
        }

        private string GetPresentationHint() {
            var hint = Property.PresentationHint();
            return string.IsNullOrWhiteSpace(hint) ? "" : " " + hint;
        }

        public string GetFieldClass() {
            return IdConstants.FieldName + GetPresentationHint();
        }

        public string GetConcurrencyFieldInputId() {
            return ParentContext == null ? IdHelper.GetConcurrencyFieldInputId(Target, Property) :
                IdHelper.GetInlineConcurrencyFieldInputId(ParentContext.Property, Target, Property);
        }
    }

    internal class ActionContextNew : FeatureContextNew {
        public ActionContextNew(IIdHelper idHelper, ActionContextNew otherContext)
            : base(idHelper, otherContext) {
            EmbeddedInObject = otherContext.EmbeddedInObject;
            Action = otherContext.Action;
        }

        public ActionContextNew(IIdHelper idHelper, INakedObjectSurface target, INakedObjectActionSurface action)
            : base(idHelper, target) {
            EmbeddedInObject = false;
            Action = action;
        }

        public ActionContextNew(IIdHelper idHelper, bool embeddedInObject, INakedObjectSurface target, INakedObjectActionSurface action)
            : base(idHelper, target) {
            EmbeddedInObject = embeddedInObject;
            Action = action;
        }

        private Func<INakedObjectActionParameterSurface, bool> filter;

        private ParameterContextNew[] parameterContexts;

        public Func<INakedObjectActionParameterSurface, bool> Filter {
            get {
                if (filter == null) {
                    return x => true;
                }
                return filter;
            }
            set { filter = value; }
        }

        public bool EmbeddedInObject { get; set; }

        public INakedObjectActionSurface Action { get; set; }

        public RouteValueDictionary ParameterValues { get; set; }

        public ParameterContextNew[] GetParameterContexts(INakedObjectsSurface surface) {
            if (parameterContexts == null) {
                parameterContexts = Action.Parameters.Where(Filter).Select(p => new ParameterContextNew(IdHelper, EmbeddedInObject, Target, Action, p, true)).ToArray();

                if (ParameterValues != null) {
                    foreach (ParameterContextNew pc in parameterContexts) {
                        object value;
                        if (ParameterValues.TryGetValue(pc.Parameter.Id, out value)) {
                            pc.IsHidden = true;
                            pc.CustomValue = surface.GetObject(value);
                        }
                    }
                }
            }

            return parameterContexts;
        }

        public override IScalarPropertyHolder Feature {
            get { return Action; }
        }

        public string GetConcurrencyActionInputId(INakedObjectAssociationSurface nakedObjectAssociation) {
            return IdHelper.GetConcurrencyActionInputId(Target, (Action), (nakedObjectAssociation));
        }

        public string GetActionId() {
            return IdHelper.GetActionId((Target), (Action));
        }

        private string GetPresentationHint() {
            var hint = Action != null ? Action.PresentationHint() : "";
            return string.IsNullOrWhiteSpace(hint) ? "" : " " + hint;
        }

        private bool IsFileActionNoParms() {        
            return Action != null && Action.ReturnType.IsFileAttachment() && !Action.Parameters.Any();
        }

        public string GetActionClass() {
            return (IsFileActionNoParms() ? IdConstants.ActionNameFile : IdConstants.ActionName) + GetPresentationHint();
        }

        public string GetSubMenuId() {
            return IdHelper.GetSubMenuId((Target), (Action));
        }

        public string GetActionDialogId() {
            return IdHelper.GetActionDialogId((Target), (Action));
        }

        public string GetFindMenuId(string propertyName) {
            return IdHelper.GetFindMenuId(Target, Action, propertyName);
        }
    }

    internal class ParameterContextNew : ActionContextNew {
        public ParameterContextNew(IIdHelper idhelper, ParameterContextNew otherContext) : base(idhelper, otherContext) {
            Parameter = otherContext.Parameter;
        }

        public ParameterContextNew(IIdHelper idhelper, bool embeddedInObject, INakedObjectSurface target, INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter, bool isEdit)
            : base(idhelper, embeddedInObject, target, action) {
            Parameter = parameter;
            IsParameterEdit = isEdit;
        }

        public bool IsHidden { get; set; }

        public bool IsFindMenuEnabled() {
            return Parameter.IsFindMenuEnabled() && (!Parameter.Action.IsContributed() || !Target.Specification.IsOfType(Parameter.Specification));
        }

        public INakedObjectActionParameterSurface Parameter { get; set; }

        public INakedObjectSurface CustomValue { get; set; }

        public override IScalarPropertyHolder Feature {
            get { return Parameter; }
        }

        public bool IsParameterEdit { get; set; }

        public string GetParameterInputId() {
            return IdHelper.GetParameterInputId((Action), (Parameter));
        }

        public string GetParameterAutoCompleteId() {
            return IdHelper.GetParameterAutoCompleteId((Action), (Parameter));
        }

        public string GetParameterId() {
            return IdHelper.GetParameterId((Action), (Parameter));
        }

        private string GetPresentationHint() {
            var hint = Parameter.PresentationHint();
            return string.IsNullOrWhiteSpace(hint) ? "" : " " + hint;
        }

        public string GetParameterClass() {
            return IdConstants.ParamName + GetPresentationHint();
        }
    }
}