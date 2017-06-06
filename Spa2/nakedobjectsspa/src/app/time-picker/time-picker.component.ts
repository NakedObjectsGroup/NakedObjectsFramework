import * as Constants from '../constants';
import { Component, ElementRef, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SlimScrollOptions } from 'ng2-slimscroll';
import * as moment from 'moment';
import concat from 'lodash/concat';
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

    date: moment.Moment;  

    constructor(private readonly el: ElementRef) {
        this.outputEvents = new EventEmitter<{ type: string, data: string }>();
    }

    private validInputFormats = ["HH:mm:ss", "HH:mm", "HHmm"];

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
        const currentDate = this.value;
        if (!newDate.isSame(currentDate)) {
            this.setValue(newDate);
            setTimeout(() => this.formatted = this.value.format("HH:mm"));
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
                this.outputEvents.emit({ type: 'timeInvalid', data: newValue });
            }
        }
    }

    private _formatted : string; 

    set formatted(s: string) {
        this._formatted = s;

        if (this.bSubject) {
            this.bSubject.next(s);
        }
    }

    get formatted(): string {
        return this._formatted;
    }

    get value(): moment.Moment {
        return this.date;
    }

    get currentDate(): moment.Moment {
        return this.date || moment();
    }

    set value(date: moment.Moment) {
        if (date) { 
            this.date = date;
            this.outputEvents.emit({ type: 'timeChanged', data: this.value.format("HH:mm:ss") });
        }
    }

    private eventsSub: ISubscription;

    ngOnInit() {
    
        this.outputEvents.emit({ type: 'default', data: 'init' });

        if (this.inputEvents) {
            this.eventsSub = this.inputEvents.subscribe((e: any) => {
               
                if (e.type === 'setTime') {
                
                    const date: moment.Moment = this.validateDate(e.data);
                    if (!date.isValid()) {
                        throw new Error(`Invalid date: ${e.data}`);
                    }
                    this.selectDate(date);
                }
            });
        }
    }

   

    setValue(date: moment.Moment) {
        this.value = date;
    }


    selectDate(date: moment.Moment, e?: MouseEvent, ) {
        if (e) { e.preventDefault(); }
        setTimeout(() => {
            this.setValue(date);
            this.formatted = this.value.format("HH:mm");
        });
        
    }  

    writeValue(date: moment.Moment) {
        if (!date) { return; }
        this.date = date;
    }

    clear() {
        this.selectDate(null);
        
    }

    private bSubject: BehaviorSubject<string>;
    private sub : ISubscription;

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.formatted;
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
