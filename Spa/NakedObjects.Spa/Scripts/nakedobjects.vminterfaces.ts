
module NakedObjects {
    import Link = Models.Link;
    import Value = Models.Value;
    import scalarValueType = RoInterfaces.scalarValueType;
    import ErrorWrapper = Models.ErrorWrapper;
    import EntryType = Models.EntryType;
    import Parameter = Models.Parameter;
    import IListRepresentation = RoInterfaces.IListRepresentation;
    import ListRepresentation = NakedObjects.Models.ListRepresentation;
    import IActionRepresentation = NakedObjects.RoInterfaces.IActionRepresentation;
    import ActionMember = NakedObjects.Models.ActionMember;
    import ActionRepresentation = NakedObjects.Models.ActionRepresentation;

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

        currentValue: Value;
        originalValue: Value;

        localFilter: ILocalFilter;
        formattedValue: string;

        // todo this is current choice options - rename for clarity - careful used in templates !
        choices: IChoiceViewModel[];  

        value : scalarValueType | Date;

        // todo this is really selected choice/selected choices - rename for clarity - careful used in templates !
        choice: IChoiceViewModel;
        multiChoices: IChoiceViewModel[];

        entryType: EntryType;

        validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

        refresh: (newValue: Value) => void;

        prompt: (searchTerm: string) => ng.IPromise<ChoiceViewModel[]>;
        promptArguments: _.Dictionary<Value>;

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

    export interface IActionViewModel {
        actionRep: Models.ActionMember | Models.ActionRepresentation;
        invokableActionRep: Models.IInvokableAction;

        menuPath: string;
        title: string;
        description: string;
        presentationHint: string;

        // doInvoke is called from template 
        doInvoke: (right?: boolean) => void;
        execute: (pps: IParameterViewModel[], right?: boolean) => ng.IPromise<Models.ActionResultRepresentation>;
        disabled: () => boolean;
        parameters: () => IParameterViewModel[];
        makeInvokable: (details: Models.IInvokableAction) => void;
    }

    export interface IMenuItemViewModel {
        name : string;
        actions: IActionViewModel[];
        menuItems: IMenuItemViewModel[];
    }

    export interface IDialogViewModel extends IMessageViewModel {
        actionViewModel: IActionViewModel;
        title: string;
        id: string;
        parameters: IParameterViewModel[];

        reset: (actionViewModel: IActionViewModel, routeData: PaneRouteData) => void;
        refresh: () => void;
        deregister: () => void;
        clientValid: () => boolean;
        tooltip: () => void;
        setParms: () => void;
        doInvoke: (right?: boolean) => void;
        doCloseKeepHistory: () => void;
        doCloseReplaceHistory: () => void;
        clearMessages: () => void;
    }

    export interface ICollectionPlaceholderViewModel {
        description: () => string;
        reload: () => void;
    }

    export interface IListViewModel extends IMessageViewModel {
        id: string;
        listRep: ListRepresentation;
        size: number;
        pluralName: string;
        header: string[];
        items: IItemViewModel[];
        actions: IActionViewModel[];
        menuItems: IMenuItemViewModel[];

        description: () => string;
        refresh : (routeData: PaneRouteData) => void;
        reset : (list: ListRepresentation, routeData: PaneRouteData) => void;
        toggleActionMenu : () => void;
        pageNext  : () => void;
        pagePrevious : () => void;
        pageFirst : () => void;
        pageLast : () => void;
        doSummary: () => void;
        doList: () => void;
        doTable: () => void;
        reload: () => void;
        selectAll: () => void;
        disableActions: () => void;
        actionsTooltip : () => string;
        actionMember : (id: string) => ActionMember | ActionRepresentation;
    }

}