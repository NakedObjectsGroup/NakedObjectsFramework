import { PaneRouteData } from '../route-data';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import * as Models from "../models";

export class CollectionPlaceholderViewModel {

    constructor(
        private readonly context: ContextService,
        private readonly error: ErrorService,
        private readonly routeData: PaneRouteData) {
    }

    // todo string constants in user messages !
    description = () => `Page ${this.routeData.page}`;

    private recreate = () =>
        this.routeData.objectId ? this.context.getListFromObject(this.routeData) : this.context.getListFromMenu(this.routeData);

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