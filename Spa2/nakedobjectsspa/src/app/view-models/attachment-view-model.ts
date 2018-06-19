import { ContextService } from '../context.service';
import * as Models from '../models';
import * as Msg from '../user-messages';
import { ErrorService } from '../error.service';
import { Pane } from '../route-data';

export class AttachmentViewModel {

    constructor(
        public readonly link: Models.Link,
        private readonly parent: Models.DomainObjectRepresentation,
        private readonly context: ContextService,
        private readonly error: ErrorService,
        public readonly onPaneId: Pane
    ) {
        this.href = link.href();
        this.mimeType = link.type().asString;
        this.title = link.title() || Msg.unknownFileTitle;
    }

    private readonly href: string;
    private readonly mimeType: string;
    empty = false;
    title: string;
    readonly downloadFile = () => this.context.getFile(this.parent, this.href, this.mimeType);
    readonly clearCachedFile = () => this.context.clearCachedFile(this.href);

    readonly displayInline = () =>
        this.mimeType === "image/jpeg" ||
        this.mimeType === "image/gif" ||
        this.mimeType === "application/octet-stream"

    setImage(setImageOn: { image: string, title: string }) {
        this.downloadFile().then(blob => {
            if (blob.size > 0) {
                const reader = new FileReader();
                reader.onloadend = () => {
                    if (reader.result) {
                        setImageOn.image = reader.result;
                    }
                };
                reader.readAsDataURL(blob);
            } else {
                this.empty = true;
                this.title = Msg.noImageMessage;
            }
            setImageOn.title = this.title;
        }).catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    setTitle(setTitleOn: {title: string }) {
         setTitleOn.title = this.title;
    }
}
