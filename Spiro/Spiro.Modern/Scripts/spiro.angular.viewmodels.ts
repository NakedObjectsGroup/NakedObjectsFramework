/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.app.ts" />

module Spiro.Angular {


    export class AttachmentViewModel {
        href: string;
        mimeType: string;
        title: string;

        static create(href : string, mimeType : string, title : string) {
            var attachmentViewModel = new AttachmentViewModel();

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
        isEnum : boolean; 

        static create(value: Value, id : string, name? : string, searchTerm? : string) {
            var choiceViewModel = new ChoiceViewModel();

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

        match(other : ChoiceViewModel) {
            if (this.isEnum) {
                return this.value.trim() === other.value.trim();
            }
            return this.search.trim() == other.search.trim(); 
        }
    }

    export class ErrorViewModel {
        message: string;
        stackTrace: string[];   
    } 


    export class LinkViewModel {
        title: string;
        href: string;
        color: string; 
    } 

    export class ItemViewModel {
        title: string;
        href: string;
        color: string;
        target : DomainObjectViewModel;
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
        arguments: ValueMap; 
        mask: string;
        isMultipleChoices: boolean; 
        minLength : number;

        setSelectedChoice() {}

        prompt(searchTerm: string): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        conditionalChoices(args: ValueMap): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        getMemento(): string {
            if (this.hasChoices) {
                if (this.isMultipleChoices) {
                    var ss = _.map(this.multiChoices, (c) => {
                        return c.search;
                    });

                    if (ss.length === 0) {
                        return "";
                    }

                    return _.reduce(ss, (m: string, s) => {
                        return m + "-" + s;
                    });
                } else {
                    return (this.choice && this.choice.search) ? this.choice.search : this.getValue().toString();
                }
            }

            return this.getValue().toString();
        }

        getValue(): Value {
           
            if (this.hasChoices || this.hasPrompt || this.hasConditionalChoices) {

                if (this.isMultipleChoices) {
                    var selections = this.multiChoices || [];

                    if (this.type === "scalar") {
                        var selValues = _.map(selections, (cvm: ChoiceViewModel) => cvm.value);
                        return new Value(selValues);
                    }

                    var selRefs = _.map(selections, (cvm: ChoiceViewModel) => {
                        return { href: cvm.value, title: cvm.name };
                    });

                    // reference 
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
        title: string;
        href: string;    
    } 

    export class DialogViewModel extends MessageViewModel {

        title: string;
        message: string;
        close: string; 
        isQuery: boolean; 
        show: boolean; 

        parameters: ParameterViewModel[];

        doShow() { }
        doInvoke() { }

        clearMessages() {
            this.message = ""; 
            _.each(this.parameters, (parm) => parm.clearMessage());
        }

    } 
    
    export class PropertyViewModel extends ValueViewModel {

        href: string;
        target: string;
        color: string;       
        isEditable: boolean;    
        attachment: AttachmentViewModel;
    } 

    export class CollectionViewModel {
        title: string;
        size: number;
        pluralName: string;
        href: string;
        color: string; 
        items: ItemViewModel[];
        header : string[];      
    } 

    export class ServicesViewModel {
        title: string; 
        color: string; 
        items: LinkViewModel[];       
    } 

    export class ServiceViewModel {
        title: string;
        serviceId: string;
        actions: ActionViewModel[];
        color: string; 
        href: string; 
    } 

    export class DomainObjectViewModel extends MessageViewModel{
        title: string;
        domainType: string; 
        properties: PropertyViewModel[];
        collections: CollectionViewModel[];
        actions: ActionViewModel[];
        color: string; 
        href: string; 
        cancelEdit: string; 
        doSave(): void {}
    } 
}