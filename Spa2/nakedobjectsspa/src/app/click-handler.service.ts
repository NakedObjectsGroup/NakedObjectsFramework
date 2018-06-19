import { Injectable } from '@angular/core';
import { Pane, getOtherPane } from './route-data';

@Injectable()
export class ClickHandlerService {

    readonly pane = this.sameOtherClickHandler;

    private leftRightClickHandler(currentPane: Pane, right = false): Pane {
        return right ? Pane.Pane2 : Pane.Pane1;
    }

    private sameOtherClickHandler(currentPane: Pane, right = false): Pane {
        return right ? getOtherPane(currentPane) : currentPane;
    }
}
