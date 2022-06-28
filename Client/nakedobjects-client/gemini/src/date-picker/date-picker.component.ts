import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import { validateDate } from '@nakedobjects/view-models';
import concat from 'lodash-es/concat';
import { DateTime }  from 'luxon';
import { BehaviorSubject, Observable, SubscriptionLike as ISubscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { focus, safeUnsubscribe } from '../helpers-components';
import { Dictionary } from 'lodash';
import { fixedDateFormat, supportedDateFormats } from '@nakedobjects/services';

// based on ng2-datepicker https://github.com/jkuri/ng2-datepicker

export type IDatePickerInputEvent = IDatePickerInputDateEvent | IDatePickerInputActionEvent;

export type IDatePickerOutputEvent = IDatePickerOutputDefaultEvent | IDatePickerOutputChangedEvent | IDatePickerOutputInvalidEvent | IDatePickerOutputClearedEvent;

export interface IDatePickerInputDateEvent {
    type: 'setDate';
    data: string;
}

export interface IDatePickerInputActionEvent {
    type: 'action';
    data: 'toggle' | 'close' | 'open';
}

export interface IDatePickerOutputDefaultEvent {
    type: 'default';
    data: 'init' | 'opened' | 'closed';
}

export interface IDatePickerOutputChangedEvent {
    type: 'dateChanged';
    data: DateTime;
}

export interface IDatePickerOutputInvalidEvent {
    type: 'dateInvalid';
    data: string;
}

export interface IDatePickerOutputClearedEvent {
    type: 'dateCleared';
    data: string;
}

export class DatePickerOptions {
    firstWeekdaySunday?: boolean;
    format: string;
    class?: string;

    constructor(obj?: DatePickerOptions) {
        this.firstWeekdaySunday = obj && obj.firstWeekdaySunday ? obj.firstWeekdaySunday : false;
        this.format = obj && obj.format ? obj.format : fixedDateFormat;
        this.class = obj && obj.class;
    }
}

export interface ICalendarDate {
    day: number | null;
    month: number | null;
    year: number | null;
    enabled: boolean;
    today: boolean;
    selected: boolean;
    dateTime: DateTime;
}

@Component({
    selector: 'nof-date-picker',
    templateUrl: 'date-picker.component.html',
    styleUrls: ['date-picker.component.css']
})
export class DatePickerComponent implements OnInit, OnDestroy {

    @Input()
    options: DatePickerOptions;

    @Input()
    inputEvents: EventEmitter<IDatePickerInputEvent>;

    @Output()
    outputEvents: EventEmitter<IDatePickerOutputEvent>;

    @Input()
    id: string;

    @Input()
    description: string;

    opened: boolean;
    days: ICalendarDate[];

    @ViewChild('inp', {static: false})
    inputField: ElementRef;

    constructor() {
        this.opened = false;
        this.options = {format : fixedDateFormat};
        this.days = [];
        this.dateModelValue = null;

        this.outputEvents = new EventEmitter<IDatePickerOutputEvent>();
    }

    private validInputFormats = supportedDateFormats.concat(fixedDateFormat);

    private dateModelValue: DateTime | null;
    private modelValue: string;

    private bSubject: BehaviorSubject<string>;
    private sub: ISubscription;

    set model(s: string) {
        this.modelValue = s;

        if (this.bSubject) {
            this.bSubject.next(s);
        }
    }

    get model(): string {
        return this.modelValue;
    }

    get dateModel(): DateTime | null {
        return this.dateModelValue;
    }

    get currentDate(): DateTime {
        return this.dateModelValue || DateTime.now();
    }

    set dateModel(date: DateTime | null) {
        if (date) {
            this.dateModelValue = date;
            this.outputEvents.emit({ type: 'dateChanged', data: this.dateModel! });
        } else {
            this.dateModelValue = null;
            this.outputEvents.emit({ type: 'dateCleared', data: '' });
        }
    }

    private eventsSub: ISubscription;

    private validateDate(newValue: string) {
        return validateDate(newValue, this.validInputFormats);
    }

    setDateIfChanged(newDate: DateTime) {
        const currentDate = this.dateModel;
        if (!currentDate || !newDate.equals(currentDate)) {
            this.setValue(newDate);
            setTimeout(() => this.model = this.formatDate(this.dateModel));
        }
    }

    inputChanged(newValue: string) {

        const dt = this.validateDate(newValue);

        if (dt && dt.isValid) {
            this.setDateIfChanged(dt);
        } else {
            this.setValue(null);
            if (newValue) {
                this.outputEvents.emit({ type: 'dateInvalid', data: newValue });
            }
        }
    }

    ngOnInit() {
        this.options = new DatePickerOptions(this.options);
        const optionFormats = this.options.format ? [this.options.format] : [];
        this.validInputFormats = concat(optionFormats, this.validInputFormats);

        this.outputEvents.emit({ type: 'default', data: 'init' } as IDatePickerOutputDefaultEvent);

        if (this.inputEvents) {
            this.eventsSub = this.inputEvents.subscribe((e: IDatePickerInputEvent) => {
                switch (e.type) {
                    case 'action': {
                        if (e.data === 'toggle') {
                            this.toggle();
                        }
                        if (e.data === 'close') {
                            this.close();
                        }
                        if (e.data === 'open') {
                            this.open();
                        }
                        break;
                    }
                    case 'setDate': {
                        const date = this.validateDate(e.data);
                        if (date && date.isValid) {
                            this.selectDate(date);
                        } else {
                            throw new Error(`Invalid date: ${e.data}`);
                        }

                        break;
                    }
                }
            });
        }
    }

    private isSame (dt1 : DateTime, dt2 : DateTime) {
        return dt1.day === dt2.day && dt1.month === dt2.month && dt1.year === dt2.year;
    }


    generateCalendar() {
    
        const month = this.currentDate.month;
        const year = this.currentDate.year;
        let n = 1;
        let firstdow = this.currentDate.startOf("month").weekday;
        let firstWeekDay = firstdow + 1;

        if (firstWeekDay === 8) {
            firstWeekDay = 1;
        }

        if (firstWeekDay !== 1) {
            n -= (firstWeekDay + 6) % 7;
        }

        this.days = [];

        const endOfMonth = this.currentDate.endOf('month');
        for (let i = n; i <= endOfMonth.day; i += 1) {
            const date: DateTime = DateTime.local(year, month, i);
            const today: boolean = this.isSame(DateTime.now(), date);
            const selected: boolean = !!this.dateModel && this.isSame(this.dateModel, date);

            const day: ICalendarDate = {
                day: i > 0 ? i : null,
                month: i > 0 ? month : null,
                year: i > 0 ? year : null,
                enabled: i > 0,
                today: i > 0 && today,
                selected: i > 0 && selected,
                dateTime: date
            };

            this.days.push(day);
        }
    }

    setValue(date: DateTime | null) {
        this.dateModel = date;
    }

    private formatDate(date: DateTime | null) {
        return this.dateModel ? this.dateModel.toFormat(this.options.format) : '';
    }

    selectDate(date: DateTime | null, e?: MouseEvent, ) {
        if (e) { e.preventDefault(); }
        setTimeout(() => {
            this.setValue(date);
            this.model = this.formatDate(this.dateModel);
        });
        this.opened = false;
    }

    writeValue(date: DateTime) {
        if (!date) { return; }
        this.dateModelValue = date;
    }

    prevMonth() {
        const date = this.currentDate.minus({month :1});
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    nextMonth() {
        const date = this.currentDate.plus({month:1});
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    prevYear() {
        const date = this.currentDate.minus({year:1});
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    nextYear() {
        const date = this.currentDate.plus({year:1});
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    today() {
        this.selectDate(DateTime.now());
    }

    toggle() {
        const change = this.opened ? this.close : this.open;
        change();
    }

    private open = () => {
        this.generateCalendar();
        this.opened = true;
        this.outputEvents.emit({ type: 'default', data: 'opened' } as IDatePickerOutputDefaultEvent);
    }

    private close = () => {
        this.opened = false;
        this.outputEvents.emit({ type: 'default', data: 'closed' } as IDatePickerOutputDefaultEvent);
    }

    clear() {
        this.selectDate(null);
        this.model = '';
        this.close();
    }

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.model;
            this.bSubject = new BehaviorSubject(initialValue);

            this.sub = this.bSubject
                .pipe(debounceTime(1000)).subscribe((data: string) => this.inputChanged(data));
        }

        return this.bSubject;
    }

    classes(): Dictionary<boolean | null> {
        return {
            'datepicker-input': true,
            [this.options.class]: !!(this.options && this.options.class)
        };
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
        safeUnsubscribe(this.eventsSub);
    }

    focus() {
        return focus(this.inputField);
    }
}
