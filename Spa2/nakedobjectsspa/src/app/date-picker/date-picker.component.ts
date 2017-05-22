import { FieldViewModel } from '../view-models/field-view-model';
import { AfterViewInit, ViewChild } from '@angular/core';
import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import * as Helpers from '../view-models/helpers-view-models';
import * as moment from 'moment';
import { DateModel, Ng2DatePickerComponent } from "../ng2-datepicker/ng2-datepicker.component";

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

    @Input()
    form: FormGroup;

    @Input()
    model: FieldViewModel;

    handleDefaultEvent(data: string) {
        if (this.control) {
            if (data === "closed") {
                const dateModel = this.datepicker.date as DateModel;
                const val = dateModel.momentObj ? dateModel.momentObj.toDate() : "";            
                this.control.setValue(val);                      
            }
        }
    }

    handleDateChangedEvent(data: DateModel) {
        if (this.control) {
            this.model.resetMessage();
            this.model.clientValid = true;

            const date = data.momentObj ? data.momentObj.toDate() : "";
            this.control.setValue(date);
          
        }
    }

   handleInvalidDateEvent(data: string) {
        if (this.control) {
           this.model.setMessage("Invalid date");
           this.model.clientValid = false;
           this.control.setErrors({"Invalid date": true});
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
             case ("dateInvalid"):
                this.handleInvalidDateEvent(e.data as string);
                break;
         
            default: //ignore
        }
    }

    inputEvents : EventEmitter<{data : string | Date, type : string}>;

    private getDateModel(date: moment.Moment) : DateModel {
        return new DateModel(date, this.datepickerConfig.format);     
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

    @ViewChild("dp")
    datepicker : Ng2DatePickerComponent;
}
