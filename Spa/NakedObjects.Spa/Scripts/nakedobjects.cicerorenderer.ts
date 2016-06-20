/// <reference path="nakedobjects.app.ts" />

module NakedObjects {
    import MenuRepresentation = Models.MenuRepresentation;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ListRepresentation = Models.ListRepresentation;
    import ErrorRepresentation = Models.ErrorRepresentation;
    import IHasActions = Models.IHasActions;
    import TypePlusTitle = Models.typePlusTitle;
    import PlusTitle = Models.typePlusTitle;
    import FriendlyNameForParam = Models.friendlyNameForParam;
    import ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    import InvokableActionMember = NakedObjects.Models.InvokableActionMember;
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

    export interface ICiceroRenderer {

        renderHome(cvm: CiceroViewModel, routeData: PaneRouteData): void;
        renderObject(cvm: CiceroViewModel, routeData: PaneRouteData): void;
        renderList(cvm: CiceroViewModel, routeData: PaneRouteData): void;
        renderError(cvm: CiceroViewModel): void;
    }

    app.service("ciceroRenderer", function ($q: ng.IQService,
        context: IContext,
        mask: IMask,
        error: IError) {
        const renderer = <ICiceroRenderer>this;
        renderer.renderHome = (cvm: CiceroViewModel, routeData: PaneRouteData) => {
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
        renderer.renderObject = (cvm: CiceroViewModel, routeData: PaneRouteData) => {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            } else {
                const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);

                context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                    .then((obj: DomainObjectRepresentation) => {
                        let output = "";
                        const openCollIds = openCollectionIds(routeData);
                        if (_.some(openCollIds)) {
                            const id = openCollIds[0];
                            const coll = obj.collectionMember(id);
                            output += `Collection: ${coll.extensions().friendlyName()} on ${TypePlusTitle(obj)}\n`;
                            switch (coll.size()) {
                                case 0:
                                    output += "empty";
                                    break;
                                case 1:
                                    output += "1 item";
                                    break;
                                default:
                                    output += `${coll.size()} items`;
                            }
                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                        } else {
                            if (obj.isTransient()) {
                                output += "Unsaved ";
                                output += obj.extensions().friendlyName() + "\n";
                                output += renderModifiedProperties(obj, routeData, mask);
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                            } else if (routeData.interactionMode === InteractionMode.Edit ||
                                routeData.interactionMode === InteractionMode.Form) {
                                let output = "Editing ";
                                output += PlusTitle(obj) + "\n";
                                if (routeData.dialogId) {
                                    context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                        .then((details: IInvokableAction) => {
                                            output += renderActionDialog(details, routeData, mask);
                                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                        });
                                } else {
                                    output += renderModifiedProperties(obj, routeData, mask);
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                }
                            } else {
                                let output = Title(obj) + "\n";
                                if (routeData.dialogId) {
                                    context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                        .then((details: IInvokableAction) => {
                                            output += renderActionDialog(details, routeData, mask);
                                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                        });
                                } else {
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                }

                            }
                        }
                    }).catch((reject: ErrorWrapper) => {
                        if (reject.category === ErrorCategory.ClientError && reject.clientErrorCode === ClientErrorCode.ExpiredTransient) {
                            cvm.output = "The requested view of unsaved object details has expired";
                        } else {
                            error.handleError(reject);
                        }
                    });
            }
        };
        renderer.renderList = (cvm: CiceroViewModel, routeData: PaneRouteData) => {
            if (cvm.message) {
                cvm.outputMessageThenClearIt();
            } else {
                const listPromise = context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
                listPromise.then((list: ListRepresentation) => {
                    context.getMenu(routeData.menuId).then((menu: MenuRepresentation) => {
                        const count = list.value().length;
                        const numPages = list.pagination().numPages;
                        let description: string;
                        if (numPages > 1) {
                            const page = list.pagination().page;
                            const totalCount = list.pagination().totalCount;
                            description = `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
                        } else {
                            description = `${count} items`;
                        }
                        const actionMember = menu.actionMember(routeData.actionId);
                        const actionName = actionMember.extensions().friendlyName();
                        const output = `Result from ${actionName}:\n${description}`;
                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                    });
                });
            }
        };
        renderer.renderError = (cvm: CiceroViewModel) => {
            const err = context.getError().error as ErrorRepresentation;
            cvm.clearInput();
            cvm.output = `Sorry, an application error has occurred. ${err.message()}`;
        };

        //Returns collection Ids for any collections on an object that are currently in List or Table mode
        function openCollectionIds(routeData: PaneRouteData): string[] {
            return _.filter(_.keys(routeData.collections), k => routeData.collections[k] != CollectionViewState.Summary);
        }

        function renderOpenMenu(routeData: PaneRouteData, cvm: CiceroViewModel) {
            var output = "";
            context.getMenu(routeData.menuId)
                .then((menu: MenuRepresentation) => {
                    output += menu.title() + " menu" + "\n";
                    return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null);
                }).then((details: IInvokableAction) => {
                    if (details) {
                        output += renderActionDialog(details, routeData, mask);
                    }
                }).finally(() => {
                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                });
        }

        function renderActionDialog(
            invokable: Models.IInvokableAction,
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
            if (_.keys(routeData.props).length > 0) {
                output += "Modified properties:\n";
                _.each(routeData.props, (value, propId) => {
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
            return value.isNull() ? "empty" : value.toString();
        }
        //Rest is for scalar fields only:
        if (value.toString()) { //i.e. not empty
            //This is to handle an enum: render it as text, not a number:           
            if (field.entryType() === EntryType.Choices) {
                const inverted = _.invert(field.choices());
                return (<any>inverted)[value.toValueString()];
            } else if (field.entryType() === EntryType.MultipleChoices && value.isList()) {
                const inverted = _.invert(field.choices());
                let output = "";
                const values = value.list();
                _.forEach(values, v => {
                    output += (<any>inverted)[v.toValueString()] + ",";
                });
                return output;
            }
        }
        let properScalarValue: number | string | boolean | Date;
        if (isDateOrDateTime(field)) {
            properScalarValue = toUtcDate(value);
        } else {
            properScalarValue = value.scalar();
        }
        if (properScalarValue === "" || properScalarValue == null) {
            return "empty";
        } else {
            const remoteMask = field.extensions().mask();
            const format = field.extensions().format();
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }
}
