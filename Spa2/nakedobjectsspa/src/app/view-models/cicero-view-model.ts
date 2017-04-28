import { PaneRouteData, ViewType } from '../route-data';
import * as Models from '../models';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs'; // for declaration compile

export class CiceroViewModel {
    message: string | null;
    alert = ""; //Alert is appended before the output
    input: string | null;
    previousInput: string;
    chainedCommands: string[] | null;
    viewType: ViewType;
    clipboard: Models.DomainObjectRepresentation | null;

    private outputSource = new Subject<string>();
    output$ = this.outputSource.asObservable();


    clearInput(): void {
        this.input = null;
    };

    setOutputSource(text: string | null) {
        this.outputSource.next(Models.withUndefined(text));
    }

    outputMessageThenClearIt() : void {
        this.setOutputSource(this.message);
        this.message = null;
    }

    renderOutputAndAppendAlertIfAny(output: string): string {
        let text = output;
        if (this.alert) {
            text += this.alert;
            this.alert = "";
        }
        return text;
    }


    clearInputRenderOutputAndAppendAlertIfAny(output: string): void {
        this.clearInput();
        let text = output;
        if (this.alert) {
            text += this.alert;
            this.alert = "";
        }
        this.setOutputSource(text);
    }
}