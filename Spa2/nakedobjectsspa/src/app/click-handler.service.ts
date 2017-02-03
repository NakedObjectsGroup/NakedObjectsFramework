import * as Models from './models';
import { Injectable } from '@angular/core';

@Injectable()
export class ClickHandlerService {


    private leftRightClickHandler(currentPane: number, right = false): number {
        return right ? 2 : 1;
    }

    private sameOtherClickHandler(currentPane: number, right = false): number {
        return right ? Models.getOtherPane(currentPane) : currentPane;
    }

    readonly pane = this.sameOtherClickHandler;
}
