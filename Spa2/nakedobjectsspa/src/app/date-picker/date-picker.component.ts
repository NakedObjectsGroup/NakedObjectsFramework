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

    constructor(
        private readonly el: ElementRef,
        private readonly renderer: Renderer
    ) {
        this.opened = false;
        this.options = this.options || {};
        this.days = [];
        this.dateModelValue = null;

        this.outputEvents = new EventEmitter<IDatePickerOutputEvent>();
    }

    private validInputFormats = ["DD/MM/YYYY", "DD/MM/YY", "D/M/YY", "D/M/YYYY", "D MMM YYYY", "D MMMM YYYY", Constants.fixedDateFormat];

    private dateModelValue: moment.Moment;
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

    get dateModel(): moment.Moment {
        return this.dateModelValue;
    }

    get currentDate(): moment.Moment {
        return this.dateModelValue || moment().utc();
    }

    set dateModel(date: moment.Moment) {
        if (date) { 
            this.dateModelValue = date;
            this.outputEvents.emit({ type: 'dateChanged', data: this.dateModel });
        }
        else {
            this.dateModelValue = null;
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
        const currentDate = this.dateModel;
        if (!newDate.isSame(currentDate)) {
            this.setValue(newDate);
            setTimeout(() => this.model = this.dateModel.format(this.options.format));
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
            const date: moment.Moment = moment.utc(`${i}.${month + 1}.${year}`, 'DD.MM.YYYY');
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

    setValue(date: moment.Moment) {
        this.dateModel = date;
    }

    private formatDate(date : moment.Moment) {
        return this.dateModel.format(this.options.format);
    }

    selectDate(date: moment.Moment, e?: MouseEvent, ) {
        if (e) { e.preventDefault(); }
        setTimeout(() => {
            this.setValue(date);
            this.model = this.formatDate(this.dateModel);
        });
        this.opened = false;
    }

    writeValue(date: moment.Moment) {
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

    private bSubject: BehaviorSubject<string>;
    private sub : ISubscription;

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.model;
            this.bSubject = new BehaviorSubject(initialValue);

            this.sub = this.bSubject.debounceTime(1000).subscribe((data : string) => this.inputChanged(data));
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
