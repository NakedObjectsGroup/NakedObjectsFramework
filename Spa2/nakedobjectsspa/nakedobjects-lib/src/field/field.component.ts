import { AutoCompleteComponent } from '../auto-complete/auto-complete.component';
import * as Models from '../models';
import { AbstractControl } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { ElementRef, QueryList, Renderer, OnDestroy } from '@angular/core';
import { ContextService } from '../context.service';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { FieldViewModel } from '../view-models/field-view-model';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { ConfigService } from '../config.service';
import { LoggerService } from '../logger.service';
import { Pane } from '../route-data';
import { Dictionary } from 'lodash';
import find from 'lodash-es/find';
import every from 'lodash-es/every';
import mapValues from 'lodash-es/mapValues';
import omit from 'lodash-es/omit';
import keys from 'lodash-es/keys';
import { BehaviorSubject ,  SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe, focus, accept, dropOn, paste } from '../helpers-components';
import { DatePickerFacadeComponent } from '../date-picker-facade/date-picker-facade.component';
import { TimePickerFacadeComponent } from '../time-picker-facade/time-picker-facade.component';
import { debounceTime } from 'rxjs/operators';

export abstract class FieldComponent implements OnDestroy {

    protected constructor(
        private readonly context: ContextService,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService,
        private readonly renderer: Renderer
    ) {}

    set formGroup(fm: FormGroup) {
        this.formGrp = fm;
        this.formGrp.valueChanges.pipe(debounceTime(200)).subscribe(data => this.onValueChanged());
        this.onValueChanged(); // (re)set validation messages now
    }

    get formGroup() {
        return this.formGrp;
    }

    get message() {
        return this.model.getMessage();
    }

    get isBoolean() {
        return this.model.returnType === "boolean";
    }

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.control.value;
            this.bSubject = new BehaviorSubject(initialValue);

