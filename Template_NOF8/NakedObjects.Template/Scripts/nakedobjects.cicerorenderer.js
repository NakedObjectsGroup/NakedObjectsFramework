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
        var _this = this;
        var renderer = this;
        renderer.renderHome = function (cvm, routeData) {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            }
            else {
                if (routeData.menuId) {
                    renderOpenMenu(routeData, cvm);
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
                    var openCollIds = openCollectionIds(routeData);
                    if (_.some(openCollIds)) {
                        renderOpenCollection(openCollIds[0], obj, cvm);
                    }
                    else if (obj.isTransient()) {
                        renderTransientObject(routeData, obj, cvm);
                    }
                    else if (routeData.interactionMode === NakedObjects.InteractionMode.Edit ||
                        routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                        renderForm(routeData, obj, cvm);
                    }
                    else {
                        renderObjectTitleAndDialogIfOpen(routeData, obj, cvm);
                    }
                }).catch(function (reject) {
                    //TODO: Is the first test necessary or would this be rendered OK by generic error handling?
                    if (reject.category === ErrorCategory.ClientError && reject.clientErrorCode === ClientErrorCode.ExpiredTransient) {
                        cvm.output = NakedObjects.errorExpiredTransient;
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
                listPromise.
                    then(function (list) {
                    context.getMenu(routeData.menuId).
                        then(function (menu) {
                        var count = list.value().length;
                        var numPages = list.pagination().numPages;
                        var description = getListDescription(numPages, list, count);
                        var actionMember = menu.actionMember(routeData.actionId);
                        var actionName = actionMember.extensions().friendlyName();
                        var output = "Result from " + actionName + ":\n" + description;
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    }).
                        catch(function (reject) { return _this.error.handleError(reject); });
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
        renderer.renderError = function (cvm) {
            var err = context.getError().error;
            cvm.clearInput();
            cvm.output = "Sorry, an application error has occurred. " + err.message();
        };
        function getListDescription(numPages, list, count) {
            if (numPages > 1) {
                var page = list.pagination().page;
                var totalCount = list.pagination().totalCount;
                return "Page " + page + " of " + numPages + " containing " + count + " of " + totalCount + " items";
            }
            else {
                return count + " items";
            }
        }
        //Returns collection Ids for any collections on an object that are currently in List or Table mode
        function openCollectionIds(routeData) {
            return _.filter(_.keys(routeData.collections), function (k) { return routeData.collections[k] != NakedObjects.CollectionViewState.Summary; });
        }
        function renderOpenCollection(collId, obj, cvm) {
            var coll = obj.collectionMember(collId);
            var output = renderCollectionNameAndSize(coll);
            output += "(" + NakedObjects.collection + " " + NakedObjects.on + " " + TypePlusTitle(obj) + ")";
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
        function renderTransientObject(routeData, obj, cvm) {
            var output = NakedObjects.unsaved + " ";
            output += obj.extensions().friendlyName() + "\n";
            output += renderModifiedProperties(obj, routeData, mask);
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
        function renderForm(routeData, obj, cvm) {
            var _this = this;
            var output = NakedObjects.editing + " ";
            output += PlusTitle(obj) + "\n";
            if (routeData.dialogId) {
                context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                    then(function (invokableAction) {
                    output += renderActionDialog(invokableAction, routeData, mask);
                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
            else {
                output += renderModifiedProperties(obj, routeData, mask);
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }
        }
        function renderObjectTitleAndDialogIfOpen(routeData, obj, cvm) {
            var _this = this;
            var output = Title(obj) + "\n";
            if (routeData.dialogId) {
                context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                    then(function (invokableAction) {
                    output += renderActionDialog(invokableAction, routeData, mask);
                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
            else {
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }
        }
        function renderOpenMenu(routeData, cvm) {
            var _this = this;
            var output = "";
            context.getMenu(routeData.menuId).
                then(function (menu) {
                output += NakedObjects.menuTitle(menu.title());
                return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null);
            }).
                then(function (invokableAction) {
                if (invokableAction) {
                    output += "\n" + renderActionDialog(invokableAction, routeData, mask);
                }
            }).
                catch(function (reject) { return _this.error.handleError(reject); }).
                finally(function () { return cvm.clearInputRenderOutputAndAppendAlertIfAny(output); });
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
            var props = context.getCurrentObjectValues(obj.id());
            if (_.keys(props).length > 0) {
                output += NakedObjects.modifiedProperties + ":\n";
                _.each(props, function (value, propId) {
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
            return value.isNull() ? NakedObjects.empty : value.toString();
        }
        //Rest is for scalar fields only:
        if (value.toString()) {
            if (field.entryType() === EntryType.Choices) {
                return renderSingleChoice(field, value);
            }
            else if (field.entryType() === EntryType.MultipleChoices && value.isList()) {
                return renderMultipleChoicesCommaSeparated(field, value);
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
            return NakedObjects.empty;
        }
        else {
            var remoteMask = field.extensions().mask();
            var format = field.extensions().format();
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }
    NakedObjects.renderFieldValue = renderFieldValue;
    function renderSingleChoice(field, value) {
        //This is to handle an enum: render it as text, not a number:  
        var inverted = _.invert(field.choices());
        return inverted[value.toValueString()];
    }
    function renderMultipleChoicesCommaSeparated(field, value) {
        //This is to handle an enum: render it as text, not a number: 
        var inverted = _.invert(field.choices());
        var output = "";
        var values = value.list();
        _.forEach(values, function (v) {
            output += inverted[v.toValueString()] + ",";
        });
        return output;
    }
    function renderCollectionNameAndSize(coll) {
        var output = coll.extensions().friendlyName() + ": ";
        switch (coll.size()) {
            case 0:
                output += NakedObjects.empty;
                break;
            case 1:
                output += "1 " + NakedObjects.item;
                break;
            default:
                output += NakedObjects.numberOfItems(coll.size());
        }
        return output + "\n";
    }
    NakedObjects.renderCollectionNameAndSize = renderCollectionNameAndSize;
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.cicerorenderer.js.map