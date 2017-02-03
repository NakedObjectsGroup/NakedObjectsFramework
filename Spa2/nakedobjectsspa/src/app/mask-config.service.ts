import * as Models from './models';
import * as _ from 'lodash';
import { Injectable } from '@angular/core';
import { ContextService } from './context.service';
import { IMaskServiceConfigurator } from './mask.service';

@Injectable()
export class MaskConfigService {

    configure(mask: IMaskServiceConfigurator) {

        // map to convert from mask representation in RO extension to client represention.

        const pound = "GBP";

        mask.setCurrencyMaskMapping("C", "decimal", pound, "1.2-2");
        mask.setCurrencyMaskMapping("c", "decimal", pound, "1.2-2");
        mask.setDateMaskMapping("d", "date-time", "d MMM yyyy", "+0000");
        //Note: "D" is the default mask for anything sent to the client as a date+time,
        //where no other mask is specified.
        //This mask deliberately does not specify the timezone as "+0000", unlike the other masks,
        //with the result that the date+time will be transformed to the timezone of the client.
        mask.setDateMaskMapping("D", "date", "d MMM yyyy hh:mm:ss");
        mask.setDateMaskMapping("T", "time", "HH:mm", "+0000");
    }
}

