import * as Ro from '../ro-interfaces';
import * as Models from '../models';
import * as Helpersviewmodels from './helpers-view-models';
import * as Choiceviewmodel from './choice-view-model';
import * as Msg from "../user-messages";
import * as Maskservice from '../mask.service';
import * as _ from "lodash";

export class TableRowColumnViewModel {

  
    constructor(id: string, propertyRep?: Models.PropertyMember | Models.CollectionMember, mask? : Maskservice.MaskService) {
       
        this.id = id;

        if (propertyRep && mask) {

            this.title = propertyRep.extensions().friendlyName();

            if (propertyRep instanceof Models.CollectionMember) {
                const size = propertyRep.size();

                this.formattedValue = Helpersviewmodels.getCollectionDetails(size);
                this.value = "";
                this.type = "scalar";
                this.returnType = "string";
            }

            if (propertyRep instanceof Models.PropertyMember) {
                const isPassword = propertyRep.extensions().dataType() === "password";
                const value = propertyRep.value();
                this.returnType = propertyRep.extensions().returnType();

                if (propertyRep.isScalar()) {
                    this.type = "scalar";
                    Helpersviewmodels.setScalarValueInView(this, propertyRep, value);

                    const remoteMask = propertyRep.extensions().mask();
                    const localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());

                    if (propertyRep.entryType() === Models.EntryType.Choices) {
                        const currentChoice = Choiceviewmodel.ChoiceViewModel.create(value, id);
                        const choices = _.map(propertyRep.choices(), (v, n) => Choiceviewmodel.ChoiceViewModel.create(v, id, n));
                        const choice = _.find(choices, c => c.valuesEqual(currentChoice));

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
                    this.type = "ref";
                    this.formattedValue = value.isNull() ? "" : value.toString();
                }
            }
        } else {
            this.type = "scalar";
            this.value = "";
            this.formattedValue = "";
            this.title = "";
        }
    }

    type: "ref" | "scalar";
    returnType: string;
    value: Ro.scalarValueType | Date;
    formattedValue: string;
    title: string;
    id: string;
}