import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as Ro from '@nakedobjects/restful-objects';
import { ConfigService, ContextService, ErrorService, ErrorWrapper, PaneRouteData, UrlManagerService } from '@nakedobjects/services';
import { ViewModelFactoryService } from '@nakedobjects/view-models';
import { PaneComponent } from '../pane/pane';

@Component({
    selector: 'nof-attachment',
    templateUrl: 'attachment.component.html',
    styleUrls: ['attachment.component.css']
})
export class AttachmentComponent extends PaneComponent {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        private readonly viewModelFactory: ViewModelFactoryService,
        context: ContextService,
        private readonly error: ErrorService,
        private readonly configService: ConfigService
    ) {
        super(activatedRoute, urlManager, context);
    }

    // template API
    image: string;
    title: string;

    protected setup(routeData: PaneRouteData) {

        const oid = Ro.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator);

        this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
            .then((object: Ro.DomainObjectRepresentation) => {

                const attachmentId = routeData.attachmentId;
                const attachment = object.propertyMember(attachmentId);

                if (attachment) {
                    const avm = this.viewModelFactory.attachmentViewModel(attachment, routeData.paneId);

                    if (avm) {
                        avm.setImage(this);
                    }
                }
            })
            .catch((reject: ErrorWrapper) => this.error.handleError(reject));
    }
}
