import { Component, Input, ElementRef, OnInit, HostListener } from '@angular/core';
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorService } from "../error.service";
import { ContextService } from "../context.service";
import { AttachmentViewModel } from '../view-models/attachment-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import * as Models from "../models";

@Component({
    selector: 'view-property',
    templateUrl: './view-property.component.html',
    styleUrls: ['./view-property.component.css']
})
export class ViewPropertyComponent implements OnInit {

    constructor(
        private readonly router: Router,
        private readonly error: ErrorService,
        private readonly context: ContextService
    ) { }

    // template inputs 

    @Input()
    property: PropertyViewModel;

    // template listeners 

    @HostListener('keydown', ['$event'])
    onEnter(event: KeyboardEvent) {
        this.cut(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.cut(event);
    }

    // template API

    get title() {
        return this.property.title;
    }

    get propertyType() {
        return this.property.type;
    }

    get propertyRefType() {
        return this.property.refType;
    }

    get propertyReturnType() {
        return this.property.returnType;
    }

    get formattedValue() {
        return this.property.formattedValue;
    }

    get value() {
        return this.property.value;
    }

    get format() {
        return this.property.format;
    }

    get isBlob() {
        return this.property.format === "blob";
    }

    get isMultiline() {
        return !(this.property.multipleLines === 1);
    }

    get multilineHeight() {
        return `${this.property.multipleLines * 20}px`;
    }

    get color() {
        return this.property.color;
    }

    doClick = (right?: boolean) => this.property.doClick(right);

    // todo delegated click here smell that we need another component 
    doAttachmentClick = (right?: boolean) => {
        const attachment = this.property.attachment;
        if (attachment) {
            attachment.doClick(right);
        };
    }

    attachmentTitle: string;
    image: string;

    // todo maybe two different click handlers on attachment view model ? 
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

    ngOnInit() {

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
            // todo use msg string 
            this.attachmentTitle = "Attachment not yet supported on transient";
        }
    }

    // todo DRY and rename this !!
    cut(event: any) {
        const cKeyCode = 67;
        if (event && (event.keyCode === cKeyCode && event.ctrlKey)) {
            this.context.setCutViewModel(this.property);
            event.preventDefault();
        }
    }
}