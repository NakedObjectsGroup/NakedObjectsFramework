import { PaneRouteData, ViewType } from '../route-data';
import * as Models from '../models';

export class CiceroViewModel {
    message: string | null;
    output: string | null;
    alert = ""; //Alert is appended before the output
    input: string | null;
    previousInput: string;
    chainedCommands: string[];
        viewType: ViewType;
    clipboard: Models.DomainObjectRepresentation;

    clearInput(): void {
        this.input = null;
    };

    outputMessageThenClearIt() : void {
        this.output = this.message;
        this.message = null;
    }

    clearInputRenderOutputAndAppendAlertIfAny(output: string): void {
        this.clearInput();
        this.output = output;
        if (this.alert) {
            this.output += this.alert;
            this.alert = "";
        }
    }
}