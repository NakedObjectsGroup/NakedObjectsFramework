import { TimePickerComponent } from '../time-picker/time-picker.component';
import { FieldViewModel } from '../view-models/field-view-model';
import {  ViewChild } from '@angular/core';
import { Component, Input, EventEmitter } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ConfigService} from '../config.service';
import * as Msg from '../user-messages';

@Component({
  selector: 'nof-time-picker-facade',
  templateUrl: './time-picker-facade.component.html',
  styleUrls: ['./time-picker-facade.component.css']
})
export class TimePickerFacadeComponent  {

    // todo make interface for events 
    constructor(private readonly configService : ConfigService) { 
        this.inputEvents = new EventEmitter<{ type: string, data: string }>();
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

    setValueIfChanged(time : string) {      
        const oldValue = this.control.value;
        const newValue = time ? time : "";            

        if (newValue !== oldValue) {
            this.model.resetMessage();
            this.model.clientValid = true;
            this.control.setValue(newValue);  
        }
    }

    handleTimeChangedEvent(time: string) {
        if (this.control) {          
            this.setValueIfChanged(time);      
        }
    }

    handleTimeClearedEvent() {
        if (this.control) {
            this.model.resetMessage();
            this.model.clientValid = true;
            this.control.setValue("");
        }
    }

   handleInvalidTimeEvent(data: string) {
        if (this.control) {
           this.control.setValue("");
           this.model.setMessage(Msg.invalidTime);
           this.model.clientValid = false;
           this.control.setErrors({[Msg.invalidTime]: true});
        }
    }

    
    handleEvents(e: { data: string, type: string }) {
        switch (e.type) {
            case ("timeChanged"):
                this.handleTimeChangedEvent(e.data);
                break;
             case ("timeInvalid"):
                this.handleInvalidTimeEvent(e.data);
                break;
             case ("timeCleared"):
                this.handleTimeClearedEvent();
                break;
            default: //ignore
        }
    }

    inputEvents : EventEmitter<{data : string, type : string}>;

    ngAfterViewInit(): void {
        const existingValue : any = this.control.value;
        if (existingValue && (existingValue instanceof String || typeof existingValue === "string")) {
            setTimeout(() => this.inputEvents.emit({ data: existingValue as string, type: "setTime" }));
        }
    }

    @ViewChild("tp")
    timepicker : TimePickerComponent;

    focus() {
        return this.timepicker.focus();
    }
}
