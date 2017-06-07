import { Component, ElementRef, OnInit, Input, Output, EventEmitter } from '@angular/core';
import * as moment from 'moment';
import { BehaviorSubject } from 'rxjs';
import { ISubscription } from 'rxjs/Subscription';
import { safeUnsubscribe } from '../helpers-components'; 


@Component({
  selector: 'nof-time-picker',
  templateUrl: './time-picker.component.html',
  styleUrls: ['./time-picker.component.css']
})
export class TimePickerComponent implements OnInit {

      
    @Input() 
    inputEvents: EventEmitter<{ type: string, data: string  }>;
    
    @Output() 
    outputEvents: EventEmitter<{ type: string, data: string }>;

    @Input()
    id : string;

    constructor(private readonly el: ElementRef) {
        this.outputEvents = new EventEmitter<{ type: string, data: string }>();
    }

    private timeValue: moment.Moment;  
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

    get time(): moment.Moment {
        return this.timeValue;
    }

    set time(time: moment.Moment) {   
        if (time && time.isValid()) { 
            this.timeValue = time;
            this.outputEvents.emit({ type: 'timeChanged', data: time.format("HH:mm:ss") });
        }
    }

    private validInputFormats = ["HH:mm:ss", "HH:mm", "HHmm"];

    private validateTime(newValue: string) {
        let dt: moment.Moment;

        for (let f of this.validInputFormats) {
            dt = moment.utc(newValue, f, true);
            if (dt.isValid()) {
                break;
            }
        }

        return dt;
    }

    setTimeIfChanged(newTime : moment.Moment){
        if (!newTime.isSame(this.time)) {
            this.time = newTime;
            setTimeout(() => this.model = newTime.format("HH:mm"));
        }
    }

    setTime(newValue: string) {

        if (newValue === "" || newValue == null) {
            this.timeValue = null;
            this.outputEvents.emit({ type: 'timeCleared', data: "" });
        }
        else {
            const dt = this.validateTime(newValue);

            if (dt.isValid()) {
                this.setTimeIfChanged(dt);
            }
            else {
                this.timeValue = null;
                this.outputEvents.emit({ type: 'timeInvalid', data: newValue });
            }
        }
    }


    inputChanged(newValue : string) {
        this.setTime(newValue);     
    }

    private eventsSub: ISubscription;

    ngOnInit() {
    
        if (this.inputEvents) {
            this.eventsSub = this.inputEvents.subscribe((e: any) => {
               
                if (e.type === 'setTime') {
                    this.setTime(e.data);
                }
            });
        }
    }

    clear() {
        this.modelValue = "";     
        this.setTime("");
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

}
