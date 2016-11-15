import * as Models from '../models';
import * as Contextservice from '../context.service';
import * as Usermessages from '../user-messages';

export class AttachmentViewModel {
    href: string;
    mimeType: string;
    title: string;
    link: Models.Link;
    onPaneId: number;

    private parent: Models.DomainObjectRepresentation;
    private context: Contextservice.ContextService;

    static create(attachmentLink: Models.Link, parent: Models.DomainObjectRepresentation, context: Contextservice.ContextService, paneId: number) {
        const attachmentViewModel = new AttachmentViewModel();
        attachmentViewModel.link = attachmentLink;
        attachmentViewModel.href = attachmentLink.href();
        attachmentViewModel.mimeType = attachmentLink.type().asString;
        attachmentViewModel.title = attachmentLink.title() || Usermessages.unknownFileTitle;
        attachmentViewModel.parent = parent;
        attachmentViewModel.context = context;
        attachmentViewModel.onPaneId = paneId;
        return attachmentViewModel as AttachmentViewModel;
    }

    downloadFile = () => this.context.getFile(this.parent, this.href, this.mimeType);
    clearCachedFile = () => this.context.clearCachedFile(this.href);

    displayInline = () =>
        this.mimeType === "image/jpeg" ||
        this.mimeType === "image/gif" ||
        this.mimeType === "application/octet-stream";

    doClick: (right?: boolean) => void;
}
