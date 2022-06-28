import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import { DateTime } from 'luxon';
import { debounceTime } from 'rxjs/operators';
import { focus, safeUnsubscribe } from '../helpers-components';
import { BehaviorSubject, SubscriptionLike as ISubscription } from 'rxjs';
import { defaultShortTimeFormat, defaultTimeFormat } from '@nakedobjects/services';

export interface ITimePickerOutputEvent {
    type: 'timeChanged' | 'timeCleared' | 'timeInvalid';
    data: string;
}

export interface ITimePickerInputEvent {
    type: 'setTime';
    data: string;
}

@Component({
    selector: 'nof-time-picker',
    templateUrl: 'time-picker.component.html',
    styleUrls: ['time-picker.component.css']
})
export class TimePickerComponent implements OnInit, OnDestroy {

    @Input()
    inputEvents: EventEmitter<ITimePickerInputEvent>;

    @Output()
    outputEvents: EventEmitter<ITimePickerOutputEvent>;

    @Input()
    id: string;

    @ViewChild('focus', {static: false})
    inputField: ElementRef;

    constructor() {
        this.outputEvents = new EventEmitter<ITimePickerOutputEvent>();
    }

    private timeValue: DateTime | null;
    private modelValue: string;
    private eventsSub: ISubscription;
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

    get time(): DateTime | null {
        return this.timeValue;
    }

    set time(time: DateTime | null) {
        if (time && time.isValid) {
            this.timeValue = time;
            this.outputEvents.emit({ type: 'timeChanged', data: time.toFormat(defaultTimeFormat) });
        }
    }

    private validInputFormats = [defaultTimeFormat, defaultShortTimeFormat];

    private validateTime(newValue: string) {
        let dt = DateTime.now();

        for (const f of this.validInputFormats) {
            dt = DateTime.fromFormat(newValue, f);
            if (dt.isValid) {
                break;
            }
        }

        return dt;
    }

    private sameTime(t1 : DateTime, t2 : DateTime | null) {
        return t2 &&
               t1.hour === t2.hour &&
               t1.minute === t2.minute &&
               t1.second === t2.second;
    }

    setTimeIfChanged(newTime: DateTime) {
        if (!this.sameTime(newTime, this.time)) {
            this.time = newTime;
            setTimeout(() => this.model = newTime.toFormat(defaultShortTimeFormat));
        }
    }

    setTime(newValue: string) {

        if (newValue === '' || newValue == null) {
            this.timeValue = null;
            this.outputEvents.emit({ type: 'timeCleared', data: '' });
        } else {
            const dt = this.validateTime(newValue);

            if (dt.isValid) {
                this.setTimeIfChanged(dt);
            } else {
                this.timeValue = null;
                this.outputEvents.emit({ type: 'timeInvalid', data: newValue });
            }
        }
    }

    inputChanged(newValue: string) {
        this.setTime(newValue);
    }

    ngOnInit() {

        if (this.inputEvents) {
            this.eventsSub = this.inputEvents.subscribe((e: ITimePickerInputEvent) => {
                if (e.type === 'setTime') {
                    this.setTime(e.data);
                }
            });
        }
    }

    clear() {
        this.modelValue = '';
        this.setTime('');
    }

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.model;
            this.bSubject = new BehaviorSubject(initialValue);

            this.sub = this.bSubject.pipe(debounceTime(200)).subscribe((data: string) => this.inputChanged(data));
        }

        return this.bSubject;
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
        safeUnsubscribe(this.eventsSub);
    }

    focus() {
        return focus(this.inputField);
    }
}
