import { AfterViewInit, ViewChild } from '@angular/core';
import { Component, EventEmitter, Input } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ConfigService } from '@nakedobjects/services';
import { FieldViewModel, PropertyViewModel } from '@nakedobjects/view-models';
import { DateTime } from 'luxon';
import { DatePickerComponent, DatePickerOptions } from '../date-picker/date-picker.component';
import { IDatePickerInputEvent, IDatePickerOutputEvent } from '../date-picker/date-picker.component';
import { fixedDateFormat } from '@nakedobjects/services';

@Component({
    selector: 'nof-date-picker-facade',
    templateUrl: 'date-picker-facade.component.html',
    styleUrls: ['date-picker-facade.component.css'],
    standalone: false
})
export class DatePickerFacadeComponent implements AfterViewInit {

    datePickerOptions = new DatePickerOptions();

    constructor(configService: ConfigService) {
        this.inputEvents = new EventEmitter<IDatePickerInputEvent>();
        this.datePickerOptions.format = configService.config.dateInputFormat;
    }

    @Input({required: true})
    control!: AbstractControl;

    @Input({required: true})
    form!: FormGroup;

    private fieldViewModel!: FieldViewModel;

    @Input({required: true})
    set model(m: FieldViewModel) {
        this.fieldViewModel = m;
        this.datePickerOptions.class = m instanceof PropertyViewModel ? 'datepicker-property' : 'datepicker-parameter';
    }

    get model(): FieldViewModel {
        return this.fieldViewModel;
    }

    @ViewChild('dp', {static: false})
    datepicker?: DatePickerComponent;

    inputEvents: EventEmitter<IDatePickerInputEvent>;

    get id() {
        return this.model.paneArgId;
    }

    get description() {
        return this.model.description;
    }

    setValueIfChanged(dateModel: DateTime | null) {
        const oldValue = this.control.value;
        const newValue = dateModel ? dateModel.toFormat(fixedDateFormat) : '';

        if (newValue !== oldValue) {
            this.model.resetMessage();
            this.model.clientValid = true;
            this.control.setValue(newValue);
        }
    }

    handleDefaultEvent(data: string) {
        if (this.control) {
            if (data === 'closed') {
                const dateModel = this.datepicker?.dateModel ?? null;
                this.setValueIfChanged(dateModel);
            }
        }
    }

    handleDateChangedEvent(dateModel: DateTime) {
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

    handleInvalidDateEvent(_: string) {
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
        const existingValue = this.control && this.control.value;
        if (existingValue && (existingValue instanceof String || typeof existingValue === 'string')) {
            setTimeout(() => this.inputEvents.emit({ type: 'setDate', data: existingValue as string, }));
        }
    }

    focus() {
        return this.datepicker?.focus();
    }
}
