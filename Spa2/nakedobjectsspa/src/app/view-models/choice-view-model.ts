import * as Models from '../models';

// todo might simplify things if this were always the result of a input ? 
export class ChoiceViewModel {

    constructor(
        private readonly wrapped : Models.Value,
        private readonly id: string,
        name?: string,
        searchTerm?: string
    ) {
        this.name = name || wrapped.toString();
        this.search = searchTerm || this.name;
        this.isEnum = !wrapped.isReference() && (this.name !== this.getValue().toValueString());
    }

    readonly name: string;

    private readonly search: string;
    private readonly isEnum: boolean;
   
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