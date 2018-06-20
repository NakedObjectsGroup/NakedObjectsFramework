import * as Models from './models';
import { PaneRouteData, Pane } from './route-data';
import { ContextService } from './context.service';
import { UrlManagerService } from './url-manager.service';
import { ColorService } from './color.service';
import { ClickHandlerService } from './click-handler.service';
import { ErrorService } from './error.service';
import { MaskService } from './mask.service';
import { Injectable } from '@angular/core';
import { AttachmentViewModel } from './view-models/attachment-view-model';
import { ErrorViewModel } from './view-models/error-view-model';
import { IMessageViewModel } from './view-models/imessage-view-model';
import { LinkViewModel } from './view-models/link-view-model';
import { ItemViewModel } from './view-models/item-view-model';
import { RecentItemViewModel } from './view-models/recent-item-view-model';
import { TableRowColumnViewModel } from './view-models/table-row-column-view-model';
import { TableRowViewModel } from './view-models/table-row-view-model';
import { RecentItemsViewModel } from './view-models/recent-items-view-model';
import { ParameterViewModel } from './view-models/parameter-view-model';
import { ActionViewModel } from './view-models/action-view-model';
import { PropertyViewModel } from './view-models/property-view-model';
import { CollectionViewModel } from './view-models/collection-view-model';
import { MenuViewModel } from './view-models/menu-view-model';
import { MenusViewModel } from './view-models/menus-view-model';
import { Dictionary } from 'lodash';
import { ListViewModel } from './view-models/list-view-model';
import { DialogViewModel } from './view-models/dialog-view-model';
import { DomainObjectViewModel } from './view-models/domain-object-view-model';
import { MultiLineDialogViewModel } from './view-models/multi-line-dialog-view-model';
import { ConfigService } from './config.service';
import { LoggerService } from './logger.service';
import { ApplicationPropertiesViewModel } from './view-models/application-properties-view-model';
import { CiceroCommandFactoryService } from './cicero-command-factory.service';
import { CiceroRendererService } from './cicero-renderer.service';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import find from 'lodash-es/find';

@Injectable()
export class ViewModelFactoryService {

    constructor(
        private readonly context: ContextService,
        private readonly urlManager: UrlManagerService,
        private readonly color: ColorService,
        private readonly error: ErrorService,
        private readonly clickHandler: ClickHandlerService,
        private readonly mask: MaskService,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService,
        private readonly commandFactory: CiceroCommandFactoryService,
        protected ciceroRenderer: CiceroRendererService
    ) { }

    errorViewModel = (error: Models.ErrorWrapper | null) => {
        return new ErrorViewModel(error);
    }

    attachmentViewModel = (propertyRep: Models.PropertyMember, paneId: Pane): AttachmentViewModel | null => {
        const link = propertyRep.attachmentLink();
        if (link) {
            const parent = propertyRep.parent as Models.DomainObjectRepresentation;
            return new AttachmentViewModel(link, parent, this.context, this.error, paneId);
        }
        return null;
    }

    linkViewModel = (linkRep: Models.Link, paneId: Pane) => {
        return new LinkViewModel(this.context, this.color, this.error, this.urlManager, this.configService, linkRep, paneId);
    }

    itemViewModel = (linkRep: Models.Link, paneId: Pane, selected: boolean, index: number, id: string) => {
        return new ItemViewModel(this.context, this.color, this.error, this.urlManager, this.configService, linkRep, paneId, this.clickHandler, this, index, selected, id);
    }

    recentItemViewModel = (obj: Models.DomainObjectRepresentation, linkRep: Models.Link, paneId: Pane, selected: boolean, index: number) => {
        return new RecentItemViewModel(this.context, this.color, this.error, this.urlManager, this.configService, linkRep, paneId, this.clickHandler, this, index, selected, obj.extensions().friendlyName());
    }

    actionViewModel = (actionRep: Models.ActionMember | Models.ActionRepresentation, vm: IMessageViewModel, routeData: PaneRouteData) => {
        return new ActionViewModel(this, this.context, this.urlManager, this.error, this.clickHandler, actionRep, vm, routeData);
    }

    propertyTableViewModel = (id: string, propertyRep?: Models.PropertyMember | Models.CollectionMember) => {
        return propertyRep ? new TableRowColumnViewModel(id, propertyRep, this.mask) : new TableRowColumnViewModel(id);
    }

    propertyViewModel = (propertyRep: Models.PropertyMember, id: string, previousValue: Models.Value, paneId: Pane, parentValues: () => Dictionary<Models.Value>) => {
        return new PropertyViewModel(propertyRep,
            this.color,
            this.error,
            this,
            this.context,
            this.mask,
            this.urlManager,
            this.clickHandler,
            this.configService,
            id,
            previousValue,
            paneId,
            parentValues);
    }

