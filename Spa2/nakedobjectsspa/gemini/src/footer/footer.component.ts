import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
    ClickHandlerService,
    ConfigService,
    ContextService,
    ErrorService,
    ErrorWrapper,
    Pane,
    RepLoaderService,
    UrlManagerService
} from '@nakedobjects/services';
import { DragAndDropService, IDraggableViewModel } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';

@Component({
    selector: 'nof-footer',
    templateUrl: 'footer.component.html',
    styleUrls: ['footer.component.css']
})
export class FooterComponent implements OnInit, OnDestroy {

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly clickHandler: ClickHandlerService,
        private readonly error: ErrorService,
        private readonly repLoader: RepLoaderService,
        private readonly location: Location,
        private readonly configService: ConfigService,
        private readonly dragAndDrop: DragAndDropService
    ) { }

    private warnSub: ISubscription;
    private messageSub: ISubscription;
    private cvmSub: ISubscription;
    private lcSub: ISubscription;

    loading: boolean;
    template: string;
    footerTemplate: string;
    userName: string;
    warnings: string[];
    messages: string[];
    copyViewModel: IDraggableViewModel;

    goHome = (right?: boolean) => {
        const newPane = this.clickHandler.pane(Pane.Pane1, right);

        if (this.configService.config.leftClickHomeAlwaysGoesToSinglePane && newPane === Pane.Pane1) {
            this.urlManager.setHomeSinglePane();
        } else {
            this.urlManager.setHome(newPane);
        }
    }

    goBack = () => {
        this.location.back();
    }

    goForward = () => {
        this.location.forward();
    }

    swapPanes = () => {
        if (!this.swapDisabled()) {
            this.context.swapCurrentObjects();
            this.urlManager.swapPanes();
        }
    }

    swapDisabled = () => {
        return this.urlManager.isMultiLineDialog() ? true : null;
    }

    singlePane = (right?: boolean) => {
        this.urlManager.singlePane(this.clickHandler.pane(Pane.Pane1, right));
    }

    logOff = () => this.urlManager.logoff();

    applicationProperties = () => this.urlManager.applicationProperties();

    recent = (right?: boolean) => {
        this.urlManager.setRecent(this.clickHandler.pane(Pane.Pane1, right));
    }

    cicero = () => {
        this.urlManager.singlePane(this.clickHandler.pane(Pane.Pane1));
        this.urlManager.cicero();
    }

    get currentCopyColor() {
        return this.copyViewModel.color;
    }

    get currentCopyTitle() {
        return this.copyViewModel.draggableTitle();
    }

    ngOnInit() {
        this.context.getUser().then(user => this.userName = user.userName()).catch((reject: ErrorWrapper) => this.error.handleError(reject));

        this.repLoader.loadingCount$.subscribe(count => this.loading = count > 0);
        this.context.warning$.subscribe(ws => this.warnings = ws);
        this.context.messages$.subscribe(ms => this.messages = ms);
        this.dragAndDrop.copiedViewModel$.subscribe(cvm => this.copyViewModel = cvm);
    }

    ngOnDestroy() {
        safeUnsubscribe(this.warnSub);
        safeUnsubscribe(this.messageSub);
        safeUnsubscribe(this.cvmSub);
        safeUnsubscribe(this.lcSub);
    }
}
