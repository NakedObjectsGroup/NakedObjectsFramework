import { AfterViewInit } from '@angular/core/core';
import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { DateModel } from 'ng2-datepicker';
import * as Helpers from '../view-models/helpers-view-models';
import * as moment from 'moment';

@Component({
    selector: 'nof-date-picker',
    template: require('./date-picker.component.html'),
    styles: [require('./date-picker.component.css')]
})
export class DatePickerComponent implements AfterViewInit {

    constructor() { 
        this.inputEvents = new EventEmitter<{ type: string, data: string | Date }>();
    }

    @Input()
    control: AbstractControl;

    
    handleDefaultEvent(data: string) {
        if (this.control) {
            // if (data ="closed") {
            //    this.control.setValue("");
            // }
        }
    }

    handleDateChangedEvent(data: DateModel) {
        if (this.control) {
            const date = data.momentObj.toDate();
            this.control.setValue(date);
        }
    }

    handleEvents(e: { data: DateModel | string, type: string }) {
        switch (e.type) {
            case ("default"):
                this.handleDefaultEvent(e.data as string);
                break;
            case ("dateChanged"):
                this.handleDateChangedEvent(e.data as DateModel);
                break;
            default: //ignore
        }
    }

    inputEvents : EventEmitter<{data : string | Date, type : string}>;

    private getDateModel(date: moment.Moment) : DateModel {
        return {
            day: date.format('DD'),
            month: date.format('MM'),
            year: date.format('YYYY'),
            formatted: date.format(this.datepickerConfig.format),
            momentObj: date
        };
    }

    ngAfterViewInit(): void {
        const existingValue = this.control.value;
        if (existingValue && (existingValue instanceof String || typeof existingValue === "string")) {
            const date = Helpers.getDate(existingValue as string);
            if (date) {
                const dModel = this.getDateModel(moment(date));
                setTimeout(() => this.inputEvents.emit({ data: date, type: "setDate" }));
            }
        }
    }


    datepickerConfig = { format: "D MMM YYYY" }

}