    dialogViewModel = (routeData: PaneRouteData, action: Models.ActionRepresentation | Models.InvokableActionMember, actionViewModel: ActionViewModel | null, isRow: boolean, row?: number) => {

        return new DialogViewModel(this.color,
            this.context,
            this,
            this.urlManager,
            this.error,
            routeData,
            action,
            actionViewModel,
            isRow,
            row);
    }

    multiLineDialogViewModel = (routeData: PaneRouteData,
        action: Models.ActionRepresentation | Models.InvokableActionMember,
        holder: Models.MenuRepresentation | Models.DomainObjectRepresentation | CollectionViewModel) => {

        return new MultiLineDialogViewModel(this.color,
            this.context,
            this,
            this.urlManager,
            this.error,
            routeData,
            action,
            holder);
    }

    domainObjectViewModel = (obj: Models.DomainObjectRepresentation, routeData: PaneRouteData, forceReload: boolean) => {
        const ovm = new DomainObjectViewModel(this.color, this.context, this, this.urlManager, this.error, this.configService, obj, routeData, forceReload);
        if (forceReload) {
            ovm.clearCachedFiles();
        }
        return ovm;
    }

    listViewModel = (list: Models.ListRepresentation, routeData: PaneRouteData) => {
        return new ListViewModel(
            this.color,
            this.context,
            this,
            this.urlManager,
            this.error,
            this.loggerService,
            list,
            routeData
        );
    }

    parameterViewModel = (parmRep: Models.Parameter, previousValue: Models.Value, paneId: Pane) => {
        return new ParameterViewModel(parmRep, paneId, this.color, this.error, this.mask, previousValue, this, this.context, this.configService);
    }

    collectionViewModel = (collectionRep: Models.CollectionMember, routeData: PaneRouteData, forceReload: boolean) => {
        return new CollectionViewModel(this, this.color, this.error, this.context, this.urlManager, this.configService, this.loggerService, collectionRep, routeData, forceReload);
    }

    menuViewModel = (menuRep: Models.MenuRepresentation, routeData: PaneRouteData) => {
        return new MenuViewModel(this, menuRep, routeData);
    }

    menusViewModel = (menusRep: Models.MenusRepresentation, routeData: PaneRouteData) => {
        return new MenusViewModel(this, menusRep, routeData.paneId);
    }

    recentItemsViewModel = (paneId: Pane) => {
        return new RecentItemsViewModel(this, this.context, this.urlManager, paneId);
    }

    tableRowViewModel = (properties: Dictionary<Models.PropertyMember | Models.CollectionMember>, paneId: Pane, title: string): TableRowViewModel => {
        return new TableRowViewModel(this, properties, paneId, title);
    }

    applicationPropertiesViewModel = () => new ApplicationPropertiesViewModel(this.context, this.error, this.configService);

    getItems = (links: Models.Link[], tableView: boolean, routeData: PaneRouteData, listViewModel: ListViewModel | CollectionViewModel) => {

        const collection = listViewModel instanceof CollectionViewModel ? listViewModel : null;
        const id = collection ? collection.name : "";
        const selectedItems = routeData.selectedCollectionItems[id];
        const items = map(links, (link, i) => this.itemViewModel(link, routeData.paneId, selectedItems && selectedItems[i], i, id));

        if (tableView) {

            const getActionExtensions = routeData.objectId
            ? (): Promise<Models.Extensions> =>
                this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator), routeData.actionId)
            : (): Promise<Models.Extensions> => this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

            const getExtensions = listViewModel instanceof CollectionViewModel ? () => Promise.resolve(listViewModel.collectionRep.extensions()) : getActionExtensions;

            // clear existing header
            listViewModel.header = null;

            if (items.length > 0) {
                getExtensions().
                    then((ext: Models.Extensions) => {
                        forEach(items, itemViewModel => {
                            itemViewModel.tableRowViewModel.conformColumns(ext.tableViewTitle(), ext.tableViewColumns());
                        });

                        if (!listViewModel.header) {
                            const firstItem = items[0].tableRowViewModel;

                            const propertiesHeader =
                                map(firstItem.properties, (p, i) => {
                                    // don't know why need cast here - problem in lodash types ?
                                    const match = find(items, (item: ItemViewModel) => item.tableRowViewModel.properties[i].title) as any;
                                    return match ? match.tableRowViewModel.properties[i].title : firstItem.properties[i].id;
                                });

                            listViewModel.header = firstItem.showTitle ? [""].concat(propertiesHeader) : propertiesHeader;
                        }
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            }
        }

        return items;
    }
}
