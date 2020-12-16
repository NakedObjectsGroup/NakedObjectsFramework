﻿import * as Ro from '@nakedobjects/restful-objects';
import { MaskService } from '@nakedobjects/services';
import find from 'lodash-es/find';
import map from 'lodash-es/map';
import { ChoiceViewModel } from './choice-view-model';
import * as Helpers from './helpers-view-models';
import * as Msg from './user-messages';

export class TableRowColumnViewModel {

    constructor(
        public readonly id: string,
        propertyRep?: Ro.PropertyMember | Ro.CollectionMember,
        mask?: MaskService
    ) {

        if (propertyRep && mask) {

            this.title = propertyRep.extensions().friendlyName();

            if (propertyRep instanceof Ro.CollectionMember) {
                const size = propertyRep.size();

                this.formattedValue = Helpers.getCollectionDetails(size);
                this.value = '';
                this.type = 'scalar';
                this.returnType = 'string';
            }

            if (propertyRep instanceof Ro.PropertyMember) {
                const isPassword = propertyRep.extensions().dataType() === 'password';
                const value = propertyRep.value();
                this.returnType = propertyRep.extensions().returnType()!;

                if (propertyRep.isScalar()) {
                    this.type = 'scalar';
                    Helpers.setScalarValueInView(this, propertyRep, value);

                    const remoteMask = propertyRep.extensions().mask();
                    const localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format()!);

                    if (propertyRep.entryType() === Ro.EntryType.Choices) {
                        const currentChoice = new ChoiceViewModel(value, id);
                        const choicesMap = propertyRep.choices()!;
                        const choices = map(choicesMap, (v, n) => new ChoiceViewModel(v, id, n));
                        const choice = find(choices, c => c.valuesEqual(currentChoice));

                        if (choice) {
                            this.value = choice.name;
                            this.formattedValue = choice.name;
                        }
                    } else if (isPassword) {
                        this.formattedValue = Msg.obscuredText;
                    } else {
                        this.formattedValue = localFilter.filter(this.value);
                    }
                } else {
                    // is reference
                    this.type = 'ref';
                    this.formattedValue = value.isNull() ? '' : value.toString();
                }
            }
        } else {
            this.type = 'scalar';
            this.value = '';
            this.formattedValue = '';
            this.title = '';
        }
    }

    readonly type: 'ref' | 'scalar';
    readonly returnType: string;
    readonly value: Ro.ScalarValueType | Date;
    readonly formattedValue: string;
    readonly title: string;
}
