import { Component, Input, ElementRef, OnInit, HostListener, ViewChildren, QueryList } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as Models from "../models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorService } from "../error.service";
import { ContextService } from "../context.service";
import { AttachmentViewModel } from '../view-models/attachment-view-model';
import * as Fieldviewmodel from '../view-models/field-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';


@Component({
    host: {
        '(document:click)': 'handleClick($event)'
    },
    selector: 'edit-property',
    templateUrl: './edit-property.component.html',
    styleUrls: ['./edit-property.component.css']
})
export class EditPropertyComponent extends FieldComponent implements OnInit {

    constructor(myElement: ElementRef,
        private router: Router,
        private error: ErrorService,
        context: ContextService) {
        super(myElement, context);
    }

    prop: PropertyViewModel;

    @Input()
    parent: DomainObjectViewModel;

    @Input()
    set property(value: PropertyViewModel) {
        this.prop = value;
        this.droppable = value as Fieldviewmodel.FieldViewModel;
    }

    get property() {
        return this.prop;
    }

    classes(): string {
        return `${this.prop.color}${this.canDrop ? " candrop" : ""}`;
    }

    @Input()
    set form(fm: FormGroup) {
        this.formGroup = fm;
    }

    get form() {
        return this.formGroup;
    }

    ngOnInit(): void {
        super.init(this.parent, this.property, this.form.controls[this.prop.id]);

        // todo this is cloned across view/edit property types - DRY it!!!!! 
        if (this.property && this.property.attachment) {
            const attachment = this.property.attachment;

            this.attachmentTitle = attachment.title;

            if (attachment.displayInline()) {
                attachment.downloadFile()
                    .then(blob => {
                        const reader = new FileReader();
                        reader.onloadend = () => {
                            if (reader.result) {
                                this.image = reader.result;
                            }
                        };
                        reader.readAsDataURL(blob);
                    })
                    .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            } else {
                attachment.doClick = this.clickHandler(attachment);
            }

        } else {
            this.attachmentTitle = "Attachment not yet supported on transient";
        }
    }

    datePickerChanged(evt: any) {
        const val = evt.currentTarget.value;
        this.formGroup.value[this.property.id] = val;
    }

    clickHandler(attachment: AttachmentViewModel) {

        return () => {

            if (!attachment.displayInline()) {
                attachment.downloadFile()
                    .then(blob => {
                        if (window.navigator.msSaveBlob) {
                            // internet explorer 
                            window.navigator.msSaveBlob(blob, attachment.title);
                        } else {
                            const burl = URL.createObjectURL(blob);
                            this.router.navigateByUrl(burl);
                        }
                    })
                    .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            }

            return false;
        };
    };

    attachmentTitle: string;
    image: string;

    @HostListener('keydown', ['$event'])
    onEnter(event: KeyboardEvent) {
        this.paste(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.paste(event);
    }

    @ViewChildren("focus")
    focusList: QueryList<ElementRef>;
}