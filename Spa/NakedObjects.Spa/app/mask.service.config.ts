import * as Models from "./models";
import * as _ from "lodash";
import { Injectable } from '@angular/core';
import { Context } from './context.service';
import { Mask } from "./mask.service";

@Injectable()
export class MaskServiceConfig {

    configure(mask: Mask) {

        // map to convert from mask representation in RO extension to client represention.

        const pound = "\u00A3";

        mask.setCurrencyMaskMapping("C", "decimal", pound, 2);
        mask.setCurrencyMaskMapping("c", "decimal", pound, 2);
        mask.setDateMaskMapping("d", "date-time", "d MMM yyyy", "+0000");
        //Note: "D" is the default mask for anything sent to the client as a date+time,
        //where no other mask is specified.
        //This mask deliberately does not specify the timezone as "+0000", unlike the other masks,
        //with the result that the date+time will be transformed to the timezone of the client.
        mask.setDateMaskMapping("D", "date", "d MMM yyyy hh:mm:ss");
        mask.setDateMaskMapping("T", "time", "HH:mm", "+0000");
    }
}
