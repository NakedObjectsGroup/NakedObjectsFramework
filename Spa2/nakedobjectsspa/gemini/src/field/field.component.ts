import { ElementRef, OnDestroy, QueryList, Renderer2 } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import * as Ro from '@nakedobjects/restful-objects';
import { LoggerService, Pane } from '@nakedobjects/services';
import {
    ChoiceViewModel,
    DialogViewModel,
    DomainObjectViewModel,
    DragAndDropService,
    FieldViewModel,
    IDraggableViewModel,
    ParameterViewModel,
    PropertyViewModel
    } from '@nakedobjects/view-models';
import { Dictionary } from 'lodash';
import every from 'lodash-es/every';
import find from 'lodash-es/find';
import keys from 'lodash-es/keys';
import mapValues from 'lodash-es/mapValues';
import omit from 'lodash-es/omit';
import { BehaviorSubject, SubscriptionLike as ISubscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { AutoCompleteComponent } from '../auto-complete/auto-complete.component';
import { DatePickerFacadeComponent } from '../date-picker-facade/date-picker-facade.component';
import { accept, dropOn, focus, paste, safeUnsubscribe } from '../helpers-components';
import { TimePickerFacadeComponent } from '../time-picker-facade/time-picker-facade.component';
import { CdkDrag, CdkDropList, CdkDragDrop } from '@angular/cdk/drag-drop';

export abstract class FieldComponent implements OnDestroy {

    protected constructor(
        private readonly loggerService: LoggerService,
        private readonly renderer: Renderer2,
        protected readonly dragAndDrop: DragAndDropService
    ) { }

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
        return this.model.returnType === 'boolean';
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
    private lastArgs: Dictionary<Ro.Value>;

    control: AbstractControl;
    currentOptions: ChoiceViewModel[] = [];
    pArgs: Dictionary<Ro.Value>;
    paneId: Pane;
    canDrop = false;
    dragOver = false;

    abstract checkboxList: QueryList<ElementRef>;
    abstract focusList: QueryList<ElementRef | DatePickerFacadeComponent | TimePickerFacadeComponent | AutoCompleteComponent>;

    protected init(vmParent: DialogViewModel | DomainObjectViewModel,
        vm: ParameterViewModel | PropertyViewModel,
        control: AbstractControl) {

        this.vmParent = vmParent;
        this.model = vm;
        this.control = control;

        this.paneId = this.model.onPaneId;

        this.isConditionalChoices = (this.model.entryType === Ro.EntryType.ConditionalChoices ||
            this.model.entryType === Ro.EntryType.MultipleConditionalChoices);

        this.isAutoComplete = this.model.entryType === Ro.EntryType.AutoComplete;

        if (this.isConditionalChoices) {
            this.pArgs = omit(this.model.promptArguments, 'x-ro-nof-members') as Dictionary<Ro.Value>;
            this.populateDropdown();
        }
    }

    get accept() {
        const _this = this;
        return (cdkDrag: CdkDrag<IDraggableViewModel>, cdkDropList: CdkDropList) => {
            return accept(_this.model, _this, cdkDrag.data);
        };
    }

    drop(event: CdkDragDrop<CdkDrag<IDraggableViewModel>>) {
        const cdkDrag: CdkDrag<IDraggableViewModel> = event.item;
        if (event.isPointerOverContainer) {
            dropOn(cdkDrag.data, this.model, this);
        }
        this.canDrop = false;
        this.dragOver = false;
    }

    exit() {
        this.canDrop = false;
        this.dragOver = false;
    }

    enter() {
        this.dragOver = true;
    }

    private isDomainObjectViewModel(object: any): object is DomainObjectViewModel {
        return object && 'properties' in object;
    }

    private mapValues(args: Dictionary<Ro.Value>, parmsOrProps: { argId: string, getValue: () => Ro.Value }[]) {
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
            this.loggerService.throw('FieldComponent:populateArguments Expect dialog or object');
        }

        let parmsOrProps: { argId: string, getValue: () => Ro.Value }[];

        if (this.isDomainObjectViewModel(object)) {
            parmsOrProps = object.properties;
        } else {
            parmsOrProps = dialog.parameters;
        }

        return this.mapValues(this.pArgs, parmsOrProps);
    }

    private argsChanged(newArgs: Dictionary<Ro.Value>) {
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
                        if (this.model.entryType === Ro.EntryType.MultipleConditionalChoices) {
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
                this.renderer.setProperty(element, 'indeterminate', true);
                this.renderer.setProperty(element, 'checked', null);
            } else {
                this.renderer.setProperty(element, 'indeterminate', false);
                this.renderer.setProperty(element, 'checked', !!input);
            }
        }
    }

    private select(item: ChoiceViewModel) {
        this.model.choices = [];
        this.model.selectedChoice = item;
        this.control.reset(item);
    }

    fileUpload(evt: Event) {

        const file: File = (evt.target as HTMLInputElement)!.files![0];
        const fileReader = new FileReader();
        fileReader.onloadend = () => {
            const link = new Ro.Link({
                href: fileReader.result as string,
                type: file.type,
                title: file.name
            });

            this.control.reset(link);
            this.model.file = link;
        };

        fileReader.readAsDataURL(file);
    }

    paste(event: KeyboardEvent) {
        paste(event, this.model, this, () => this.dragAndDrop.getCopyViewModel(), () => this.dragAndDrop.setCopyViewModel(null));
    }

    clear() {
        this.control.reset('');
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
            return focus(first);
        }
        return first && first.focus();
    }

    ngOnDestroy() {
        safeUnsubscribe(this.sub);
    }
}
