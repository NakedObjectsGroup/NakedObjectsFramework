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

/*
    parseInput: (input: string) => void;
    selectPreviousInput = () => {
        this.input = this.previousInput;
    };
    clearInput = () => {
        this.input = null;
    };
    autoComplete: (input: string) => void;
*/
    outputMessageThenClearIt() {
        this.output = this.message;
        this.message = null;
    }

/*
    renderHome: (routeData: PaneRouteData) => void;
    renderObject: (routeData: PaneRouteData) => void;
    renderList: (routeData: PaneRouteData) => void;
    renderError: () => void;


    executeNextChainedCommandIfAny: () => void;

    popNextCommand(): string | null {
        if (this.chainedCommands) {
            const next = this.chainedCommands[0];
            this.chainedCommands.splice(0, 1);
            return next;

        }
        return null;
    }

    clearInputRenderOutputAndAppendAlertIfAny(output: string): void {
        this.clearInput();
        this.output = output;
        if (this.alert) {
            this.output += this.alert;
            this.alert = "";
        }
    }
    */
}