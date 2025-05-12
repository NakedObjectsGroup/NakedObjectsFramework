import { Component, HostListener, Input, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { copy, DragAndDropService, AttachmentViewModel, PropertyViewModel } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';

@Component({
    selector: 'nof-view-property',
    templateUrl: 'view-property.component.html',
    styleUrls: ['view-property.component.css'],
    standalone: false
})
export class ViewPropertyComponent implements OnInit, OnDestroy {

    private ddSub?: ISubscription;

    constructor(
        private readonly dragAndDrop: DragAndDropService,
        private readonly router: Router,
    ) { }

    dropZones: string[] = [];

    // template inputs

    @Input()
    property?: PropertyViewModel;

    @Input()
    propertyName?: string;

    // template listeners

    @HostListener('keydown', ['$event'])
    onEnter(event: KeyboardEvent) {
        this.copy(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.copy(event);
    }

    // template API

    get title() {
        return this.property?.title ?? this.propertyName;
    }

    get propertyType() {
        return this.property?.type ?? 'scalar';
    }

    get propertyRefType() {
        return this.property?.refType;
    }

    get propertyReturnType() {
        return this.property?.returnType ?? '';
    }

    get formattedValue() {
        return this.property?.formattedValue ?? '';
    }

    get value() {
        return this.property?.value;
    }

    get format() {
        return this.property?.format;
    }

    get isBlob() {
        return this.property?.format === 'blob';
    }

    get isMultiline() {
        return !(this.property?.multipleLines === 1);
    }

    get multilineHeight() {
        return `${(this.property?.multipleLines ?? 1) * 20}px`;
    }

    get color() {
        return this.property?.color;
    }

    get attachment(): AttachmentViewModel | null {
        return this.property?.attachment ?? null;
    }

    get isEditByAction() {
        return this.property?.isEditByAction ?? false;
    }

    get editActionTooltip() {
        return this.property?.editActionTooltip;
    }

    get isLink() {
        return this.property?.isLink;
    }

    get linkValue() {
        const lp = this.property?.linkProperties();
        return lp?.[1] || this.value;
    }

    doLinkClick(newPane?: boolean) {
        const href = this.value as string;
        if (newPane || this.property?.linkProperties()?.[0]) {
            window.open(href, '_blank');
        }
        else if (href.startsWith('http')) {
            window.location.href = href;
        }
        else {  
            const tree = this.router.createUrlTree([href]);
            this.router.navigateByUrl(tree);
        }
    }

    doClick = (right?: boolean) => this.property?.doClick(right);

    doEdit = () => {
        if (!this.property?.isEditActionDisabled) {
          this.property?.doEditByAction();
        }
    };

    copy(event: KeyboardEvent) {
        const prop = this.property;
        if (prop) {
            copy(event, prop, this.dragAndDrop);
        }
    }

    setDropZones(ids: string[]) {
        setTimeout(() => this.dropZones = ids);
    }

    get editActionClass() {
        return ({
            tempdisabled: this.property?.isEditActionDisabled,
        });
    }

    ngOnInit(): void {
        this.ddSub = this.dragAndDrop.dropZoneIds$.subscribe(ids => this.setDropZones(ids || []));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.ddSub);
    }
}
