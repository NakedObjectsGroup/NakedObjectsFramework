
namespace NakedObjects {
    import Link = Models.Link;
    import Value = Models.Value;
    import scalarValueType = RoInterfaces.scalarValueType;
    import ErrorWrapper = Models.ErrorWrapper;
    import EntryType = Models.EntryType;
    import Parameter = Models.Parameter;
    import ListRepresentation = Models.ListRepresentation;
    import ActionMember = Models.ActionMember;
    import ActionRepresentation = Models.ActionRepresentation;
    import MenusRepresentation = Models.MenusRepresentation;
    import IVersionRepresentation = RoInterfaces.IVersionRepresentation;
    import IUserRepresentation = RoInterfaces.IUserRepresentation;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ErrorMap = Models.ErrorMap;

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
        selectedChoice: IChoiceViewModel;
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
        tableRowViewModel: ITableRowViewModel;
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

        choices: IChoiceViewModel[];  

        value : scalarValueType | Date;

        selectedChoice: IChoiceViewModel;
        selectedMultiChoices: IChoiceViewModel[];

        entryType: EntryType;

        validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

        refresh: (newValue: Value) => void;

        prompt: (searchTerm: string) => ng.IPromise<ChoiceViewModel[]>;
        promptArguments: _.Dictionary<Value>;

        conditionalChoices: (args: _.Dictionary<Value>) => ng.IPromise<ChoiceViewModel[]>;

        setNewValue : (newValue: IDraggableViewModel) => void ;

        drop: (newValue: IDraggableViewModel) => void;

        clear : () => void; 

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

    export interface ICollectionViewModel {
        title: string;
        details: string;
        pluralName: string;
        color: string;
        mayHaveItems: boolean;
        items: IItemViewModel[];
        header: string[];
        onPaneId: number;
        currentState: CollectionViewState;
        presentationHint: string;
        template: string;
        actions: IActionViewModel[];
        menuItems: IMenuItemViewModel[];
        messages: string;
        collectionRep: Models.CollectionMember | Models.CollectionRepresentation;

        doSummary: () => void;
        doTable: () => void;
        doList: () => void;

        description: () => string;
        refresh: (routeData: PaneRouteData, resetting: boolean) => void;
    }

    export interface IMenusViewModel {
        reset: (menusRep: MenusRepresentation, routeData: PaneRouteData) => IMenusViewModel;
        menusRep: MenusRepresentation;
        onPaneId: number;
        items: ILinkViewModel[];
    }

    export interface IMenuViewModel extends IMessageViewModel {
        id: string;
        title: string;
        actions: IActionViewModel[];
        menuItems: IMenuItemViewModel[];
        menuRep: Models.MenuRepresentation;
    }

    export interface ITableRowColumnViewModel {
        type: "ref" | "scalar";
        returnType: string;
        value: scalarValueType | Date;
        formattedValue: string;
        title: string;
    }

    export interface ITableRowViewModel {
        title: string;
        hasTitle: boolean;
        properties: ITableRowColumnViewModel[];
    }

    export interface  IApplicationPropertiesViewModel {
        serverVersion: IVersionRepresentation;
        user: IUserRepresentation;
        serverUrl: string;
        clientVersion: string;
    }

    export interface IToolBarViewModel {
        loading: string;
        template: string;
        footerTemplate: string;
        goHome: (right?: boolean) => void;
        goBack: () => void;
        goForward: () => void;
        swapPanes: () => void;
        logOff: () => void;
        singlePane: (right?: boolean) => void;
        recent: (right?: boolean) => void;
        cicero: () => void;
        userName: string;
        applicationProperties: () => void;

        warnings: string[];
        messages: string[];
    }

    export interface IRecentItemsViewModel {
        onPaneId: number;
        items: IRecentItemViewModel[];
    }

    export interface IDomainObjectViewModel extends IMessageViewModel {
        domainObject: Models.DomainObjectRepresentation;
        onPaneId: number;

        title: string;
        friendlyName: string;
        presentationHint: string;
        domainType: string;

        isInEdit: boolean;

        actions: IActionViewModel[];
        menuItems: IMenuItemViewModel[];
        properties: IPropertyViewModel[];
        collections: ICollectionViewModel[];

        refresh: (routeData: PaneRouteData) => void;
        reset: (obj: DomainObjectRepresentation, routeData: PaneRouteData) => IDomainObjectViewModel;
        concurrency: () => (event: ng.IAngularEvent, em: ErrorMap) => void;
        clientValid: () => boolean;
        tooltip: () => string;
        actionsTooltip: () => string;
        toggleActionMenu: () => void;
        setProperties: () => void;
        doEditCancel: () => void;
        clearCachedFiles: () => void;
        doSave: (viewObject: boolean) => void;
        doSaveValidate: () => ng.IPromise<boolean>;
        doEdit: () => void;
        doReload: () => void;
        hideEdit: () => boolean;
        disableActions: () => boolean;
    }

    export interface ICiceroViewModel {
        message: string;
        output: string;
        alert: string;
        input: string;
        parseInput: (input: string) => void;
        previousInput: string;
        chainedCommands: string[];

        selectPreviousInput: () => void;

        clearInput: () => void;

        autoComplete: (input: string) => void;

        outputMessageThenClearIt: () => void;

        renderHome: (routeData: PaneRouteData) => void;
        renderObject: (routeData: PaneRouteData) => void;
        renderList: (routeData: PaneRouteData) => void;
        renderError: () => void;
        viewType: ViewType;
        clipboard: DomainObjectRepresentation;

        executeNextChainedCommandIfAny: () => void;

        popNextCommand: () => string;

        clearInputRenderOutputAndAppendAlertIfAny: (output: string) => void;
    }
}