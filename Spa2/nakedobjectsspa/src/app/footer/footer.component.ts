import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Http, RequestOptionsArgs } from '@angular/http';
import { GeminiClickDirective } from '../gemini-click.directive';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from '../click-handler.service';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import { RepLoaderService } from '../rep-loader.service';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { IMessageViewModel } from '../view-models/imessage-view-model';
import * as Msg from '../user-messages';
import * as Models from '../models';
import { ConfigService } from '../config.service';

@Component({
    selector: 'nof-footer',
    template: require('./footer.component.html'),
    styles: [require('./footer.component.css')]
})
export class FooterComponent implements OnInit {

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly clickHandler: ClickHandlerService,
        private readonly error: ErrorService,
        private readonly repLoader: RepLoaderService,
        private readonly location: Location,
        private readonly configService: ConfigService,
        private readonly http: Http
    ) { }

    loading: string;
    template: string;
    footerTemplate: string;

    goHome = (right?: boolean) => {
        const newPane = this.clickHandler.pane(1, right);

        if (this.configService.config.leftClickHomeAlwaysGoesToSinglePane && newPane === 1) {
            this.urlManager.setHomeSinglePane();
        } else {
            this.urlManager.setHome(newPane);
        }
    };
    goBack = () => {
        this.location.back();
    };
    goForward = () => {
        this.location.forward();
    };

    swapPanes = () => {
        if (!this.swapDisabled()) {
            this.context.swapCurrentObjects();
            this.urlManager.swapPanes();
        }
    };

    swapDisabled = () => {
        return this.urlManager.isMultiLineDialog() ? true : null;
    }

    singlePane = (right?: boolean) => {
        this.urlManager.singlePane(this.clickHandler.pane(1, right));
    };

    logOff = () => {
        this.context.getUser()
            .then((u: Models.UserRepresentation) => {
                if (window.confirm(Msg.logOffMessage(u.userName() || "Unknown"))) {

                    const args: RequestOptionsArgs = {
                        withCredentials: true               
                    };

                    // logoff server
                    this.http.post(this.configService.config.logoffUrl, args);

                    // logoff client without waiting for server
                    window.location.href = this.configService.config.postLogoffUrl;
                }
            })
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    };

    applicationProperties = () => {
        this.urlManager.applicationProperties();
    };

    recent = (right?: boolean) => {
        this.urlManager.setRecent(this.clickHandler.pane(1, right));
    };

    cicero = () => {
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

        this.context.copiedViewModel$.subscribe((cvm: any) =>
            this.copyViewModel = cvm);
    }
}