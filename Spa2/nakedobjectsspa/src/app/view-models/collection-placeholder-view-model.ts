import { PaneRouteData } from '../route-data';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import * as Models from "../models";

export class CollectionPlaceholderViewModel {

    constructor(
        private context: ContextService,
        private error: ErrorService,
        private routeData: PaneRouteData) {
    }

    // todo string constants in user messages !
    description = () => `Page ${this.routeData.page}`;

    private recreate = () =>
        this.routeData.objectId ?
            // todo can't we just pass routeData ? !
            this.context.getListFromObject(this.routeData.paneId, this.routeData, this.routeData.page, this.routeData.pageSize) :
            this.context.getListFromMenu(this.routeData.paneId, this.routeData, this.routeData.page, this.routeData.pageSize);

    reload = () =>
        this.recreate().
            then(() => {
                // do we need 'then' 
                //$route.reload()
            }).
            catch((reject: Models.ErrorWrapper) => {
                this.error.handleError(reject);
            });
}