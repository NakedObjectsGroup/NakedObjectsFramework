import * as Constants from '../constants';
import { Component, ElementRef, OnInit, Input, Output, EventEmitter, ViewChild, Renderer } from '@angular/core';
import * as moment from 'moment';
import concat from 'lodash/concat';
import { BehaviorSubject } from 'rxjs';
import { ISubscription } from 'rxjs/Subscription';
import { safeUnsubscribe, focus } from '../helpers-components'; 

// based on ng2-datepicker https://github.com/jkuri/ng2-datepicker
// todo - clean it up !!!!

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
    data: moment.Moment;
}

export interface IDatePickerOutputInvalidEvent {
    type: "dateInvalid";
    data: string;
}

export interface IDatePickerOutputClearedEvent {
    type: "dateCleared";
    data: string;
}

export class DateModel  {
  
    formatted: string;
    momentObj: moment.Moment;

    private initFromDate(date: moment.Moment, format: string) {
        this.formatted = date.format(format);
        this.momentObj = date;
    }

    private initFromDateModel(dateModel: DateModel) {
        this.formatted = dateModel && dateModel.formatted ? dateModel.formatted : "";
        this.momentObj = dateModel && dateModel.momentObj ? dateModel.momentObj : null;
    }

    constructor(obj?: DateModel | moment.Moment, format?: string) {
        if (obj && format) {
            this.initFromDate(obj as moment.Moment, format);
        }
        else {
            this.initFromDateModel(obj as DateModel);
        }
    }
}


export class DatePickerOptions {
    autoApply?: boolean;
    style?: 'normal' | 'big' | 'bold';
    locale?: string;
    initialDate?: Date;
    firstWeekdaySunday?: boolean;
    format?: string;
    selectYearText?: string;
    todayText?: string;
    clearText?: string;

    constructor(obj?: DatePickerOptions) {
        this.autoApply = obj && obj.autoApply;
        this.style = obj && obj.style ? obj.style : 'normal';
        this.locale = obj && obj.locale ? obj.locale : 'en';
        this.initialDate = obj && obj.initialDate ? obj.initialDate : null;
        this.firstWeekdaySunday = obj && obj.firstWeekdaySunday ? obj.firstWeekdaySunday : false;
        this.format = obj && obj.format ? obj.format : 'YYYY-MM-DD';
        this.selectYearText = obj && obj.selectYearText ? obj.selectYearText : 'Select Year';
        this.todayText = obj && obj.todayText ? obj.todayText : 'Today';
        this.clearText = obj && obj.clearText ? obj.clearText : 'Clear';
    }
}

export interface ICalendarDate {
    day: number;
    month: number;
    year: number;
    enabled: boolean;
    today: boolean;
    selected: boolean;
    momentObj: moment.Moment;
}

@Component({
    selector: 'nof-date-picker',
    template: require('./date-picker.component.html'),
    styles: [require('./date-picker.component.css')]
})
export class DatePickerComponent implements OnInit {

    @Input() 
    options: DatePickerOptions;
    
    @Input() 
    inputEvents: EventEmitter<IDatePickerInputEvent>;
    
    @Output() 
    outputEvents: EventEmitter<IDatePickerOutputEvent>;

    @Input()
    id : string;

    
    opened: boolean;
    days: ICalendarDate[];
    years: number[];
    yearPicker: boolean;

    constructor(
        private readonly el: ElementRef,
        private readonly renderer: Renderer
    ) {
        this.opened = false;
        this.options = this.options || {};
        this.days = [];
        this.years = [];
        this.dateModelValue = new DateModel();

        this.outputEvents = new EventEmitter<IDatePickerOutputEvent>();
    }

    private validInputFormats = ["DD/MM/YYYY", "DD/MM/YY", "D/M/YY", "D/M/YYYY", "D MMM YYYY", "D MMMM YYYY", Constants.fixedDateFormat];

    private dateModelValue: DateModel;
    private modelValue : string; 

    set model(s: string) {
        this.modelValue = s;

        if (this.bSubject) {
            this.bSubject.next(s);
        }
    }

    get model(): string {
        return this.modelValue;
    }

    get dateModel(): DateModel {
        return this.dateModelValue;
    }

    get currentDate(): moment.Moment {
        return this.dateModelValue.momentObj || moment();
    }

    set dateModel(date: DateModel) {
        if (date && date.momentObj) { 
            this.dateModelValue = date;
            this.outputEvents.emit({ type: 'dateChanged', data: this.dateModel.momentObj });
        }
        else {
            this.dateModelValue = date;
            this.outputEvents.emit({ type: 'dateCleared', data: "" });
        }
    }

    private eventsSub: ISubscription;

    private validateDate(newValue: string) {
        let dt: moment.Moment;

        for (let f of this.validInputFormats) {
            dt = moment.utc(newValue, f, true);
            if (dt.isValid()) {
                break;
            }
        }

        return dt;
    }

    setDateIfChanged(newDate : moment.Moment){
        const currentDate = this.dateModel.momentObj;
        if (!newDate.isSame(currentDate)) {
            this.setValue(newDate);
            setTimeout(() => this.model = this.dateModel.formatted);
        }

    }


