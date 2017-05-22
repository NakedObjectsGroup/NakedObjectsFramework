import { Component, ElementRef, Inject, OnInit, Input, Output, EventEmitter, } from '@angular/core';
import { SlimScrollOptions } from 'ng2-slimscroll';
import * as moment from 'moment';
import { FieldViewModel } from '../view-models/field-view-model';

export interface IDateModel {
    day: string;
    month: string;
    year: string;
    formatted: string;
    momentObj: moment.Moment;
}

export class DateModel implements IDateModel {
    day: string;
    month: string;
    year: string;
    formatted: string;
    momentObj: moment.Moment;

    private initFromDate(date: moment.Moment, format: string) {
        this.day = date.format('DD');
        this.month = date.format('MM');
        this.year = date.format('YYYY');
        this.formatted = date.format(format);
        this.momentObj = date;
    }

    private initFromDateModel(dateModel: IDateModel) {
        this.day = dateModel && dateModel.day ? dateModel.day : null;
        this.month = dateModel && dateModel.month ? dateModel.month : null;
        this.year = dateModel && dateModel.year ? dateModel.year : null;
        this.formatted = dateModel && dateModel.formatted ? dateModel.formatted : "";
        this.momentObj = dateModel && dateModel.momentObj ? dateModel.momentObj : null;
    }

    constructor(obj?: IDateModel | moment.Moment, format?: string) {
        if (obj && format) {
            this.initFromDate(obj as moment.Moment, format);
        }
        else {
            this.initFromDateModel(obj as IDateModel);
        }
    }
}


export class DatePickerOptions {
    autoApply?: boolean;
    style?: 'normal' | 'big' | 'bold';
    locale?: string;
    minDate?: Date;
    maxDate?: Date;
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
        this.minDate = obj && obj.minDate ? obj.minDate : null;
        this.maxDate = obj && obj.maxDate ? obj.maxDate : null;
        this.initialDate = obj && obj.initialDate ? obj.initialDate : null;
        this.firstWeekdaySunday = obj && obj.firstWeekdaySunday ? obj.firstWeekdaySunday : false;
        this.format = obj && obj.format ? obj.format : 'YYYY-MM-DD';
        this.selectYearText = obj && obj.selectYearText ? obj.selectYearText : 'Select Year';
        this.todayText = obj && obj.todayText ? obj.todayText : 'Today';
        this.clearText = obj && obj.clearText ? obj.clearText : 'Clear';
    }
}

export interface CalendarDate {
    day: number;
    month: number;
    year: number;
    enabled: boolean;
    today: boolean;
    selected: boolean;
    momentObj: moment.Moment;
}

@Component({
    selector: 'nof-ng2-datepicker',
    templateUrl: './ng2-datepicker.component.html',
    styleUrls: ['./ng2-datepicker.component.sass']
})
export class Ng2DatePickerComponent implements OnInit {
    @Input() options: DatePickerOptions;
    @Input() inputEvents: EventEmitter<{ type: string, data: string | DateModel }>;
    @Output() outputEvents: EventEmitter<{ type: string, data: string | DateModel }>;

    date: DateModel;

    opened: boolean;
    days: CalendarDate[];
    years: number[];
    yearPicker: boolean;
    scrollOptions: SlimScrollOptions;

    minDate: moment.Moment | any;
    maxDate: moment.Moment | any;

    constructor( @Inject(ElementRef) public el: ElementRef) {
        this.opened = false;
        this.options = this.options || {};
        this.days = [];
        this.years = [];
        this.date = new DateModel();

        this.outputEvents = new EventEmitter<{ type: string, data: string | DateModel }>();
    }

    inputChanged(newValue: string) {

        const dt = moment(newValue, this.options.format, true);
        if (dt.isValid()) {
            this.setValue(dt)
        }
        else {
            this.setValue(null);
            if (newValue) {
                this.outputEvents.emit({ type: 'dateInvalid', data: newValue });
            }
        }
    }

    get formatted() : string {
        return this.date.formatted;
    }

    get value(): DateModel {
        return this.date;
    }

    get currentDate(): moment.Moment {
        return this.date.momentObj || moment();
    }

