/// <reference path="nakedobjects.app.ts" />

namespace NakedObjects {
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ListRepresentation = Models.ListRepresentation;
    import ErrorRepresentation = Models.ErrorRepresentation;
    import TypePlusTitle = Models.typePlusTitle;
    import PlusTitle = Models.typePlusTitle;
    import FriendlyNameForParam = Models.friendlyNameForParam;
    import ObjectIdWrapper = Models.ObjectIdWrapper;
    import IInvokableAction = Models.IInvokableAction;
    import Title = Models.typePlusTitle;
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;
    import FriendlyNameForProperty = Models.friendlyNameForProperty;
    import IField = Models.IField;
    import Value = Models.Value;
    import EntryType = Models.EntryType;
    import toUtcDate = Models.toUtcDate;
    import isDateOrDateTime = Models.isDateOrDateTime;
    import CollectionMember = Models.CollectionMember;

    export interface ICiceroRenderer {

        renderHome(cvm: ICiceroViewModel, routeData: PaneRouteData): void;
        renderObject(cvm: ICiceroViewModel, routeData: PaneRouteData): void;
        renderList(cvm: ICiceroViewModel, routeData: PaneRouteData): void;
        renderError(cvm: ICiceroViewModel): void;
    }

    app.service("ciceroRenderer", function ($q: ng.IQService,
        context: IContext,
        mask: IMask,
        error: IError) {
        const renderer = <ICiceroRenderer>this;
        renderer.renderHome = (cvm: ICiceroViewModel, routeData: PaneRouteData) => {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            } else {
                if (routeData.menuId) {
                    renderOpenMenu(routeData, cvm);
                } else {
                    cvm.clearInput();
                    cvm.output = welcomeMessage;
                }
            }
        };
        renderer.renderObject = (cvm: ICiceroViewModel, routeData: PaneRouteData) => {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            } else {
                const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
                context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                    .then((obj: DomainObjectRepresentation) => {
                        const openCollIds = openCollectionIds(routeData);
                        if (_.some(openCollIds)) {
                            renderOpenCollection(openCollIds[0], obj, cvm);
                        } else if (obj.isTransient()) {
                            renderTransientObject(routeData, obj, cvm);
                        } else if (routeData.interactionMode === InteractionMode.Edit ||
                            routeData.interactionMode === InteractionMode.Form) {
                            renderForm(routeData, obj, cvm);
                        } else {
                            renderObjectTitleAndDialogIfOpen(routeData, obj, cvm);
                        }
                    }).catch((reject: ErrorWrapper) => {
                        //TODO: Is the first test necessary or would this be rendered OK by generic error handling?
                        if (reject.category === ErrorCategory.ClientError && reject.clientErrorCode === ClientErrorCode.ExpiredTransient) {
                            cvm.output = errorExpiredTransient;
                        } else {
                            error.handleError(reject);
                        }
                    });
            }
        };

