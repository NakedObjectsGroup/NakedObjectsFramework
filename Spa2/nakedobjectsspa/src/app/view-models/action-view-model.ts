import { ParameterViewModel } from './parameter-view-model';
import * as Models from '../models';

export class ActionViewModel {
    actionRep: Models.ActionMember | Models.ActionRepresentation;
    invokableActionRep: Models.IInvokableAction;

    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;

    doInvoke: (right?: boolean) => void;
    execute: (pps: ParameterViewModel[], right?: boolean) => Promise<Models.ActionResultRepresentation>;
    disabled: () => boolean;
    parameters: () => ParameterViewModel[];
    makeInvokable: (details: Models.IInvokableAction) => void;
}