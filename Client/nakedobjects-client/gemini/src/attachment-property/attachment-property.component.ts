import { Component, Input } from '@angular/core';
import { ClickHandlerService, ErrorService, ErrorWrapper, UrlManagerService } from '@nakedobjects/services';
import { AttachmentViewModel } from '@nakedobjects/view-models';

@Component({
    selector: 'nof-attachment-property',
    templateUrl: 'attachment-property.component.html',
    styleUrls: ['attachment-property.component.css']
})
export class AttachmentPropertyComponent {

    constructor(
        private readonly error: ErrorService,
        private readonly urlManager: UrlManagerService,
        private readonly clickHandlerService: ClickHandlerService
    ) { }

    private attach: AttachmentViewModel | null = null;

    @Input()
    set attachment(avm: AttachmentViewModel | null) {
        this.attach = avm;
        this.setup();
    }

    get attachment() {
        return this.attach;
    }

    title = 'Empty';
    image?: string;

    doAttachmentClick = (right?: boolean) => {
        if (this.attachment!.empty && !this.image) {
            return;
        }

        if (this.attachment!.displayInline()) {
            this.urlManager.setAttachment(this.attachment!.link, this.clickHandlerService.pane(this.attachment!.onPaneId, right));
        } else {
            this.attachment!.downloadFile()
                .then(blob => {
                    const burl = URL.createObjectURL(blob);
                    window.open(burl);
                })
                .catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }
    };

    private setup() {
        if (this.attachment) {
            if (this.attachment.displayInline()) {
                this.attachment.setImage(this);
            } else {
                this.attachment.setTitle(this);
            }
        }
    }
}
