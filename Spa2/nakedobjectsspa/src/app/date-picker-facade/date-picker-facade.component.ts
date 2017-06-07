import { FieldViewModel } from '../view-models/field-view-model';
import { AfterViewInit, ViewChild } from '@angular/core';
import { Component, Input, EventEmitter } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import * as moment from 'moment';
import { DateModel, DatePickerComponent, DatePickerOptions } from "../date-picker/date-picker.component";
import { ConfigService} from '../config.service';
import * as Constants from '../constants';
import * as Msg from '../user-messages';

@Component({
    selector: 'nof-date-picker-facade',
    template: require('./date-picker-facade.component.html'),
    styles: [require('./date-picker-facade.component.css')]
})
export class DatePickerFacadeComponent implements AfterViewInit {

    // todo make interface for events 
    constructor(private readonly configService : ConfigService) { 
        this.inputEvents = new EventEmitter<{ type: string, data: string | Date }>();
        this.datePickerOptions.format = configService.config.dateInputFormat;
    }

    @Input()
    control: AbstractControl;

    @Input()
    form: FormGroup;

    @Input()
    model: FieldViewModel;

    get id() {
        return this.model.paneArgId;
    }

    setValueIfChanged(dateModel : DateModel) {
        const oldValue = this.control.value;
        const newValue = dateModel.momentObj ? dateModel.momentObj.format(Constants.fixedDateFormat) : "";            

        if (newValue !== oldValue) {
            this.model.resetMessage();
            this.model.clientValid = true;
            this.control.setValue(newValue);  
        }
    }

    handleDefaultEvent(data: string) {
        if (this.control) {
            if (data === "closed") {
                const dateModel = this.datepicker.date;
                this.setValueIfChanged(dateModel);                      
            }
        }
    }

    handleDateChangedEvent(dateModel: DateModel) {
        if (this.control) {          
            this.setValueIfChanged(dateModel);      
        }
    }

   handleInvalidDateEvent(data: string) {
        if (this.control) {
           this.model.setMessage(Msg.invalidDate);
           this.model.clientValid = false;
           this.control.setErrors({[Msg.invalidDate]: true});
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
        return new DateModel(date, this.datePickerOptions.format);     
    }

    ngAfterViewInit(): void {
        const existingValue : any = this.control.value;
        if (existingValue && (existingValue instanceof String || typeof existingValue === "string")) {
            setTimeout(() => this.inputEvents.emit({ data: existingValue as string, type: "setDate" }));
        }
    }

    datePickerOptions = new DatePickerOptions();

    @ViewChild("dp")
    datepicker : DatePickerComponent;

    focus() {
        return this.datepicker.focus();
    }
}
