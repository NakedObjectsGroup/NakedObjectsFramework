import { Component } from '@angular/core';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { PaneComponent } from '../pane/pane';
import * as Models from '../models';
import { ConfigService } from '../config.service';
import { PaneRouteData } from '../route-data';

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

        const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator);

        this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
            .then((object: Models.DomainObjectRepresentation) => {

                const attachmentId = routeData.attachmentId;
                const attachment = object.propertyMember(attachmentId);

                if (attachment) {
                    const avm = this.viewModelFactory.attachmentViewModel(attachment, routeData.paneId);

                    if (avm) {
                        avm.setImage(this);
                    }
                }
            })
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }
}