            this.sub = this.control.valueChanges.subscribe((data) => {
                this.bSubject.next(data);
            });
        }

        return this.bSubject;
    }

    private formGrp: FormGroup;
    private vmParent: DialogViewModel | DomainObjectViewModel;
    private model: ParameterViewModel | PropertyViewModel;
    private isConditionalChoices: boolean;
    private isAutoComplete: boolean;
    private bSubject: BehaviorSubject<any>;
    private sub: ISubscription;
    private lastArgs: Dictionary<Models.Value>;

    control: AbstractControl;
    currentOptions: ChoiceViewModel[] = [];
    pArgs: Dictionary<Models.Value>;
    paneId: Pane;
    canDrop = false;

    abstract checkboxList: QueryList<ElementRef>;
    abstract focusList: QueryList<ElementRef | DatePickerFacadeComponent | TimePickerFacadeComponent | AutoCompleteComponent>;

    protected init(vmParent: DialogViewModel | DomainObjectViewModel,
        vm: ParameterViewModel | PropertyViewModel,
        control: AbstractControl) {

        this.vmParent = vmParent;
        this.model = vm;
        this.control = control;

        this.paneId = this.model.onPaneId;

        this.isConditionalChoices = (this.model.entryType === Models.EntryType.ConditionalChoices ||
            this.model.entryType === Models.EntryType.MultipleConditionalChoices);

        this.isAutoComplete = this.model.entryType === Models.EntryType.AutoComplete;

        if (this.isConditionalChoices) {
            this.pArgs = omit(this.model.promptArguments, "x-ro-nof-members") as Dictionary<Models.Value>;
            this.populateDropdown();
        }
    }

    accept(droppableVm: FieldViewModel) {
        return accept(droppableVm, this);
    }

    drop(draggableVm: IDraggableViewModel) {
        dropOn(draggableVm, this.model, this);
    }

    private isDomainObjectViewModel(object: any): object is DomainObjectViewModel {
        return object && "properties" in object;
    }

    private mapValues(args: Dictionary<Models.Value>, parmsOrProps: { argId: string, getValue: () => Models.Value }[]) {
        return mapValues(this.pArgs,
            (v, n) => {
                const pop = find(parmsOrProps, p => p.argId === n);
                return pop!.getValue();
            });
    }

    private populateArguments() {

        const dialog = this.vmParent as DialogViewModel;
        const object = this.vmParent as DomainObjectViewModel;

        if (!dialog && !object) {
            this.loggerService.throw("FieldComponent:populateArguments Expect dialog or object");
        }

        let parmsOrProps: { argId: string, getValue: () => Models.Value }[];

        if (this.isDomainObjectViewModel(object)) {
            parmsOrProps = object.properties;
        } else {
            parmsOrProps = dialog.parameters;
        }

        return this.mapValues(this.pArgs, parmsOrProps);
    }

    private argsChanged(newArgs: Dictionary<Models.Value>) {
        const same = this.lastArgs &&
                     keys(this.lastArgs).length === keys(newArgs).length &&
                     every(this.lastArgs, (v, k) => newArgs[k].toValueString() === v.toValueString());

        this.lastArgs = newArgs;
        return !same;
    }

    private populateDropdown() {
        const nArgs = this.populateArguments();
        if (this.argsChanged(nArgs)) {
            const prompts = this.model.conditionalChoices(nArgs);
            prompts.
                then((cvms: ChoiceViewModel[]) => {
                    // if unchanged return
                    if (cvms.length === this.currentOptions.length && every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
                        return;
                    }
                    this.model.choices = cvms;
                    this.currentOptions = cvms;

                    if (this.isConditionalChoices) {
                        // need to reset control to find the selected options
                        if (this.model.entryType === Models.EntryType.MultipleConditionalChoices) {
                            this.control.reset(this.model.selectedMultiChoices);
                        } else {
                            this.control.reset(this.model.selectedChoice);
                        }
                    }
                }).
                catch(() => {
                    // error clear everything
                    this.model.selectedChoice = null;
                    this.currentOptions = [];
                });
        }
    }

    private onChange() {
        if (this.isConditionalChoices) {
            this.populateDropdown();
        } else if (this.isAutoComplete) {
            this.populateAutoComplete();
        } else if (this.isBoolean) {
            this.populateBoolean();
        }
    }

    private onValueChanged() {
        if (this.model) {
            this.onChange();
        }
    }

    private populateAutoComplete() {
        const input = this.control.value;

        if (input instanceof ChoiceViewModel) {
            return;
        }

        if (input && input.length > 0 && input.length >= this.model.minLength) {
            this.model.prompt(input)
                .then((cvms: ChoiceViewModel[]) => {
                    if (cvms.length === this.currentOptions.length && every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
                        return;
                    }
                    this.model.choices = cvms;
                    this.currentOptions = cvms;
                    this.model.selectedChoice = null;
                })
                .catch(() => {
                    this.model.choices = [];
                    this.currentOptions = [];
                    this.model.selectedChoice = null;
                });
        } else {
            this.model.choices = [];
            this.currentOptions = [];
            this.model.selectedChoice = null;
        }
    }

    protected populateBoolean() {

        // editable booleans only
        if (this.isBoolean && this.control) {
            const input = this.control.value;
            const element = this.checkboxList.first.nativeElement;
            if (input == null) {
                this.renderer.setElementProperty(element, "indeterminate", true);
                this.renderer.setElementProperty(element, "checked", null);
            } else {
                this.renderer.setElementProperty(element, "indeterminate", false);
                this.renderer.setElementProperty(element, "checked", !!input);
            }
        }
    }

    private select(item: ChoiceViewModel) {
        this.model.choices = [];
        this.model.selectedChoice = item;
        this.control.reset(item);
    }

    fileUpload(evt: Event) {

        const file: File = (evt.target as HTMLInputElement) !.files![0];
        const fileReader = new FileReader();
        fileReader.onloadend = () => {
            const link = new Models.Link({
                href: fileReader.result,
                type: file.type,
                title: file.name
            });

            this.control.reset(link);
            this.model.file = link;
        };

        fileReader.readAsDataURL(file);
    }

    paste(event: KeyboardEvent) {
        paste(event, this.model, this, () => this.context.getCopyViewModel(), () => this.context.setCopyViewModel(null));
    }

    clear() {
        this.control.reset("");
        this.model.clear();
    }

    private filterEnter(event: KeyboardEvent) {
        const enterKeyCode = 13;
        if (event && event.keyCode === enterKeyCode) {
            event.preventDefault();
        }
    }

    protected handleKeyEvents(event: KeyboardEvent, isMultiline: boolean) {
        this.paste(event);
        // catch and filter enters or they will submit form - ok for multiline
        if (!isMultiline) {
            this.filterEnter(event);
        }
    }

    private triStateClick = (currentValue: any) => {

        switch (currentValue) {
            case false:
                return true;
            case true:
                return null;
            default: // null
                return false;
        }
    }

    protected handleClick(event: Event) {
        if (this.isBoolean && this.model.optional) {
            const currentValue = this.control.value;
            setTimeout(() => this.control.setValue(this.triStateClick(currentValue)));
            event.preventDefault();
        }
    }

    focus() {
        const first = this.focusList && this.focusList.first;

        if (first instanceof ElementRef) {
            return focus(this.renderer, first);
        }
        return first && first.focus();
    }

    ngOnDestroy() {
        safeUnsubscribe(this.sub);
    }
}
