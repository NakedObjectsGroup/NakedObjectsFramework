import { ContextService } from '../context.service';
import * as Models from '../models';
import * as Msg from '../user-messages';

export class AttachmentViewModel {
    href: string;
    mimeType: string;
    title: string;
    link: Models.Link;
    onPaneId: number;

    private parent: Models.DomainObjectRepresentation;
    private context: ContextService;

    static create(attachmentLink: Models.Link, parent: Models.DomainObjectRepresentation, context: ContextService, paneId: number) {
        const attachmentViewModel = new AttachmentViewModel();
        attachmentViewModel.link = attachmentLink;
        attachmentViewModel.href = attachmentLink.href();
        attachmentViewModel.mimeType = attachmentLink.type().asString;
        attachmentViewModel.title = attachmentLink.title() || Msg.unknownFileTitle;
        attachmentViewModel.parent = parent;
        attachmentViewModel.context = context;
        attachmentViewModel.onPaneId = paneId;
        return attachmentViewModel;
    }

    downloadFile = () => this.context.getFile(this.parent, this.href, this.mimeType);
    clearCachedFile = () => this.context.clearCachedFile(this.href);

    displayInline = () =>
        this.mimeType === "image/jpeg" ||
        this.mimeType === "image/gif" ||
        this.mimeType === "application/octet-stream";

    doClick: (right?: boolean) => void;
}