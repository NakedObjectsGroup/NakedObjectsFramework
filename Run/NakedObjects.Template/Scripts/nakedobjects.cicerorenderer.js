/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var TypePlusTitle = NakedObjects.Models.typePlusTitle;
    var PlusTitle = NakedObjects.Models.typePlusTitle;
    var FriendlyNameForParam = NakedObjects.Models.friendlyNameForParam;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    var Title = NakedObjects.Models.typePlusTitle;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    var FriendlyNameForProperty = NakedObjects.Models.friendlyNameForProperty;
    var EntryType = NakedObjects.Models.EntryType;
    var toUtcDate = NakedObjects.Models.toUtcDate;
    var isDateOrDateTime = NakedObjects.Models.isDateOrDateTime;
    NakedObjects.app.service("ciceroRenderer", function ($q, context, mask, error) {
        var renderer = this;
        renderer.renderHome = function (cvm, routeData) {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            }
            else {
                var output_1 = "";
                if (routeData.menuId) {
                    context.getMenu(routeData.menuId)
                        .then(function (menu) {
                        output_1 += menu.title() + " menu" + "\n";
                        return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null);
                    }).then(function (details) {
                        if (details) {
                            output_1 += renderActionDialog(details, routeData, mask);
                        }
                    }).finally(function () {
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output_1);
                    });
                }
                else {
                    cvm.clearInput();
                    cvm.output = NakedObjects.welcomeMessage;
                }
            }
        };
        renderer.renderObject = function (cvm, routeData) {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            }
            else {
                var oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
                context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                    .then(function (obj) {
                    var output = "";
                    var openCollIds = openCollectionIds(routeData);
                    if (_.some(openCollIds)) {
                        var id = openCollIds[0];
                        var coll = obj.collectionMember(id);
                        output += "Collection: " + coll.extensions().friendlyName() + " on " + TypePlusTitle(obj) + "\n";
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
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    }
                    else {
                        if (obj.isTransient()) {
                            output += "Unsaved ";
                            output += obj.extensions().friendlyName() + "\n";
                            output += renderModifiedProperties(obj, routeData, mask);
                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                        }
                        else if (routeData.interactionMode === NakedObjects.InteractionMode.Edit ||
                            routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                            var output_2 = "Editing ";
                            output_2 += PlusTitle(obj) + "\n";
                            if (routeData.dialogId) {
                                context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                    .then(function (details) {
                                    output_2 += renderActionDialog(details, routeData, mask);
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output_2);
                                });
                            }
                            else {
                                output_2 += renderModifiedProperties(obj, routeData, mask);
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output_2);
                            }
                        }
                        else {
                            var output_3 = Title(obj) + "\n";
                            if (routeData.dialogId) {
                                context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                    .then(function (details) {
                                    output_3 += renderActionDialog(details, routeData, mask);
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output_3);
                                });
                            }
                            else {
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output_3);
                            }
                        }
                    }
                }).catch(function (reject) {
                    if (reject.category === ErrorCategory.ClientError && reject.clientErrorCode === ClientErrorCode.ExpiredTransient) {
                        cvm.output = "The requested view of unsaved object details has expired";
                    }
                    else {
                        error.handleError(reject);
                    }
                });
            }
        };
        renderer.renderList = function (cvm, routeData) {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            }
            else {
                var listPromise = context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
                listPromise.then(function (list) {
                    context.getMenu(routeData.menuId).then(function (menu) {
                        var count = list.value().length;
                        var numPages = list.pagination().numPages;
                        var description;
                        if (numPages > 1) {
                            var page = list.pagination().page;
                            var totalCount = list.pagination().totalCount;
                            description = "Page " + page + " of " + numPages + " containing " + count + " of " + totalCount + " items";
                        }
                        else {
                            description = count + " items";
                        }
                        var actionMember = menu.actionMember(routeData.actionId);
                        var actionName = actionMember.extensions().friendlyName();
                        var output = "Result from " + actionName + ":\n" + description;
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    });
                });
            }
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
        function renderActionDialog(invokable, routeData, mask) {
            var actionName = invokable.extensions().friendlyName();
            var output = "Action dialog: " + actionName + "\n";
            _.forEach(NakedObjects.getParametersAndCurrentValue(invokable, context), function (value, paramId) {
                output += FriendlyNameForParam(invokable, paramId) + ": ";
                var param = invokable.parameters()[paramId];
                output += renderFieldValue(param, value, mask);
                output += "\n";
            });
            return output;
        }
        function renderModifiedProperties(obj, routeData, mask) {
            var output = "";
            if (_.keys(routeData.props).length > 0) {
                output += "Modified properties:\n";
                _.each(routeData.props, function (value, propId) {
                    output += FriendlyNameForProperty(obj, propId) + ": ";
                    var pm = obj.propertyMember(propId);
                    output += renderFieldValue(pm, value, mask);
                    output += "\n";
                });
            }
            return output;
        }
    });
    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    function openCollectionIds(routeData) {
        return _.filter(_.keys(routeData.collections), function (k) { return routeData.collections[k] !== NakedObjects.CollectionViewState.Summary; });
    }
    NakedObjects.openCollectionIds = openCollectionIds;
    //Handles empty values, and also enum conversion
    function renderFieldValue(field, value, mask) {
        if (!field.isScalar()) {
            return value.isNull() ? "empty" : value.toString();
        }
        //Rest is for scalar fields only:
        if (value.toString()) {
            //This is to handle an enum: render it as text, not a number:           
            if (field.entryType() === EntryType.Choices) {
                var inverted = _.invert(field.choices());
                return inverted[value.toValueString()];
            }
            else if (field.entryType() === EntryType.MultipleChoices && value.isList()) {
                var inverted_1 = _.invert(field.choices());
                var output_4 = "";
                var values = value.list();
                _.forEach(values, function (v) {
                    output_4 += inverted_1[v.toValueString()] + ",";
                });
                return output_4;
            }
        }
        var properScalarValue;
        if (isDateOrDateTime(field)) {
            properScalarValue = toUtcDate(value);
        }
        else {
            properScalarValue = value.scalar();
        }
        if (properScalarValue === "" || properScalarValue == null) {
            return "empty";
        }
        else {
            var remoteMask = field.extensions().mask();
            var format = field.extensions().format();
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }
    NakedObjects.renderFieldValue = renderFieldValue;
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.cicerorenderer.js.map