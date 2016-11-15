
import * as Models from '../models';
import * as Parameterviewmodel from './parameter-view-model';

export class ActionViewModel {
    actionRep: Models.ActionMember | Models.ActionRepresentation;
    invokableActionRep: Models.IInvokableAction;

    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;

    doInvoke: (right?: boolean) => void;
    execute: (pps: Parameterviewmodel.ParameterViewModel[], right?: boolean) => Promise<Models.ActionResultRepresentation>;
    disabled: () => boolean;
    parameters: () => Parameterviewmodel.ParameterViewModel[];
    makeInvokable: (details: Models.IInvokableAction) => void;
}