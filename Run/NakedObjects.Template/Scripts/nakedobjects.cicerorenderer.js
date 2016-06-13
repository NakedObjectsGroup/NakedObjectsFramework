/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var TypePlusTitle = NakedObjects.Models.typePlusTitle;
    var PlusTitle = NakedObjects.Models.typePlusTitle;
    var FriendlyNameForParam = NakedObjects.Models.friendlyNameForParam;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    NakedObjects.app.service("ciceroRenderer", function (context) {
        var renderer = this;
        renderer.renderHome = function (routeData, cvm) {
            if (routeData.menuId) {
                context.getMenu(routeData.menuId)
                    .then(function (menu) {
                    var output = ""; //TODO: use builder
                    output += menu.title() + " menu" + ". ";
                    output += renderActionDialogIfOpen(menu, routeData);
                    cvm.clearInput();
                    cvm.output = output;
                });
            }
            else {
                cvm.clearInput();
                cvm.output = "Welcome to Cicero. Type 'help' and the Enter key for more information.";
            }
        };
        renderer.renderObject = function (routeData, cvm) {
            var oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
            context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                .then(function (obj) {
                var output = "";
                var openCollIds = openCollectionIds(routeData);
                if (_.some(openCollIds)) {
                    var id = openCollIds[0];
                    var coll = obj.collectionMember(id);
                    output += "Collection: " + coll.extensions().friendlyName();
                    output += " on " + TypePlusTitle(obj) + ",  ";
                    switch (coll.size()) {
                        case 0:
                            output += "empty";
                            break;
                        case 1:
                            output += "1 item";
                            break;
                        default:
                            output += coll.size() + " items";
                    }
                }
                else {
                    if (routeData.interactionMode === NakedObjects.InteractionMode.Edit) {
                        output += "Editing ";
                    }
                    output += PlusTitle(obj) + ". ";
                    output += renderActionDialogIfOpen(obj, routeData);
                }
                cvm.clearInput();
                cvm.output = output;
            });
        };
        renderer.renderList = function (routeData, cvm) {
            var listPromise = context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
            listPromise.then(function (list) {
                var page = list.pagination().page;
                var numPages = list.pagination().numPages;
                var count = list.value().length;
                var totalCount = list.pagination().totalCount;
                var description = "Page " + page + " of " + numPages + " containing " + count + " of " + totalCount + " items";
                context.getMenu(routeData.menuId).then(function (menu) {
                    var actionMember = menu.actionMember(routeData.actionId);
                    var actionName = actionMember.extensions().friendlyName();
                    cvm.clearInput();
                    cvm.output = "Result from " + actionName + ": " + description;
                });
                //TODO: add number of items selected, if any.
            });
        };
        renderer.renderError = function (cvm) {
            var err = context.getError().error;
            cvm.clearInput();
            cvm.output = "Sorry, an application error has occurred. " + err.message();
        };
        //Returns collection Ids for any collections on an object that are currently in List or Table mode
        function openCollectionIds(routeData) {
            return _.filter(_.keys(routeData.collections), function (k) { return routeData.collections[k] != NakedObjects.CollectionViewState.Summary; });
        }
        function renderActionDialogIfOpen(repWithActions, routeData) {
            var output = "";
            if (routeData.dialogId) {
                // can safely downcast as we know we're in a dialog
                var actionMember_1 = repWithActions.actionMember(routeData.dialogId);
                var actionName = actionMember_1.extensions().friendlyName();
                output += "Action dialog: " + actionName + ". ";
                _.forEach(NakedObjects.getParametersAndCurrentValue(actionMember_1, context), function (value, key) {
                    output += FriendlyNameForParam(actionMember_1, key) + ": ";
                    output += value.toString() || "empty";
                    output += ", ";
                });
            }
            return output;
        }
    });
})(NakedObjects || (NakedObjects = {}));
//Code to go in ViewModelFactory
//cvm.renderHome = _.partial(ciceroRenderer.renderHome, cvm) as (routeData: PaneRouteData) => void;
//cvm.renderObject = _.partial(ciceroRenderer.renderObject, cvm) as (routeData: PaneRouteData) => void;
//cvm.renderList = _.partial(ciceroRenderer.renderList, cvm) as (routeData: PaneRouteData) => void;
//cvm.renderError = _.partial(ciceroRenderer.renderError, cvm); 
//# sourceMappingURL=nakedobjects.cicerorenderer.js.map