import * as Constants from '../constants';
import { Component, ElementRef, OnInit, Input, Output, EventEmitter, ViewChild, Renderer, OnDestroy } from '@angular/core';
import * as momentNs from 'moment';
import concat from 'lodash-es/concat';
import { BehaviorSubject, Observable,  SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe, focus } from '../helpers-components';
import * as Msg from '../user-messages';
import * as Models from '../models';
import * as Validate from '../validate';
import { debounceTime } from 'rxjs/operators';

const moment = momentNs;

// based on ng2-datepicker https://github.com/jkuri/ng2-datepicker

export type IDatePickerInputEvent = IDatePickerInputDateEvent | IDatePickerInputActionEvent;

export type IDatePickerOutputEvent = IDatePickerOutputDefaultEvent | IDatePickerOutputChangedEvent | IDatePickerOutputInvalidEvent | IDatePickerOutputClearedEvent;

export interface IDatePickerInputDateEvent {
    type: "setDate";
    data:  string;
}

export interface IDatePickerInputActionEvent {
    type: "action";
    data: "toggle" | "close" | "open";
}

export interface IDatePickerOutputDefaultEvent {
    type: "default";
    data: "init" | "opened" | "closed";
}

export interface IDatePickerOutputChangedEvent {
    type: "dateChanged";
    data: momentNs.Moment;
}

export interface IDatePickerOutputInvalidEvent {
    type: "dateInvalid";
    data: string;
}

export interface IDatePickerOutputClearedEvent {
    type: "dateCleared";
    data: string;
}

export class DatePickerOptions {
    firstWeekdaySunday?: boolean;
    format?: string;

    constructor(obj?: DatePickerOptions) {
        this.firstWeekdaySunday = obj && obj.firstWeekdaySunday ? obj.firstWeekdaySunday : false;
        this.format = obj && obj.format ? obj.format : 'YYYY-MM-DD';
    }
}

export interface ICalendarDate {
    day: number | null;
    month: number | null;
    year: number | null;
    enabled: boolean;
    today: boolean;
    selected: boolean;
    momentObj: momentNs.Moment;
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

    @ViewChild("inp")
    inputField: ElementRef;

    constructor(
        private readonly renderer: Renderer
    ) {
        this.opened = false;
        this.options = this.options || {};
        this.days = [];
        this.dateModelValue = null;

        this.outputEvents = new EventEmitter<IDatePickerOutputEvent>();
    }

    private validInputFormats = ["DD/MM/YYYY", "DD/MM/YY", "D/M/YY", "D/M/YYYY", "D MMM YYYY", "D MMMM YYYY", Constants.fixedDateFormat];

    private dateModelValue: momentNs.Moment | null;
    private modelValue: string;

    todayMsg = Msg.today;
    clearMsg = Msg.clear;

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

    get dateModel(): momentNs.Moment | null {
        return this.dateModelValue;
    }

    get currentDate(): momentNs.Moment {
        return this.dateModelValue || moment().utc();
    }

    set dateModel(date: momentNs.Moment | null) {
        if (date) {
            this.dateModelValue = date;
            this.outputEvents.emit({ type: 'dateChanged', data: this.dateModel! });
        } else {
            this.dateModelValue = null;
            this.outputEvents.emit({ type: 'dateCleared', data: "" });
        }
    }

    private eventsSub: ISubscription;

    private validateDate(newValue: string) {
        return Validate.validateDate(newValue, this.validInputFormats);
    }

    setDateIfChanged(newDate: momentNs.Moment) {
        const currentDate = this.dateModel;
        if (!newDate.isSame(Models.withUndefined(currentDate))) {
            this.setValue(newDate);
            setTimeout(() => this.model = this.formatDate(this.dateModel));
        }
    }

    inputChanged(newValue: string) {

        const dt = this.validateDate(newValue);

        if (dt && dt.isValid()) {
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

        this.outputEvents.emit({ type: 'default', data: "init" } as IDatePickerOutputDefaultEvent);

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
                        if (date && date.isValid()) {
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

    generateCalendar() {
        const currentDate = moment(this.currentDate); // clone so not mutated
        const month = currentDate.month();
        const year = currentDate.year();
        let n = 1;
        const firstWeekDay = (this.options.firstWeekdaySunday) ? currentDate.date(2).day() : currentDate.date(1).day();

        if (firstWeekDay !== 1) {
            n -= (firstWeekDay + 6) % 7;
        }

        this.days = [];

        const endOfMonth = moment(currentDate).endOf('month');
        for (let i = n; i <= endOfMonth.date(); i += 1) {
            const date: momentNs.Moment = moment.utc(`${i}.${month + 1}.${year}`, 'DD.MM.YYYY');
            const today: boolean = moment().utc().isSame(date, 'day') && moment().isSame(date, 'month');
            const selected: boolean = this.currentDate.isSame(date, 'day');

            const day: ICalendarDate = {
                day: i > 0 ? i : null,
                month: i > 0 ? month : null,
                year: i > 0 ? year : null,
                enabled: i > 0,
                today: i > 0 && today,
                selected: i > 0 && selected,
                momentObj: date
            };

            this.days.push(day);
        }
    }

    setValue(date: momentNs.Moment | null) {
        this.dateModel = date;
    }

    private formatDate(date: momentNs.Moment | null) {
        return this.dateModel ? this.dateModel.format(this.options.format) : "";
    }

    selectDate(date: momentNs.Moment | null, e?: MouseEvent, ) {
        if (e) { e.preventDefault(); }
        setTimeout(() => {
            this.setValue(date);
            this.model = this.formatDate(this.dateModel);
        });
        this.opened = false;
    }

    writeValue(date: momentNs.Moment) {
        if (!date) { return; }
        this.dateModelValue = date;
    }

    prevMonth() {
        const date = this.currentDate.subtract(1, 'month');
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    nextMonth() {
        const date =  this.currentDate.add(1, 'month');
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    prevYear() {
        const date = this.currentDate.subtract(1, 'year');
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    nextYear() {
        const date =  this.currentDate.add(1, 'year');
        this.setValue(date);
        this.model = this.formatDate(this.dateModel);
        this.generateCalendar();
    }

    today() {
        this.selectDate(moment().utc());
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
        this.model = "";
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

    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
        safeUnsubscribe(this.eventsSub);
    }

    focus() {
        return focus(this.renderer, this.inputField);
    }
}
