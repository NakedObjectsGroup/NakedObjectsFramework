import * as Routedata from '../route-data';
import * as Contextservice from '../context.service';
import * as Errorservice from '../error.service';
import * as Models from "../models";


export class CollectionPlaceholderViewModel {

    constructor(private context: Contextservice.ContextService, private error: Errorservice.ErrorService, private routeData: Routedata.PaneRouteData) {

    }

    description = () => `Page ${this.routeData.page}`;

    private recreate = () =>
        this.routeData.objectId ?
            // todo can't we just pass routeData ? !
            this.context.getListFromObject(this.routeData.paneId, this.routeData, this.routeData.page, this.routeData.pageSize) :
            this.context.getListFromMenu(this.routeData.paneId, this.routeData, this.routeData.page, this.routeData.pageSize);


    reload = () =>
        this.recreate().
            then(() => {
                //$route.reload()
            }).
            catch((reject: Models.ErrorWrapper) => {
                this.error.handleError(reject);
            });
}