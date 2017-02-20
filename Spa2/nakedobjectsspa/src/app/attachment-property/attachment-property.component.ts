import { Component, OnInit, Input } from '@angular/core';
import { AttachmentViewModel} from '../view-models/attachment-view-model';
import * as Models from '../models';
import { ErrorService } from '../error.service';
import { Router } from '@angular/router';

@Component({
    selector: 'nof-attachment-property',
    template: require('./attachment-property.component.html'),
    styles: [require('./attachment-property.component.css')]
})
export class AttachmentPropertyComponent {

    constructor(
        private readonly error: ErrorService,
        private readonly router: Router
    ) { }

    private attach : AttachmentViewModel;

    @Input()
    set attachment(avm: AttachmentViewModel) {
        this.attach = avm;
        if (this.attach) {
            this.setup();
        }
    }

    get attachment() {
        return this.attach;
    }

    title: string;
    image: string;

    doAttachmentClick = (right?: boolean) => this.attachment.doClick(right);

    private clickHandler(attachment: AttachmentViewModel) {

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

    private setup() {

        this.title = this.attachment.title;

        if (this.attachment.displayInline()) {
            this.attachment.downloadFile()
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
            this.attachment.doClick = this.clickHandler(this.attachment);
        }
    }
}
