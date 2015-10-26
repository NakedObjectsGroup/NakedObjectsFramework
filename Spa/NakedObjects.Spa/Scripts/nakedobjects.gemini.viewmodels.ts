/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface IDraggableViewModel {
        canDropOn:  (targetType: string) => ng.IPromise<boolean>;
        value : Object;
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

        value: Object;
        reference: string;
        choice: ChoiceViewModel;
        domainType: string;
        draggableType : string;
    }

    export class ItemViewModel extends LinkViewModel{
        target: DomainObjectViewModel;      
    }

    export class MessageViewModel {
     
        message: string;
       
        clearMessage() {
            this.message = "";
        }
    }

    export class ValueViewModel extends MessageViewModel {
        value: Object;    
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
        arguments: IValueMap; 
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

        conditionalChoices(args: IValueMap): ng.IPromise<ChoiceViewModel[]> {
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
        menuPath : string;
        title: string;
        description: string;
        doInvoke: (right?: boolean) => void;
    } 

    export class DialogViewModel extends MessageViewModel {

        title: string;
        message: string;
        isQueryOnly: boolean;

        parameters: ParameterViewModel[];

        doClose: () => void;
        doInvoke: (right?: boolean) => void;

        clearMessages() {
            this.message = "";
            _.each(this.parameters, parm => parm.clearMessage());
        }
    } 
    
    export class PropertyViewModel extends ValueViewModel implements IDraggableViewModel {

        target: string;       
        isEditable: boolean;    
        attachment: AttachmentViewModel;
        draggableType : string;
        doClick(right?: boolean): void { }
        canDropOn: (targetType: string) => ng.IPromise<boolean>;
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

        template: string;
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

        showEdit(): boolean {
            return  !this.isTransient &&  _.any(this.properties, (p) => p.isEditable);
        }

        canDropOn: (targetType: string) => ng.IPromise<boolean>;

        value: Object;
        reference: string;
        choice : ChoiceViewModel;
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

    //Cicero
    export class CiceroViewModel {
        wrapped: any;
        message: string;
        output() {

            const vm = this.wrapped;
            if (vm instanceof DomainObjectViewModel) {
                const fullName = (<DomainObjectViewModel> vm).domainType;
                const shortName = fullName.substr(fullName.lastIndexOf(".")+1);
                return shortName +": "+ vm.title;
            }
            if (this.wrapped == null) {
                return "Home";
            }
        }
        command: string;
        processCommand: (input: string) => void;
    }
}