    set value(date: DateModel) {
        if (date) { 
            this.date = date;
            this.outputEvents.emit({ type: 'dateChanged', data: this.value });
        }
    }

    ngOnInit() {
        this.options = new DatePickerOptions(this.options);
        this.scrollOptions = {
            barBackground: '#C9C9C9',
            barWidth: '7',
            gridBackground: '#C9C9C9',
            gridWidth: '2'
        };

        if (this.options.initialDate instanceof Date) {
            const initialDate = moment(this.options.initialDate);
            this.selectDate(initialDate);
        }

        if (this.options.minDate instanceof Date) {
            this.minDate = moment(this.options.minDate);
        } else {
            this.minDate = null;
        }

        if (this.options.maxDate instanceof Date) {
            this.maxDate = moment(this.options.maxDate);
        } else {
            this.maxDate = null;
        }

        this.generateYears();
        this.generateCalendar();
        this.outputEvents.emit({ type: 'default', data: 'init' });

        if (this.inputEvents) {
            this.inputEvents.subscribe((e: any) => {
                if (e.type === 'action') {
                    if (e.data === 'toggle') {
                        this.toggle();
                    }
                    if (e.data === 'close') {
                        this.close();
                    }
                    if (e.data === 'open') {
                        this.open();
                    }
                }

                if (e.type === 'setDate') {
                    if (!(e.data instanceof Date)) {
                        throw new Error(`Input data must be an instance of Date!`);
                    }
                    const date: moment.Moment = moment(e.data);
                    if (!date) {
                        throw new Error(`Invalid date: ${e.data}`);
                    }
                    this.selectDate(date);
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
            let betweenMinMax = true;

            if (this.minDate !== null) {
                if (this.maxDate !== null) {
                    betweenMinMax = currentDate.isBetween(this.minDate, this.maxDate, 'day', '[]');
                } else {
                    betweenMinMax = !currentDate.isBefore(this.minDate, 'day');
                }
            } else {
                if (this.maxDate !== null) {
                    betweenMinMax = !currentDate.isAfter(this.maxDate, 'day');
                }
            }

            const day: CalendarDate = {
                day: i > 0 ? i : null,
                month: i > 0 ? month : null,
                year: i > 0 ? year : null,
                enabled: i > 0 ? betweenMinMax : false,
                today: i > 0 && today,
                selected: i > 0 && selected,
                momentObj: currentDate
            };

            this.days.push(day);
        }
    }

    setValue(date: moment.Moment) {
        this.value = date ? new DateModel(date, this.options.format) : new DateModel();
        this.generateCalendar();
    }


    selectDate(date: moment.Moment, e?: MouseEvent, ) {
        if (e) { e.preventDefault(); }
        setTimeout(() => this.setValue(date));
        this.opened = false;
    }

    selectYear(e: MouseEvent, year: number) {
        e.preventDefault();

        setTimeout(() => {
            const date: moment.Moment = this.date.momentObj.year(year);
            this.setValue(date);
            this.yearPicker = false;
            this.generateCalendar();
        });
    }

    generateYears() {
        const date: moment.Moment = this.minDate || moment().year(moment().year() - 40);
        const toDate: moment.Moment = this.maxDate || moment().year(moment().year() + 40);
        const years = toDate.year() - date.year();

        for (let i = 0; i < years; i++) {
            this.years.push(date.year());
            date.add(1, 'year');
        }
    }

    writeValue(date: DateModel) {
        if (!date) { return; }
        this.date = date;
    }

    prevMonth() {
        const date = this.currentDate.subtract(1, 'month');
        this.setValue(date);
        this.generateCalendar();
    }

    nextMonth() {
        const date =  this.currentDate.add(1, 'month');
        this.setValue(date);
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
        this.outputEvents.emit({ type: 'default', data: 'opened' });
    }

    private close = () => {
        this.opened = false;
        this.outputEvents.emit({ type: 'default', data: 'closed' });
    }

    openYearPicker() {
        setTimeout(() => this.yearPicker = true);
    }

    clear() {
        this.selectDate(null);
        this.close();
    }
}
