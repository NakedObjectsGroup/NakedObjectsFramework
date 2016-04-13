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

    export interface ICiceroRenderer {

        renderHome(routeData: PaneRouteData, cvm: CiceroViewModel): void;
        renderObject(routeData: PaneRouteData, cvm: CiceroViewModel): void;
        renderList(routeData: PaneRouteData, cvm: CiceroViewModel): void;
        renderError(cvm: CiceroViewModel): void;
    }

    app.service("ciceroRenderer", function(
        context: IContext) {
        const renderer = <ICiceroRenderer>this;
        renderer.renderHome = (routeData: PaneRouteData, cvm: CiceroViewModel) => {
            if (routeData.menuId) {
                context.getMenu(routeData.menuId)
                    .then((menu: MenuRepresentation) => {
                        let output = ""; //TODO: use builder
                        output += menu.title() + " menu" + ". ";
                        output += renderActionDialogIfOpen(menu, routeData);
                        cvm.clearInput();
                        cvm.output = output;
                    });
            } else {
                cvm.clearInput();
                cvm.output = "Welcome to Cicero";
            }

        };
        renderer.renderObject = (routeData: PaneRouteData, cvm: CiceroViewModel) => {
            const [domainType, ...id] = routeData.objectId.split(keySeparator);
            context.getObject(1, domainType, id, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                .then((obj: DomainObjectRepresentation) => {
                    let output = "";
                    const openCollIds = openCollectionIds(routeData);
                    if (_.some(openCollIds)) {
                        const id = openCollIds[0];
                        const coll = obj.collectionMember(id);
                        output += `Collection: ${coll.extensions().friendlyName()}`;
                        output += ` on ${TypePlusTitle(obj)},  `;
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
                    } else {
                        if (routeData.interactionMode === InteractionMode.Edit) {
                            output += "Editing ";
                        }
                        output += PlusTitle(obj) + ". ";
                        output += renderActionDialogIfOpen(obj, routeData);
                    }
                    cvm.clearInput();
                    cvm.output = output;
                });
        };
        renderer.renderList = (routeData: PaneRouteData, cvm: CiceroViewModel) => {
            const listPromise = context.getListFromMenu(1, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.state, routeData.page, routeData.pageSize);
            listPromise.then((list: ListRepresentation) => {
                const page = list.pagination().page;
                const numPages = list.pagination().numPages;
                const count = list.value().length;
                const totalCount = list.pagination().totalCount;
                const description = `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
                context.getMenu(routeData.menuId).then((menu: MenuRepresentation) => {
                    const actionMember = menu.actionMember(routeData.actionId);
                    const actionName = actionMember.extensions().friendlyName();
                    cvm.clearInput();
                    cvm.output = `Result from ${actionName}: ${description}`;
                });
                //TODO: add number of items selected, if any.
            });
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

        function renderActionDialogIfOpen(
            repWithActions: IHasActions,
            routeData: PaneRouteData): string {
            let output = "";
            if (routeData.dialogId) {
                const actionMember = repWithActions.actionMember(routeData.dialogId);
                const actionName = actionMember.extensions().friendlyName();
                output += `Action dialog: ${actionName}. `;
                _.forEach(routeData.dialogFields, (value, key) => {
                    output += FriendlyNameForParam(actionMember, key) + ": ";
                    output += value.toString() || "empty";
                    output += ", ";
                });
            }
            return output;
        }
    });
}
//Code to go in ViewModelFactory
//cvm.renderHome = _.partial(ciceroRenderer.renderHome, cvm) as (routeData: PaneRouteData) => void;
//cvm.renderObject = _.partial(ciceroRenderer.renderObject, cvm) as (routeData: PaneRouteData) => void;
//cvm.renderList = _.partial(ciceroRenderer.renderList, cvm) as (routeData: PaneRouteData) => void;
//cvm.renderError = _.partial(ciceroRenderer.renderError, cvm);