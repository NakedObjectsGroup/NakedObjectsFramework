import { Component, OnInit } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import * as ViewModels from "../view-models";
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';
import { UrlManagerService } from "../url-manager.service";
import { RouteData, PaneRouteData } from "../route-data";
import * as Models from "../models";
import { ErrorService } from "../error.service";
import { AttachmentViewModel } from '../view-models/attachment-view-model';



@Component({
    selector: 'attachment',
    templateUrl: './attachment.component.html',
    styleUrls: ['./attachment.component.css']
})
export class AttachmentComponent implements OnInit {

    constructor(private activatedRoute: ActivatedRoute,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private context: ContextService,
        private error: ErrorService
    ) {
    }

    paneId: number;
    vm: AttachmentViewModel;

    paneType: string;

    onChild() {
        this.paneType = "split";
    }

    onChildless() {
        this.paneType = "single";
    }

    paneIdName = () => this.paneId === 1 ? "pane1" : "pane2";

    getAttachment(routeData: PaneRouteData) {
        // context.clearWarnings();
        // context.clearMessages();

        const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId);


        this.context.getObject(routeData.paneId, oid, routeData.interactionMode)
            .then((object: Models.DomainObjectRepresentation) => {

                const attachmentId = routeData.attachmentId;
                const attachment = object.propertyMember(attachmentId);

                if (attachment && attachment.attachmentLink()) {
                    const avm = this.viewModelFactory.attachmentViewModel(attachment, routeData.paneId);
                    this.vm = avm;

                    this.attachmentTitle = avm.title;

                    avm.downloadFile()
                        .then(blob => {
                            const reader = new FileReader();
                            reader.onloadend = () => this.image = reader.result;
                            reader.readAsDataURL(blob);
                        })
                        .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
                }
            })
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: any) => {
            this.paneId = data["pane"];
            this.paneType = data["class"];
        });

        this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
            .subscribe((rd: RouteData) => {
                if (this.paneId) {
                    const paneRouteData = rd.pane()[this.paneId];
                    this.getAttachment(paneRouteData);
                }
            });
    }

    ngOnDestroy(): void {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    }

    attachmentTitle: string;
    image: string;
}