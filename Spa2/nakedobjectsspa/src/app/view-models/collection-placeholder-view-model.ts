import { PaneRouteData } from '../route-data';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import * as Models from '../models';
import * as Msg from '../user-messages';

export class CollectionPlaceholderViewModel {

    constructor(
        private readonly context: ContextService,
        private readonly error: ErrorService,
        private readonly routeData: PaneRouteData) {
    }

    private readonly description = () => `${Msg.page} ${this.routeData.page}`;

    private readonly recreate = () =>
        this.routeData.objectId ? this.context.getListFromObject(this.routeData) : this.context.getListFromMenu(this.routeData);

    private readonly reload = () =>
        this.recreate().
            then(() => {
                // todo do we need 'then' 
                //$route.reload()
            }).
            catch((reject: Models.ErrorWrapper) => {
                this.error.handleError(reject);
            });
}