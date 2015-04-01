using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Web.Mvc.Html {
    public interface IIdHelper {
        string GetDisplayFormatId(string id);
        string GetCollectionItemId(INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetObjectId(INakedObjectAdapter owner);
        string GetFieldId(INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetInlineFieldId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetFieldAutoCompleteId(string id, INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetInlineFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetConcurrencyFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetInlineConcurrencyFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc);
        string GetConcurrencyActionInputId(INakedObjectAdapter owner, IActionSpec action, IAssociationSpec assoc);
        string GetActionId(INakedObjectAdapter owner, IActionSpec action);
        string GetActionDialogId(INakedObjectAdapter owner, IActionSpec action);
        string GetSubMenuId(INakedObjectAdapter owner, IActionSpec action);
        string GetSubMenuId(INakedObjectAdapter owner, INakedObjectAdapter service);
        string GetFindMenuId(INakedObjectAdapter nakedObject, IActionSpec action, string propertyName);
        string GetParameterId(IActionSpec action, IActionParameterSpec parameter);
        string GetParameterInputId(IActionSpec action, IActionParameterSpec parameter);
        string GetParameterAutoCompleteId(IActionSpec action, IActionParameterSpec parameter);
        string GetCollectionContainerId(INakedObjectAdapter collection);
        string GetActionContainerId(INakedObjectAdapter nakedObject);
        string GetServiceContainerId(INakedObjectAdapter nakedObject);
        string GetFieldContainerId(INakedObjectAdapter nakedObject);
        string GetParameterContainerId(IActionSpec action);
        string GetGenericActionId(INakedObjectAdapter owner, string type);
        string GetActionLabel(INakedObjectAdapter nakedObject);
        string GetServiceLabel(INakedObjectAdapter nakedObject);
        string GetMandatoryIndicatorClass();
        string GetMandatoryIndicator();
        string MakeId(params string[] ids);
        bool KeyPrefixIs(string key, string match);
        string GetActionId(ActionContext targetActionContext, ActionContext actionContext, string propertyName);
    }
}