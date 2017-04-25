import { Injectable } from '@angular/core';
import { Pane, getOtherPane } from './route-data';

@Injectable()
export class ClickHandlerService {


    private leftRightClickHandler(currentPane: Pane, right = false): Pane {
        return right ? Pane.Pane2 : Pane.Pane1;
    }

    private sameOtherClickHandler(currentPane: Pane, right = false): Pane {
        return right ? getOtherPane(currentPane) : currentPane;
    }

    readonly pane = this.sameOtherClickHandler;
}
