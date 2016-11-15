

import * as Routedata from '../route-data';
import * as Models from '../models';

export class CiceroViewModel {
    message: string;
    output: string;
    alert = ""; //Alert is appended before the output
    input: string;
    parseInput: (input: string) => void;
    previousInput: string;
    chainedCommands: string[];

    selectPreviousInput = () => {
        this.input = this.previousInput;
    }

    clearInput = () => {
        this.input = null;
    }

    autoComplete: (input: string) => void;

    outputMessageThenClearIt() {
        this.output = this.message;
        this.message = null;
    }

    renderHome: (routeData: Routedata.PaneRouteData) => void;
    renderObject: (routeData: Routedata.PaneRouteData) => void;
    renderList: (routeData: Routedata.PaneRouteData) => void;
    renderError: () => void;
    viewType: Routedata.ViewType;
    clipboard: Models.DomainObjectRepresentation;

    executeNextChainedCommandIfAny: () => void;

    popNextCommand(): string {
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
}
