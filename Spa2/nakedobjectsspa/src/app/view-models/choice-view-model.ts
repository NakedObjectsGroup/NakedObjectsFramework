import * as Models from '../models';

export class ChoiceViewModel {

    constructor(
        private readonly wrapped: Models.Value,
        private readonly id: string,
        name?: string | null,
        searchTerm?: string
    ) {
        this.name = name || wrapped.toString();
        this.search = searchTerm || this.name;
        this.isEnum = !wrapped.isReference() && (this.name !== this.getValue().toValueString());
    }

    readonly name: string;
    private readonly search: string;
    private readonly isEnum: boolean;

    readonly getValue = () => this.wrapped;

    readonly equals = (other: ChoiceViewModel): boolean =>
        other instanceof ChoiceViewModel &&
        this.id === other.id &&
        this.name === other.name &&
        this.wrapped.toValueString() === other.wrapped.toValueString()

    readonly valuesEqual = (other: ChoiceViewModel): boolean => {
        if (other instanceof ChoiceViewModel) {
            const thisValue = this.isEnum ? this.wrapped.toValueString().trim() : this.search.trim();
            const otherValue = this.isEnum ? other.wrapped.toValueString().trim() : other.search.trim();
            return thisValue === otherValue;
        }
        return false;
    }

    readonly toString = () => this.name;
}
