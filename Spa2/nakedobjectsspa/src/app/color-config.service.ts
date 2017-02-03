import { Injectable } from '@angular/core';
import { IColorServiceConfigurator } from './color.service';

@Injectable()
export class ColorConfigService {

    configure(color: IColorServiceConfigurator) {
        //Note: colour is determined by the FIRST matching rule
        const awm = "AdventureWorksModel.";
        //Match specific class
        color.addType(awm + "Customer", 1);
        color.addType(awm + "Product", 4);
        color.addType(awm + "Employee", 5);
        color.addType(awm + "SalesPerson", 6);
        color.addType(awm + "SpecialOffer", 7);
        color.addType(awm + "Vendor", 9);
        color.addType(awm + "WorkOrder", 12);

        //Match Regex on name
        color.addMatch(/.*SalesOrder.*/, 2); //Matches ..Header & ..Detail
        color.addMatch(/.*PurchaseOrder.*/, 10); //Matches ..Header & ..Detail

        //Match on sub-type
        color.addSubtype(awm + "BusinessEntity", 8); //Matches Person and Store

        //Default colour -  must be specified last
        color.setDefault(0);
    }
}