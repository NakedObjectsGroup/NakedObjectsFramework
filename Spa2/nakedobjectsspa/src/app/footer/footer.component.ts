import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { GeminiClickDirective } from "../gemini-click.directive";
import { UrlManagerService } from "../url-manager.service";
import { ClickHandlerService } from "../click-handler.service";
import { ContextService } from "../context.service";
import { ErrorService } from "../error.service";
import { RepLoaderService } from "../rep-loader.service";
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { IMessageViewModel } from '../view-models/imessage-view-model';
import * as Msg from "../user-messages";
import * as Config from "../config";
import * as Models from "../models";

@Component({
    selector: 'footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {

    constructor(private urlManager: UrlManagerService,
        private context: ContextService,
        private clickHandler: ClickHandlerService,
        private error: ErrorService,
        private repLoader: RepLoaderService,
        private location: Location) {
    }

    loading: string;
    template: string;
    footerTemplate: string;

    goHome = (right?: boolean) => {
        this.context.updateValues();
        this.urlManager.setHome(this.clickHandler.pane(1, right));
    };
    goBack = () => {
        this.context.updateValues();
        this.location.back();
    };
    goForward = () => {
        this.context.updateValues();
        this.location.forward();
    };

    swapPanes = () => {
        if (!this.swapDisabled()) {
            this.context.updateValues();
            this.context.swapCurrentObjects();
            this.urlManager.swapPanes();
        }
    };

    swapDisabled = () => {
        return this.urlManager.isMultiLineDialog();
    }

    singlePane = (right?: boolean) => {
        this.context.updateValues();
        this.urlManager.singlePane(this.clickHandler.pane(1, right));
    };
    logOff = () => {
        this.context.getUser()
            .then((u: Models.UserRepresentation) => {
                if (window.confirm(Msg.logOffMessage(u.userName() || "Unknown"))) {
                    const config = {
                        withCredentials: true,
                        url: Config.logoffUrl,
                        method: "POST",
                        cache: false
                    };

                    // logoff server
                    //$http(config);

                    // logoff client without waiting for server
                    //$rootScope.$broadcast(Nakedobjectsconstants.geminiLogoffEvent);
                    //$timeout(() => window.location.href = Nakedobjectsconfig.postLogoffUrl);
                }
            })
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    };

    applicationProperties = () => {
        this.context.updateValues();
        this.urlManager.applicationProperties();
    };

    recent = (right?: boolean) => {
        this.context.updateValues();
        this.urlManager.setRecent(this.clickHandler.pane(1, right));
    };

    cicero = () => {
        this.context.updateValues();
        this.urlManager.singlePane(this.clickHandler.pane(1));
        this.urlManager.cicero();
    };
    userName: string;

    warnings: string[];
    messages: string[];

    copyViewModel: IDraggableViewModel;

    get currentCopyColor() {
        return this.copyViewModel.color;
    }

    get currentCopyTitle() {
        return this.copyViewModel.draggableTitle();
    }


    ngOnInit() {
        this.context.getUser().then((user: Models.UserRepresentation) => this.userName = user.userName()).catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.repLoader.loadingCount$.subscribe((count: any) => this.loading = count > 0 ? Msg.loadingMessage : "");

        this.context.warning$.subscribe((ws: any) =>
            this.warnings = ws);

        this.context.messages$.subscribe((ms: any) =>
            this.messages = ms);

        this.context.cutViewModel$.subscribe((cvm: any) =>
            this.copyViewModel = cvm);
    }
}