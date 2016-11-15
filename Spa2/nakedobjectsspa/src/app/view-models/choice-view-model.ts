import * as Models from '../models';

export class ChoiceViewModel {
    name: string;

    private id: string;
    private search: string;
    private isEnum: boolean;
    private wrapped: Models.Value;

    static create(value: Models.Value, id: string, name?: string, searchTerm?: string) {
        const choiceViewModel = new ChoiceViewModel();
        choiceViewModel.wrapped = value;
        choiceViewModel.id = id;
        choiceViewModel.name = name || value.toString();
        choiceViewModel.search = searchTerm || choiceViewModel.name;

        choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.getValue().toValueString());
        return choiceViewModel;
    }

    getValue() {
        return this.wrapped;
    }

    equals(other: ChoiceViewModel): boolean {
        return other instanceof ChoiceViewModel &&
            this.id === other.id &&
            this.name === other.name &&
            this.wrapped.toValueString() === other.wrapped.toValueString();
    }

    valuesEqual(other: ChoiceViewModel): boolean {

        if (other instanceof ChoiceViewModel) {
            const thisValue = this.isEnum ? this.wrapped.toValueString().trim() : this.search.trim();
            const otherValue = this.isEnum ? other.wrapped.toValueString().trim() : other.search.trim();
            return thisValue === otherValue;
        }
        return false;
    }

    toString() {
        return this.name;
    }
}