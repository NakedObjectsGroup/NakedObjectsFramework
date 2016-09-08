import { Directive, ElementRef, HostListener, Output, EventEmitter, Input, OnInit } from '@angular/core';
import * as ViewModels  from './nakedobjects.viewmodels';
import * as Models from './models';
import * as _ from "lodash";
import * as Ro from './nakedobjects.rointerfaces';
import { Observable } from 'rxjs/Observable';
import "./rxjs-extensions";
import { Subject } from 'rxjs/Subject';

@Directive({ selector: '[geminiConditionalChoices]' })
export class GeminiConditionalChoicesDirective implements OnInit {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    model: ViewModels.ParameterViewModel;
    currentOptions: ViewModels.ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId: number;

    @Input('geminiConditionalChoices')
    set viewModel(vm: ViewModels.ParameterViewModel) {
        this.model = vm;
        this.pArgs = _.omit(this.model.promptArguments, "x-ro-nof-members") as _.Dictionary<Models.Value>;
        this.paneId = this.model.onPaneId;
    }

    @Input('parent')
    parent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;

    @Input('parameterChanged')
    parameterChanged: Observable<boolean>;

    isDomainObjectViewModel(object: any): object is ViewModels.DomainObjectViewModel {
        return object && "properties" in object;
    }

    mapValues(args: _.Dictionary<Models.Value>, parmsOrProps: { argId: string, getValue: () => Models.Value }[]) {
        return _.mapValues(this.pArgs, (v, n) => {
            const pop = _.find(parmsOrProps, p => p.argId === n);
            return pop.getValue();
        });
    }

    populateArguments() {

        const dialog = this.parent as ViewModels.DialogViewModel;
        const object = this.parent as ViewModels.DomainObjectViewModel;

        if (!dialog && !object) {
            throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
        }

        let parmsOrProps: { argId: string, getValue: () => Models.Value }[] = [];

        if (this.isDomainObjectViewModel(object)) {
            parmsOrProps = object.properties;
        } else {
            parmsOrProps = dialog.parameters;
        }

        return this.mapValues(this.pArgs, parmsOrProps);
    }

    populateDropdown() {
        const nArgs = this.populateArguments();
        const prompts = this.model.conditionalChoices(nArgs);//  scope.select({ args: nArgs });
        prompts.then((cvms: ViewModels.ChoiceViewModel[]) => {
            // if unchanged return 
            if (cvms.length === this.currentOptions.length && _.every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
                return;
            }
            this.model.choices = cvms;
            this.currentOptions = cvms;
        }).catch(() => {
            // error clear everything 
            this.model.selectedChoice = null;
            this.currentOptions = [];
        });
    }

    wrapReferences(val: string): string | Ro.ILink {
        if (val && this.model.type === "ref") {
            return { href: val };
        }
        return val;
    }

    ngOnInit(): void {
        this.populateDropdown();
        this.parameterChanged.subscribe((change) => {
            if (change) {
                this.populateDropdown();
            }
        });
    }

    setListeners() {
        _.forEach(this.pArgs, (v, n) =>
            document.getElementById(`${n}${this.paneId}`).addEventListener("change", () => this.populateDropdown()));
    }
}