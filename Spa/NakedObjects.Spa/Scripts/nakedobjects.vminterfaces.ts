
module NakedObjects {
    import Link = Models.Link;
    import Value = Models.Value;
    import scalarValueType = RoInterfaces.scalarValueType;
    import ErrorWrapper = Models.ErrorWrapper;
    import EntryType = NakedObjects.Models.EntryType;
    import Parameter = NakedObjects.Models.Parameter;

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

    export interface IRecentItemViewModel extends ILinkViewModel {
        friendlyName: string;
    }

    export interface  IMessageViewModel {
        clearMessage : () => void ;
        resetMessage : () => void ;
        setMessage: (msg: string) => void;
        getMessage: () => string;
    }

    export interface IFieldViewModel extends IMessageViewModel {
        id: string;
        argId: string;
        paneArgId: string;
        onPaneId: number;

        optional: boolean;
        description: string;
        presentationHint: string;
        mask: string;
        title: string;
        returnType: string;
        format: formatType;
        multipleLines: number;
        password: boolean;
        clientValid: boolean;

        type: "scalar" | "ref";
        reference: string;
        minLength: number;

        color: string;

        isCollectionContributed: boolean;

        promptArguments: _.Dictionary<Value>;

        currentValue: Value;
        originalValue: Value;

        localFilter: ILocalFilter;
        formattedValue: string;

        choices: IChoiceViewModel[];  

        choice: IChoiceViewModel;

        value : scalarValueType | Date;

        multiChoices: IChoiceViewModel[];

        entryType: EntryType;

        validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

        refresh: (newValue: Value) => void;

        prompt: (searchTerm: string) => ng.IPromise<ChoiceViewModel[]>;

        conditionalChoices: (args: _.Dictionary<Value>) => ng.IPromise<ChoiceViewModel[]>;

        setNewValue : (newValue: IDraggableViewModel) => void ;

        drop: (newValue: IDraggableViewModel) => void;

        clear : () => void ; 

        getValue: () => Value;
     
    }

    export interface IParameterViewModel extends IFieldViewModel {
        parameterRep: Parameter;
        dflt: string;
    }

    export interface IPropertyViewModel extends IFieldViewModel {
        propertyRep: Models.PropertyMember;
        isEditable: boolean;
        attachment: IAttachmentViewModel;
        refType: "null" | "navigable" | "notNavigable";
        isDirty: () => boolean;
        doClick: (right?: boolean) => void;
    }
}