using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface;

namespace NakedObjects.Web.Mvc.Html {
    public interface IIdHelper {
        string GetDisplayFormatId(string id);
        string GetCollectionItemId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetObjectId(INakedObjectSurface owner);
        string GetFieldId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetInlineFieldId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetFieldInputId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetFieldAutoCompleteId(string id, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetInlineFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetConcurrencyFieldInputId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetInlineConcurrencyFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetConcurrencyActionInputId(INakedObjectSurface owner, INakedObjectActionSurface action, INakedObjectAssociationSurface assoc);
        string GetActionId(INakedObjectSurface owner, INakedObjectActionSurface action);
        string GetActionDialogId(INakedObjectSurface owner, INakedObjectActionSurface action);
        string GetSubMenuId(INakedObjectSurface owner, INakedObjectActionSurface action);
        string GetSubMenuId(INakedObjectSurface owner, INakedObjectSurface service);
        string GetFindMenuId(INakedObjectSurface nakedObject, INakedObjectActionSurface action, string propertyName);
        string GetParameterId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetParameterInputId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetParameterAutoCompleteId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetCollectionContainerId(INakedObjectSurface collection);
        string GetActionContainerId(INakedObjectSurface nakedObject);
        string GetServiceContainerId(INakedObjectSurface nakedObject);
        string GetFieldContainerId(INakedObjectSurface nakedObject);
        string GetParameterContainerId(INakedObjectActionSurface action);
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