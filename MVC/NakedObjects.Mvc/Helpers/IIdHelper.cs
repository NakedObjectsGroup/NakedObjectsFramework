using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface;

namespace NakedObjects.Web.Mvc.Html {
    public interface IIdHelper {
        string GetDisplayFormatId(string id);
        string GetCollectionItemId(INakedObjectSurface owner, IAssociationSpec assoc);
        string GetObjectId(INakedObjectSurface owner);
        string GetFieldId(INakedObjectSurface owner, IAssociationSpec assoc);
        string GetInlineFieldId(IAssociationSpec parent, INakedObjectSurface owner, IAssociationSpec assoc);
        string GetFieldInputId(INakedObjectSurface owner, IAssociationSpec assoc);
        string GetFieldAutoCompleteId(string id, INakedObjectSurface owner, IAssociationSpec assoc);
        string GetInlineFieldInputId(IAssociationSpec parent, INakedObjectSurface owner, IAssociationSpec assoc);
        string GetConcurrencyFieldInputId(INakedObjectSurface owner, IAssociationSpec assoc);
        string GetInlineConcurrencyFieldInputId(IAssociationSpec parent, INakedObjectSurface owner, IAssociationSpec assoc);
        string GetConcurrencyActionInputId(INakedObjectSurface owner, IActionSpec action, IAssociationSpec assoc);
        string GetActionId(INakedObjectSurface owner, IActionSpec action);
        string GetActionDialogId(INakedObjectSurface owner, IActionSpec action);
        string GetSubMenuId(INakedObjectSurface owner, IActionSpec action);
        string GetSubMenuId(INakedObjectSurface owner, INakedObjectSurface service);
        string GetFindMenuId(INakedObjectSurface nakedObject, IActionSpec action, string propertyName);
        string GetParameterId(IActionSpec action, IActionParameterSpec parameter);
        string GetParameterInputId(IActionSpec action, IActionParameterSpec parameter);
        string GetParameterAutoCompleteId(IActionSpec action, IActionParameterSpec parameter);
        string GetCollectionContainerId(INakedObjectSurface collection);
        string GetActionContainerId(INakedObjectSurface nakedObject);
        string GetServiceContainerId(INakedObjectSurface nakedObject);
        string GetFieldContainerId(INakedObjectSurface nakedObject);
        string GetParameterContainerId(IActionSpec action);
        string GetGenericActionId(INakedObjectSurface owner, string type);
        string GetActionLabel(INakedObjectSurface nakedObject);
        string GetServiceLabel(INakedObjectSurface nakedObject);
        string GetMandatoryIndicatorClass();
        string GetMandatoryIndicator();
        string MakeId(params string[] ids);
        bool KeyPrefixIs(string key, string match);
        string GetActionId(ActionContext targetActionContext, ActionContext actionContext, string propertyName);
    }
}