import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import {
    ClickHandlerService,
    ColorService,
    ConfigService,
    ContextService,
    ErrorService,
    ErrorWrapper,
    LoggerService,
    MaskService,
    Pane,
    PaneRouteData,
    UrlManagerService
} from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import find from 'lodash-es/find';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import { ActionViewModel } from './action-view-model';
import { ApplicationPropertiesViewModel } from './application-properties-view-model';
import { AttachmentViewModel } from './attachment-view-model';
import { CollectionViewModel } from './collection-view-model';
import { DialogViewModel } from './dialog-view-model';
import { DomainObjectViewModel } from './domain-object-view-model';
import { ErrorViewModel } from './error-view-model';
import { IMessageViewModel } from './imessage-view-model';
import { ItemViewModel } from './item-view-model';
import { LinkViewModel } from './link-view-model';
import { ListViewModel } from './list-view-model';
import { MenuViewModel } from './menu-view-model';
import { MenusViewModel } from './menus-view-model';
import { MultiLineDialogViewModel } from './multi-line-dialog-view-model';
import { ParameterViewModel } from './parameter-view-model';
import { PropertyViewModel } from './property-view-model';
import { RecentItemViewModel } from './recent-item-view-model';
import { RecentItemsViewModel } from './recent-items-view-model';
import { TableRowColumnViewModel } from './table-row-column-view-model';
import { TableRowViewModel } from './table-row-view-model';

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
        private readonly loggerService: LoggerService
    ) { }

    errorViewModel = (error: ErrorWrapper | null) => {
        return new ErrorViewModel(error);
    }

    attachmentViewModel = (propertyRep: Ro.PropertyMember, paneId: Pane): AttachmentViewModel | null => {
        const link = propertyRep.attachmentLink();
        if (link) {
            const parent = propertyRep.parent as Ro.DomainObjectRepresentation;
            return new AttachmentViewModel(link, parent, this.context, this.error, paneId);
        }
        return null;
    }

    linkViewModel = (linkRep: Ro.Link, paneId: Pane) => {
        return new LinkViewModel(this.context, this.color, this.error, this.urlManager, this.configService, linkRep, paneId);
    }

    itemViewModel = (linkRep: Ro.Link, paneId: Pane, selected: boolean, index: number, id: string) => {
        return new ItemViewModel(this.context, this.color, this.error, this.urlManager, this.configService, linkRep, paneId, this.clickHandler, this, index, selected, id);
    }

    recentItemViewModel = (obj: Ro.DomainObjectRepresentation, linkRep: Ro.Link, paneId: Pane, selected: boolean, index: number) => {
        return new RecentItemViewModel(this.context, this.color, this.error, this.urlManager, this.configService, linkRep, paneId, this.clickHandler, this, index, selected, obj.extensions().friendlyName());
    }

    actionViewModel = (actionRep: Ro.ActionMember | Ro.ActionRepresentation, vm: IMessageViewModel, routeData: PaneRouteData) => {
        return new ActionViewModel(this, this.context, this.urlManager, this.error, this.clickHandler, actionRep, vm, routeData);
    }

    propertyTableViewModel = (id: string, propertyRep?: Ro.PropertyMember | Ro.CollectionMember) => {
        return propertyRep ? new TableRowColumnViewModel(id, propertyRep, this.mask) : new TableRowColumnViewModel(id);
    }

    propertyViewModel = (propertyRep: Ro.PropertyMember, id: string, previousValue: Ro.Value, paneId: Pane, parentValues: () => Dictionary<Ro.Value>) => {
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

    dialogViewModel = (routeData: PaneRouteData, action: Ro.ActionRepresentation | Ro.InvokableActionMember, actionViewModel: ActionViewModel | null, isRow: boolean, row?: number) => {

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
        action: Ro.ActionRepresentation | Ro.InvokableActionMember,
        holder: Ro.MenuRepresentation | Ro.DomainObjectRepresentation | CollectionViewModel) => {

        return new MultiLineDialogViewModel(this.color,
            this.context,
            this,
            this.urlManager,
            this.error,
            routeData,
            action,
            holder);
    }

    domainObjectViewModel = (obj: Ro.DomainObjectRepresentation, routeData: PaneRouteData, forceReload: boolean) => {
        const ovm = new DomainObjectViewModel(this.color, this.context, this, this.urlManager, this.error, this.configService, obj, routeData, forceReload);
        if (forceReload) {
            ovm.clearCachedFiles();
        }
        return ovm;
    }

    listViewModel = (list: Ro.ListRepresentation, routeData: PaneRouteData) => {
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

    parameterViewModel = (parmRep: Ro.Parameter, previousValue: Ro.Value, paneId: Pane) => {
        return new ParameterViewModel(parmRep, paneId, this.color, this.error, this.mask, previousValue, this, this.context, this.configService);
    }

    collectionViewModel = (collectionRep: Ro.CollectionMember, routeData: PaneRouteData, forceReload: boolean) => {
        return new CollectionViewModel(this, this.color, this.error, this.context, this.urlManager, this.configService, this.loggerService, collectionRep, routeData, forceReload);
    }

    menuViewModel = (menuRep: Ro.MenuRepresentation, routeData: PaneRouteData) => {
        return new MenuViewModel(this, menuRep, routeData);
    }

    menusViewModel = (menusRep: Ro.MenusRepresentation, routeData: PaneRouteData) => {
        return new MenusViewModel(this, menusRep, routeData.paneId);
    }

    recentItemsViewModel = (paneId: Pane) => {
        return new RecentItemsViewModel(this, this.context, this.urlManager, paneId);
    }

    tableRowViewModel = (properties: Dictionary<Ro.PropertyMember | Ro.CollectionMember>, paneId: Pane, title: string): TableRowViewModel => {
        return new TableRowViewModel(this, properties, paneId, title);
    }

    applicationPropertiesViewModel = () => new ApplicationPropertiesViewModel(this.context, this.error, this.configService);

    getItems = (links: Ro.Link[], tableView: boolean, routeData: PaneRouteData, listViewModel: ListViewModel | CollectionViewModel) => {

        const collection = listViewModel instanceof CollectionViewModel ? listViewModel : null;
        const id = collection ? collection.name : '';
        const selectedItems = routeData.selectedCollectionItems[id];
        const items = map(links, (link, i) => this.itemViewModel(link, routeData.paneId, selectedItems && selectedItems[i], i, id));

        if (tableView) {

            const getActionExtensions = routeData.objectId
                ? (): Promise<Ro.Extensions> =>
                    this.context.getActionExtensionsFromObject(routeData.paneId, Ro.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator), routeData.actionId)
                : (): Promise<Ro.Extensions> => this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

            const getExtensions = listViewModel instanceof CollectionViewModel ? () => Promise.resolve(listViewModel.collectionRep.extensions()) : getActionExtensions;

            // clear existing header
            listViewModel.header = null;

            if (items.length > 0) {
                getExtensions().
                    then((ext: Ro.Extensions) => {
                        forEach(items, itemViewModel => {
                            itemViewModel.tableRowViewModel.conformColumns(ext.tableViewTitle(), ext.tableViewColumns());
                        });

                        if (!listViewModel.header) {
                            const firstItem = items[0].tableRowViewModel;

                            const propertiesHeader =
                                map(firstItem.properties, (p, i) => {
                                    const match = find(items, item => !!item.tableRowViewModel.properties[i].title);
                                    return match ? match.tableRowViewModel.properties[i].title : firstItem.properties[i].id;
                                });

                            listViewModel.header = firstItem.showTitle ? [''].concat(propertiesHeader) : propertiesHeader;
                        }
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
        }

        return items;
    }
}
