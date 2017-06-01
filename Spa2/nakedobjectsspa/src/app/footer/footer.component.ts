import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { Http } from '@angular/http';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from '../click-handler.service';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import { RepLoaderService } from '../rep-loader.service';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import * as Msg from '../user-messages';
import * as Models from '../models';
import { ConfigService } from '../config.service';
import { AuthService } from '../auth.service';
import { Pane } from '../route-data';
import { ISubscription } from 'rxjs/Subscription';
import { safeUnsubscribe } from '../helpers-components'; 

@Component({
    selector: 'nof-footer',
    template: require('./footer.component.html'),
    styles: [require('./footer.component.css')]
})
export class FooterComponent implements OnInit, OnDestroy {

    constructor(
        private readonly authService: AuthService,
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
        const newPane = this.clickHandler.pane(Pane.Pane1, right);

        if (this.configService.config.leftClickHomeAlwaysGoesToSinglePane && newPane === Pane.Pane1) {
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
        this.urlManager.singlePane(this.clickHandler.pane(Pane.Pane1, right));
    };

    logOff = () => this.urlManager.logoff();

    applicationProperties = () => this.urlManager.applicationProperties();

    recent = (right?: boolean) => {
        this.urlManager.setRecent(this.clickHandler.pane(Pane.Pane1, right));
    };

    cicero = () => {
        this.urlManager.singlePane(this.clickHandler.pane(Pane.Pane1));
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

    private warnSub: ISubscription;
    private messageSub: ISubscription;
    private cvmSub: ISubscription;
    private lcSub: ISubscription;

    ngOnInit() {
        this.context.getUser().then(user => this.userName = user.userName()).catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.repLoader.loadingCount$.subscribe(count => this.loading = count > 0 ? Msg.loadingMessage : "");
        this.context.warning$.subscribe(ws => this.warnings = ws);
        this.context.messages$.subscribe(ms => this.messages = ms);
        this.context.copiedViewModel$.subscribe(cvm => this.copyViewModel = cvm);
    }

    ngOnDestroy() {
        safeUnsubscribe(this.warnSub);
        safeUnsubscribe(this.messageSub);
        safeUnsubscribe(this.cvmSub);
        safeUnsubscribe(this.lcSub);
    }
}