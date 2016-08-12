import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { GeminiClickDirective } from "./gemini-click.directive";
import { UrlManager} from "./urlmanager.service";
import { ClickHandlerService } from "./click-handler.service";

@Component({
    selector: 'footer',
    templateUrl: 'app/footer.component.html',
    directives: [GeminiClickDirective]
})
export class FooterComponent {

    constructor(private router: Router,
        private urlManager: UrlManager,
        private clickHandler : ClickHandlerService) { }


    loading: string;
    template: string;
    footerTemplate: string;

    goHome = (right?: boolean) => {
        this.urlManager.setHome(this.clickHandler.pane(1, right));
    }

    goBack: () => void;
    goForward: () => void;
    swapPanes: () => void;
    logOff: () => void;
    singlePane: (right?: boolean) => void;
    recent: (right?: boolean) => void;
    cicero: () => void;
    userName: string;
    applicationProperties: () => void;

    warnings: string[];
    messages: string[];
}