import { AfterViewInit, ViewChild } from '@angular/core';
import { Component, EventEmitter, Input } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ConfigService } from '@nakedobjects/services';
import { FieldViewModel, fixedDateFormat, PropertyViewModel } from '@nakedobjects/view-models';
import * as momentNs from 'moment';
import { DatePickerComponent, DatePickerOptions } from '../date-picker/date-picker.component';
import { IDatePickerInputEvent, IDatePickerOutputEvent } from '../date-picker/date-picker.component';

@Component({
    selector: 'nof-date-picker-facade',
    templateUrl: 'date-picker-facade.component.html',
    styleUrls: ['date-picker-facade.component.css']
})
export class DatePickerFacadeComponent implements AfterViewInit {

    datePickerOptions = new DatePickerOptions();

    constructor(configService: ConfigService) {
        this.inputEvents = new EventEmitter<IDatePickerInputEvent>();
        this.datePickerOptions.format = configService.config.dateInputFormat;
    }

    @Input()
    control: AbstractControl;

    @Input()
    form: FormGroup;

    private fieldViewModel: FieldViewModel;

    @Input()
    set model(m: FieldViewModel) {
        this.fieldViewModel = m;
        this.datePickerOptions.class = m instanceof PropertyViewModel ? 'property' : 'parameter';
    }

    get model(): FieldViewModel {
        return this.fieldViewModel;
    }

    @ViewChild('dp')
    datepicker: DatePickerComponent;

    inputEvents: EventEmitter<IDatePickerInputEvent>;

    get id() {
        return this.model.paneArgId;
    }

    get description() {
        return this.model.description;
    }

    setValueIfChanged(dateModel: momentNs.Moment | null) {
        const oldValue = this.control.value;
        const newValue = dateModel ? dateModel.format(fixedDateFormat) : '';

        if (newValue !== oldValue) {
            this.model.resetMessage();
            this.model.clientValid = true;
            this.control.setValue(newValue);
        }
    }

    handleDefaultEvent(data: string) {
        if (this.control) {
            if (data === 'closed') {
                const dateModel = this.datepicker.dateModel;
                this.setValueIfChanged(dateModel);
            }
        }
    }

    handleDateChangedEvent(dateModel: momentNs.Moment) {
        if (this.control) {
            this.setValueIfChanged(dateModel);
        }
    }

    handleDateClearedEvent() {
        if (this.control) {
            this.model.resetMessage();
            this.model.clientValid = true;
            this.control.setValue('');
        }
    }

    handleInvalidDateEvent(data: string) {
        if (this.control) {
            this.model.setInvalidDate();
            this.model.clientValid = false;
            this.control.setErrors({ [this.model.getMessage()]: true });
        }
    }

    handleEvents(e: IDatePickerOutputEvent) {
        switch (e.type) {
            case ('default'):
                this.handleDefaultEvent(e.data);
                break;
            case ('dateChanged'):
                this.handleDateChangedEvent(e.data);
                break;
            case ('dateCleared'):
                this.handleDateClearedEvent();
                break;
            case ('dateInvalid'):
                this.handleInvalidDateEvent(e.data);
                break;

            default: // ignore
        }
    }

    ngAfterViewInit(): void {
        const existingValue: any = this.control && this.control.value;
        if (existingValue && (existingValue instanceof String || typeof existingValue === 'string')) {
            setTimeout(() => this.inputEvents.emit({ type: 'setDate', data: existingValue as string, }));
        }
    }

    focus() {
        return this.datepicker.focus();
    }
}
