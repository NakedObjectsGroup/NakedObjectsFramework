/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface IDraggableViewModel {
        canDropOn:  (targetType: string) => ng.IPromise<boolean>;
        value : number | string | boolean;
        reference: string;
        choice: ChoiceViewModel;
        color: string;
        draggableType : string;
    }

    export class AttachmentViewModel {
        href: string;
        mimeType: string;
        title: string;

        static create(href : string, mimeType : string, title : string) {
            const attachmentViewModel = new AttachmentViewModel();
            attachmentViewModel.href = href;
            attachmentViewModel.mimeType = mimeType;
            attachmentViewModel.title = title || "UnknownFile";

            return attachmentViewModel;
        }
    }

    export class ChoiceViewModel {
        id: string; 
        name: string;
        value: string;
        search: string; 
        isEnum: boolean; 
        wrapped : Value;

        static create(value: Value, id : string, name? : string, searchTerm? : string) {
            const choiceViewModel = new ChoiceViewModel();
            choiceViewModel.wrapped = value;
            choiceViewModel.id = id;
            choiceViewModel.name = name || value.toString(); 
            choiceViewModel.value = value.isReference() ? value.link().href() : value.toValueString();
            choiceViewModel.search = searchTerm || choiceViewModel.name; 

            choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.value);
            return choiceViewModel;
        } 

        equals(other: ChoiceViewModel) {
            return this.id === other.id &&
                   this.name === other.name &&
                   this.value === other.value;
        }

        match(other: ChoiceViewModel) {
            const thisValue = this.isEnum ? this.value.trim() : this.search.trim();
            const otherValue = this.isEnum ? other.value.trim() : other.search.trim();
            return thisValue === otherValue; 
        }

    }

    export class ErrorViewModel {
        message: string;
        stackTrace: string[];   
    } 

    export class LinkViewModel implements IDraggableViewModel{
        title: string;
        color: string;
        doClick: (right?: boolean) => void;

        canDropOn: (targetType: string) => ng.IPromise<boolean>;

        value: number | string | boolean;
        reference: string;
        choice: ChoiceViewModel;
        domainType: string;
        draggableType: string;
        link : Link;
    }

    export class ItemViewModel extends LinkViewModel{
        target: DomainObjectViewModel;   
        selected : boolean;
        checkboxChange: (index: number) => void;
    }

    export class MessageViewModel {
     
        message: string;
       
        clearMessage() {
            this.message = "";
        }
    }

    export class ValueViewModel extends MessageViewModel {
        value: number | string | boolean;    
        id: string; 
        argId: string; 
        choices: ChoiceViewModel[];
        hasChoices: boolean;
        hasPrompt: boolean; 
        hasConditionalChoices: boolean;
        type: string;
        reference: string;
        choice: ChoiceViewModel; 
        multiChoices: ChoiceViewModel[]; 
        returnType: string;
        title: string;
        format: string;
        arguments: _.Dictionary<Value>; 
        mask: string;
        isMultipleChoices: boolean; 
        minLength: number;
        hasAutoAutoComplete: boolean;
        color: string;
        description: string;
        optional: boolean;

        //setSelectedChoice() {}

        prompt(searchTerm: string): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        conditionalChoices(args: _.Dictionary<Value>): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        getMemento(): string {
            if (this.hasChoices) {
                if (this.isMultipleChoices) {
                    const ss = _.map(this.multiChoices, (c) => {
                        return c.search;
                    });

                    if (ss.length === 0) {
                        return "";
                    }

                    return _.reduce(ss, (m: string, s) => {
                        return m + "-" + s;
                    });
                } 

                return (this.choice && this.choice.search) ? this.choice.search : this.getValue().toString(); 
            }

            return this.getValue().toString();
        }

        setNewValue(newValue: IDraggableViewModel) {
            this.value = newValue.value;
            this.reference = newValue.reference;
            this.choice = newValue.choice;
            this.color = newValue.color;
        }

        drop : (newValue: IDraggableViewModel) => void;

        clear() {
            this.value = null;
            this.reference = "";
            this.choice = null;
            this.color = "";
        }

        getValue(): Value {
           
            if (this.hasChoices || this.hasPrompt || this.hasConditionalChoices || this.hasAutoAutoComplete) {

                if (this.isMultipleChoices) {
                    const selections = this.multiChoices || [];
                    if (this.type === "scalar") {
                        const selValues = _.map(selections, cvm => cvm.value);
                        return new Value(selValues);
                    }
                    const selRefs = _.map(selections, cvm => ({ href: cvm.value, title: cvm.name })); // reference 
                    return new Value(selRefs);
                }


                if (this.type === "scalar") {
                    return new Value(this.choice && this.choice.value != null ? this.choice.value : "");
                }

                // reference 
                return new Value(this.choice && this.choice.value ? { href: this.choice.value, title: this.choice.name } : null );
            }

            if (this.type === "scalar") {
                return new Value(this.value == null ? "" : this.value);
            }

            // reference
            return new Value(this.reference ? { href: this.reference } : null);
        }
    }

    export class ParameterViewModel extends ValueViewModel{
        dflt: string;       
    } 

    export class ActionViewModel {
        actionRep : ActionMember;
        menuPath : string;
        title: string;
        description: string;
        doInvoke: (right?: boolean) => void;
        executeInvoke: (dvm: DialogViewModel, right?: boolean)  => void;
        disabled(): boolean { return false; }
    } 

    export class DialogViewModel extends MessageViewModel {

        title: string;
        message: string;
        isQueryOnly: boolean;
        onPaneId: number;

        action : ActionMember;

        parameters: ParameterViewModel[];

        doClose: () => void;
        doInvoke: (right?: boolean) => void;

        clearMessages() {
            this.message = "";
            _.each(this.parameters, parm => parm.clearMessage());
        }

        isSame(paneId : number, otherAction : ActionMember ) {
            return this.onPaneId === paneId && this.action.invokeLink().href() === otherAction.invokeLink().href();
        }
    } 
    
    export class PropertyViewModel extends ValueViewModel implements IDraggableViewModel {

        target: string;
        isEditable: boolean;
        attachment: AttachmentViewModel;
        draggableType: string;
        doClick(right?: boolean): void { }
        canDropOn: (targetType: string) => ng.IPromise<boolean>;
    }

    export class CollectionPlaceholderViewModel {
        description: () => string;
        reload: () => void;
    }

    export class CollectionViewModel {
        title: string;
        size: number;
        pluralName: string;
        color: string; 
        items: ItemViewModel[];
        header: string[];
        onPaneId: number;

        doSummary(): void { }
        doTable(): void { }
        doList(): void { }

        pageFirst(): void { }
        pagePrevious(): void { }
        pageNext(): void { }
        pageLast(): void {}

        pageFirstDisabled(): boolean { return false; }
        pagePreviousDisabled(): boolean { return false; }
        pageNextDisabled(): boolean { return false; }
        pageLastDisabled(): boolean { return false; }

        reload: () => void;

        description(): string { return this.size.toString()}

        template: string;

        disableActions(): boolean {
            return !this.actions || this.actions.length === 0;
        }

        toggleActionMenu(): void { }

        actions: ActionViewModel[];
        messages: string;
    } 

    export class ServicesViewModel {
        title: string; 
        color: string; 
        items: LinkViewModel[];       
    } 

    export class MenusViewModel {
        title: string;
        color: string;
        items: LinkViewModel[];
    } 

    export class ServiceViewModel {
        title: string;
        serviceId: string;
        actions: ActionViewModel[];
        color: string; 
    } 

    export class MenuViewModel {
        title: string;
        actions: ActionViewModel[];
        color: string;
    } 


    export class DomainObjectViewModel extends MessageViewModel implements IDraggableViewModel{
        title: string;
        domainType: string; 
        instanceId: string;
        properties: PropertyViewModel[];
        collections: CollectionViewModel[];
        actions: ActionViewModel[];
        color: string; 
        doSave(edit : boolean): void { }
        toggleActionMenu(): void { }
        isTransient: boolean;
        onPaneId: number;
        draggableType : string;

        doEdit(): void { }
        doEditCancel(): void { }
        doReload(refreshScope?: boolean): void { }
        editComplete(): void { };

        hideEdit(): boolean {
            return  this.isTransient ||  _.all(this.properties, p => !p.isEditable);
        }

        disableActions(): boolean {
            return !this.actions || this.actions.length === 0;
        }

        canDropOn: (targetType: string) => ng.IPromise<boolean>;

        value: number | string | boolean;
        reference: string;
        choice: ChoiceViewModel;

        domainObject : DomainObjectRepresentation;
        isInEdit : boolean = false;

        isSameEditView(paneId: number, otherObject: DomainObjectRepresentation, editing : boolean) {
            const bothEditing = this.isInEdit && editing;
            return bothEditing && this.onPaneId === paneId && this.domainObject.selfLink().href() === otherObject.selfLink().href();
        }
    }

    export class ToolBarViewModel {
        loading: string;
        template: string;
        footerTemplate: string;
        goHome: (right? : boolean) => void;
        goBack: () => void;
        goForward: () => void;
        swapPanes: () => void;
        singlePane: (right?: boolean) => void;
    }

    export class CiceroViewModel {
        output: string;
        input: string;
        parseInput: (input: string) => void;
        previousInput: string;
        selectPreviousInput(): void {
            this.input = this.previousInput;
        }
        clearInput(): void {
            this.input = null;
        }
        setOutputToSummaryOfRepresentation: (routeData: PaneRouteData) => void;
    }
}