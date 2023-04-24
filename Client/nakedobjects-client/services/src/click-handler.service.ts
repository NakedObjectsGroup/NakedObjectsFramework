import { Injectable } from '@angular/core';
import { getOtherPane, Pane } from './route-data';
import { ConfigService } from './config.service';

@Injectable()
export class ClickHandlerService {

    constructor(configService: ConfigService) {
        this.pane = configService.config.clickAlwaysGoesToSameSide ? this.leftRightClickHandler : this.sameOtherClickHandler;
    }

    readonly pane = this.sameOtherClickHandler;

    private leftRightClickHandler(currentPane: Pane, right = false): Pane {
        return right ? Pane.Pane2 : Pane.Pane1;
    }

    private sameOtherClickHandler(currentPane: Pane, right = false): Pane {
        return right ? getOtherPane(currentPane) : currentPane;
    }
}