        renderer.renderList = (cvm: ICiceroViewModel, routeData: PaneRouteData) => {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            } else {
                const listPromise = context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
                listPromise.
                    then((list: ListRepresentation) => {
                        context.getMenu(routeData.menuId).
                            then(menu => {
                                const count = list.value().length;
                                const numPages = list.pagination().numPages;
                                const description = getListDescription(numPages, list, count);
                                const actionMember = menu.actionMember(routeData.actionId);
                                const actionName = actionMember.extensions().friendlyName();
                                const output = `Result from ${actionName}:\n${description}`;
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                            }).
                            catch((reject: ErrorWrapper) => this.error.handleError(reject));
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
        };

        renderer.renderError = (cvm: ICiceroViewModel) => {
            const err = context.getError().error as ErrorRepresentation;
            cvm.clearInput();
            cvm.output = `Sorry, an application error has occurred. ${err.message()}`;
        };

        function getListDescription(numPages: number, list: ListRepresentation, count: number) {
            if (numPages > 1) {
                const page = list.pagination().page;
                const totalCount = list.pagination().totalCount;
                return `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
            } else {
                return `${count} items`;
            }
        }

        //Returns collection Ids for any collections on an object that are currently in List or Table mode
        function openCollectionIds(routeData: PaneRouteData): string[] {
            return _.filter(_.keys(routeData.collections), k => routeData.collections[k] != CollectionViewState.Summary);
        }

        function renderOpenCollection(collId: string, obj: DomainObjectRepresentation, cvm: ICiceroViewModel) {
            const coll = obj.collectionMember(collId);
            let output = renderCollectionNameAndSize(coll);
             output += `(${collection} ${on} ${TypePlusTitle(obj)})`;
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }
        
        function renderTransientObject(routeData: PaneRouteData, obj: DomainObjectRepresentation, cvm: ICiceroViewModel) {
            var output = `${unsaved} `;
            output += obj.extensions().friendlyName() + "\n";
            output += renderModifiedProperties(obj, routeData, mask);
            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
        }

        function renderForm(routeData: PaneRouteData, obj: DomainObjectRepresentation, cvm: ICiceroViewModel) {
            let output = `${editing} `;
            output += PlusTitle(obj) + "\n";
            if (routeData.dialogId) {
                context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                    then(invokableAction => {
                        output += renderActionDialog(invokableAction, routeData, mask);
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            } else {
                output += renderModifiedProperties(obj, routeData, mask);
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }
        }

        function renderObjectTitleAndDialogIfOpen(routeData: PaneRouteData, obj: DomainObjectRepresentation, cvm: ICiceroViewModel) {
            let output = Title(obj) + "\n";
            if (routeData.dialogId) {
                context.getInvokableAction(obj.actionMember(routeData.dialogId)).
                    then(invokableAction => {
                        output += renderActionDialog(invokableAction, routeData, mask);
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            } else {
                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
            }
        }

        function renderOpenMenu(routeData: PaneRouteData, cvm: ICiceroViewModel) {
            var output = "";
            context.getMenu(routeData.menuId).
                then(menu => {
                    output += menuTitle(menu.title());
                    return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null) as ng.IPromise<IInvokableAction>;
                }).
                then(invokableAction => {
                    if (invokableAction) {
                        output += `\n${renderActionDialog(invokableAction, routeData, mask)}`;
                    }
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject)).
                finally(() => cvm.clearInputRenderOutputAndAppendAlertIfAny(output));
        }

        function renderActionDialog(invokable: Models.IInvokableAction,
                                    routeData: PaneRouteData,
                                    mask: IMask): string {
            const actionName = invokable.extensions().friendlyName();
            let output = `Action dialog: ${actionName}\n`;
            _.forEach(getParametersAndCurrentValue(invokable, context), (value, paramId) => {
                output += FriendlyNameForParam(invokable, paramId) + ": ";
                const param = invokable.parameters()[paramId];
                output += renderFieldValue(param, value, mask);
                output += "\n";
            });
            return output;
        }

        function renderModifiedProperties(obj: DomainObjectRepresentation, routeData: PaneRouteData, mask: IMask): string {
            let output = "";
            const props = context.getCurrentObjectValues(obj.id());
            if (_.keys(props).length > 0) {
                output += modifiedProperties + ":\n";
                _.each(props, (value, propId) => {
                    output += FriendlyNameForProperty(obj, propId) + ": ";
                    const pm = obj.propertyMember(propId);
                    output += renderFieldValue(pm, value, mask);
                    output += "\n";
                });
            }
            return output;
        }
    });

    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    export function openCollectionIds(routeData: PaneRouteData): string[] {
        return _.filter(_.keys(routeData.collections), k => routeData.collections[k] !== CollectionViewState.Summary);
    }

    //Handles empty values, and also enum conversion
    export function renderFieldValue(field: IField, value: Value, mask: IMask): string {
        if (!field.isScalar()) { //i.e. a reference
            return value.isNull() ? empty : value.toString();
        }
        //Rest is for scalar fields only:
        if (value.toString()) { //i.e. not empty        
            if (field.entryType() === EntryType.Choices) {
                return renderSingleChoice(field, value);
            } else if (field.entryType() === EntryType.MultipleChoices && value.isList()) {
                return renderMultipleChoicesCommaSeparated(field, value);
            }
        }
        let properScalarValue: number | string | boolean | Date;
        if (isDateOrDateTime(field)) {
            properScalarValue = toUtcDate(value);
        } else {
            properScalarValue = value.scalar();
        }
        if (properScalarValue === "" || properScalarValue == null) {
            return empty;
        } else {
            const remoteMask = field.extensions().mask();
            const format = field.extensions().format();
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }

    function renderSingleChoice(field: IField, value: Value) {
        //This is to handle an enum: render it as text, not a number:  
        const inverted = _.invert(field.choices());
        return (<any>inverted)[value.toValueString()];
    }

    function renderMultipleChoicesCommaSeparated(field: IField, value: Value) {
        //This is to handle an enum: render it as text, not a number: 
        const inverted = _.invert(field.choices());
        let output = "";
        const values = value.list();
        _.forEach(values, v => {
            output += (<any>inverted)[v.toValueString()] + ",";
        });
        return output;
    }

    export function renderCollectionNameAndSize(coll: CollectionMember): string {
        let output: string = coll.extensions().friendlyName() + ": ";
        switch (coll.size()) {
            case 0:
                output += empty;
                break;
            case 1:
                output += `1 ${item}`;
                break;
            default:
                output += numberOfItems(coll.size());
        }
        return output + "\n";
    }
}
