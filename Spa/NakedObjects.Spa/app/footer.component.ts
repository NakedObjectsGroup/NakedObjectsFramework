import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { GeminiClickDirective } from "./gemini-click.directive";
import { UrlManager} from "./urlmanager.service";
import { ClickHandlerService } from "./click-handler.service";
import { FocusManager } from "./focus-manager.service";
import { Context } from "./context.service";
import { Error } from "./error.service";
import * as Msg from "./user-messages.config";
import * as Config from "./nakedobjects.config";
import * as Models from "./models";
import { RepLoader } from "./reploader.service";

@Component({
    selector: 'footer',
    templateUrl: 'app/footer.component.html',
    styleUrls: ['app/footer.component.css']
})
export class FooterComponent implements OnInit {

    constructor(private router: Router,
        private urlManager: UrlManager,
        private focusManager: FocusManager,
        private context : Context,
        private clickHandler: ClickHandlerService,
        private error: Error,
        private repLoader : RepLoader,
        private location : Location) { }


    loading: string;
    template: string;
    footerTemplate: string;

    goHome = (right?: boolean) => {
        this.focusManager.focusOverrideOff();
        this.context.updateValues();
        this.urlManager.setHome(this.clickHandler.pane(1, right));
    }

    goBack = () => {
        this.focusManager.focusOverrideOff();
        this.context.updateValues();
        this.location.back();
    }

    goForward = () => {
        this.focusManager.focusOverrideOff();
        this.context.updateValues();
        this.location.forward();
    }

    swapPanes = () => {
        // $rootScope.$broadcast(Nakedobjectsconstants.geminiPaneSwapEvent);
        this.context.updateValues();
        this.context.swapCurrentObjects();
        this.urlManager.swapPanes();
    }

    singlePane = (right?: boolean) => {
        this.context.updateValues();
        this.urlManager.singlePane(this.clickHandler.pane(1, right));
        this.focusManager.refresh(1);
    }

    logOff = () => {
        this.context.getUser().
            then(u => {
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
            }).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    };

    applicationProperties = () => {
        this.context.updateValues();
        this.urlManager.applicationProperties();
    };
   
    recent = (right?: boolean) => {
        this.context.updateValues();
        this.focusManager.focusOverrideOff();
        this.urlManager.setRecent(this.clickHandler.pane(1, right));
    };

    cicero = () => {
        this.context.updateValues();
        this.urlManager.singlePane(this.clickHandler.pane(1));
        this.urlManager.cicero();
    }

    userName: string;
   
    warnings: string[];
    messages: string[];

    ngOnInit() {
        this.context.getUser().
            then(user => this.userName = user.userName()).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.repLoader.loadingCount$.subscribe(count => this.loading = count > 0 ? Msg.loadingMessage : "");
    }
}