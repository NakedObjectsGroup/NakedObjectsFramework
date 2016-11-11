import { Component, Input, ElementRef, OnInit, HostListener } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorService } from "../error.service";
import { ContextService } from "../context.service";

@Component({
    selector: 'view-property',
    templateUrl: './view-property.component.html',
    styleUrls: ['./view-property.component.css']
})
export class ViewPropertyComponent implements OnInit {

    constructor(private router: Router,
        private error: ErrorService,
        private context: ContextService) {
    }


    @Input()
    property: ViewModels.PropertyViewModel;

    classes(): string {
        return `${this.property.color}`;
    }

    clickHandler(attachment: ViewModels.IAttachmentViewModel) {

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
            this.attachmentTitle = "Attachment not yet supported on transient";
        }
    }

    cut(event: any) {
        const cKeyCode = 67;
        if (event && (event.keyCode === cKeyCode && event.ctrlKey)) {
            this.context.setCutViewModel(this.property);
            event.preventDefault();
        }
    }

    @HostListener('keydown', ['$event'])
    onEnter(event: KeyboardEvent) {
        this.cut(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.cut(event);
    }

    attachmentTitle: string;
    image: string;
}