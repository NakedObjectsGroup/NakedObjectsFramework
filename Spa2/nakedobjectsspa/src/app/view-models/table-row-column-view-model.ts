import * as Ro from '../ro-interfaces';

export class TableRowColumnViewModel {
    type: "ref" | "scalar";
    returnType: string;
    value: Ro.scalarValueType | Date;
    formattedValue: string;
    title: string;
    id: string;
}