    inputChanged(newValue : string) {

        const dt = this.validateDate(newValue);

        if (dt.isValid()) {
            this.setDateIfChanged(dt);
        }
        else {
            this.setValue(null);
            if (newValue) {
                this.outputEvents.emit({ type: 'dateInvalid', data: newValue });
            }
        }
    }

    

    ngOnInit() {
        this.options = new DatePickerOptions(this.options);
        this.validInputFormats = concat([this.options.format], this.validInputFormats);
       

        if (this.options.initialDate instanceof Date) {
            const initialDate = moment(this.options.initialDate);
            this.selectDate(initialDate);
        }

        this.generateCalendar();
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
                        const date: moment.Moment = this.validateDate(e.data);
                        if (!date.isValid()) {
                            throw new Error(`Invalid date: ${e.data}`);
                        }
                        this.selectDate(date);
                        break;
                    }
                }
            });
        }
    }

    generateCalendar() {
        const date: moment.Moment = moment(this.currentDate);
        const month = date.month();
        const year = date.year();
        let n = 1;
        const firstWeekDay = (this.options.firstWeekdaySunday) ? date.date(2).day() : date.date(1).day();

        if (firstWeekDay !== 1) {
            n -= (firstWeekDay + 6) % 7;
        }

        this.days = [];
        const selectedDate: moment.Moment = date;
        for (let i = n; i <= date.endOf('month').date(); i += 1) {
            const currentDate: moment.Moment = moment(`${i}.${month + 1}.${year}`, 'DD.MM.YYYY');
            const today: boolean = moment().isSame(currentDate, 'day') && moment().isSame(currentDate, 'month');
            const selected: boolean = (selectedDate && selectedDate.isSame(currentDate, 'day'));
           

            const day: ICalendarDate = {
                day: i > 0 ? i : null,
                month: i > 0 ? month : null,
                year: i > 0 ? year : null,
                enabled: i > 0,
                today: i > 0 && today,
                selected: i > 0 && selected,
                momentObj: currentDate
            };

            this.days.push(day);
        }
        this.generateYears();
    }

    setValue(date: moment.Moment) {
        this.dateModel = date ? new DateModel(date, this.options.format) : new DateModel();
        this.generateCalendar();
    }


    selectDate(date: moment.Moment, e?: MouseEvent, ) {
        if (e) { e.preventDefault(); }
        setTimeout(() => {
            this.setValue(date);
            this.model = this.dateModel.formatted;
        });
        this.opened = false;
    }

    selectYear(e: MouseEvent, year: number) {
        e.preventDefault();

        setTimeout(() => {
            const date: moment.Moment = this.dateModelValue.momentObj.year(year);
            this.setValue(date);
            this.model = this.dateModel.formatted;
            this.yearPicker = false;
            this.generateCalendar();
        });
    }

    generateYears() {
        const currentDate =  moment(this.currentDate);
        const fromDate: moment.Moment = moment().year(currentDate.year() - 40);
        const toDate: moment.Moment = moment().year(currentDate.year() + 40);
        const years = toDate.year() - fromDate.year();
        this.years = [];

        for (let i = 0; i < years; i++) {
            this.years.push(fromDate.year());
            fromDate.add(1, 'year');
        }
    }

    writeValue(date: DateModel) {
        if (!date) { return; }
        this.dateModelValue = date;
    }

    prevMonth() {
        const date = this.currentDate.subtract(1, 'month');
        this.setValue(date);
        this.model = this.dateModel.formatted;
        this.generateCalendar();
    }

    nextMonth() {
        const date =  this.currentDate.add(1, 'month');
        this.setValue(date);
        this.model = this.dateModel.formatted;
        this.generateCalendar();
    }

    prevYear() {
        const date = this.currentDate.subtract(1, 'year');
        this.setValue(date);
        this.model = this.dateModel.formatted;
        this.generateCalendar();
    }

    nextYear() {
        const date =  this.currentDate.add(1, 'year');
        this.setValue(date);
        this.model = this.dateModel.formatted;
        this.generateCalendar();
    }


    today() {      
        this.selectDate(moment());
    }

    toggle() {
        const change = this.opened ? this.close : this.open;
        change();
    }

    private open = () => {
        this.opened = true;
        this.yearPicker = false;
        this.outputEvents.emit({ type: 'default', data: 'opened' } as IDatePickerOutputDefaultEvent);
    }

    private close = () => {
        this.opened = false;
        this.outputEvents.emit({ type: 'default', data: 'closed' } as IDatePickerOutputDefaultEvent);
    }

    openYearPicker() {
        setTimeout(() => this.yearPicker = true);
    }

    clear() {
        this.selectDate(null);
        this.close();
    }

    private bSubject: BehaviorSubject<string>;
    private sub : ISubscription;

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.model;
            this.bSubject = new BehaviorSubject(initialValue);

            this.sub = this.bSubject.debounceTime(200).subscribe((data : string) => this.inputChanged(data));
        }

        return this.bSubject;
    }


    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
        safeUnsubscribe(this.eventsSub);
    }

    @ViewChild("inp")
    inputField : ElementRef;

    focus() {
        return focus(this.renderer, this.inputField);
    }
}
