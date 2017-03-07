import { ContextService } from '../context.service';
import { UrlManagerService } from '../url-manager.service';
import { ClickHandlerService } from "../click-handler.service";
import * as Models from '../models';
import * as Msg from '../user-messages';

export class AttachmentViewModel {

    constructor(
        public readonly link: Models.Link,
        private readonly parent: Models.DomainObjectRepresentation,
        private readonly context: ContextService,
        public readonly onPaneId: number
    ) {
        this.href = link.href();
        this.mimeType = link.type().asString;
        this.title = link.title() || Msg.unknownFileTitle;
    }

    private readonly href: string;
    private readonly mimeType: string;
    readonly title: string;
    readonly downloadFile = () => this.context.getFile(this.parent, this.href, this.mimeType);
    readonly clearCachedFile = () => this.context.clearCachedFile(this.href);

    readonly displayInline = () =>
        this.mimeType === "image/jpeg" ||
        this.mimeType === "image/gif" ||
        this.mimeType === "application/octet-stream";

   
}