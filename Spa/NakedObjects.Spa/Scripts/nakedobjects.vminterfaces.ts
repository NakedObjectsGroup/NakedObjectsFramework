
module NakedObjects {
    import Link = Models.Link;
    import Value = Models.Value;
    import scalarValueType = RoInterfaces.scalarValueType;
    import ErrorWrapper = Models.ErrorWrapper;

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

    export interface IDraggableViewModel {       
        value: scalarValueType | Date;
        reference: string;
        choice: IChoiceViewModel;
        color: string;
        draggableType: string;

        canDropOn: (targetType: string) => ng.IPromise<boolean>;
    }

    export interface IErrorViewModel {
        originalError: ErrorWrapper;
        title: string;
        message: string;
        stackTrace: string[];
        errorCode: string;
        description: string;
        isConcurrencyError: boolean;
    }

    export interface ILinkViewModel {
        title: string;
        domainType: string;   
        link: Link;

        doClick: (right?: boolean) => void;
    }

    export interface IItemViewModel extends ILinkViewModel {
        tableRowViewModel: TableRowViewModel;
        selected: boolean;

        selectionChange: (index: number) => void;
    }

}