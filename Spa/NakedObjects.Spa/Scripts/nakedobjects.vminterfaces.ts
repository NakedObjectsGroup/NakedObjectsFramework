
module NakedObjects {
    import Link = Models.Link;
    import Value = Models.Value;

    export interface IAttachmentViewModel {
        href: string;
        mimeType: string;
        title: string;
        link: Link;
        onPaneId: number;

        downloadFile: () => ng.IPromise<Blob>;
        clearCachedFile: () => void;
        displayInline: () => boolean;
        doClick: (right?: boolean) => void;
    }

    export interface IChoiceViewModel {
        name: string;

        getValue: () => Value;
        equals: (other: IChoiceViewModel) => boolean;
        valuesEqual: (other: IChoiceViewModel) => boolean;
    }

}