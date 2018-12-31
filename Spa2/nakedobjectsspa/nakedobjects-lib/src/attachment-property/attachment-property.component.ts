import { Component, Input } from '@angular/core';
import { AttachmentViewModel } from '../view-models/attachment-view-model';
import { ErrorService } from '@nakedobjects/services';
import { Router } from '@angular/router';
import { UrlManagerService } from '@nakedobjects/services';
import { ClickHandlerService } from '@nakedobjects/services';
import * as Msg from '../user-messages';
import { ErrorWrapper } from '@nakedobjects/services';

@Component({
    selector: 'nof-attachment-property',
    templateUrl: 'attachment-property.component.html',
    styleUrls: ['attachment-property.component.css']
})
export class AttachmentPropertyComponent {

    constructor(
        private readonly error: ErrorService,
        private readonly urlManager: UrlManagerService,
        private readonly clickHandlerService: ClickHandlerService,
        private readonly router: Router
    ) { }

    private attach: AttachmentViewModel;

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
    noImage = Msg.noImageMessage;

    doAttachmentClick = (right?: boolean) => {
        if (this.attachment.displayInline()) {
            this.urlManager.setAttachment(this.attachment.link, this.clickHandlerService.pane(this.attachment.onPaneId, right));
        } else {
            this.attachment.downloadFile()
                .then(blob => {
                    if (window.navigator.msSaveBlob) {
                        // internet explorer
                        window.navigator.msSaveBlob(blob, this.attachment.title);
                    } else {
                        const burl = URL.createObjectURL(blob);
                        window.open(burl);
                    }
                })
                .catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }
    }

    private setup() {
        if (this.attachment.displayInline()) {
            this.attachment.setImage(this);
        } else {
            this.attachment.setTitle(this);
        }
    }
}
