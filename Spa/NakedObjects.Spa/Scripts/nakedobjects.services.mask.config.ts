/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects {

    app.run((mask: IMask) => {

        // map to convert from mask representation in RO extension to client represention.

        var pound = "\u00A3";

        mask.setCurrencyMaskMapping("C", "decimal", pound, 2);
        mask.setCurrencyMaskMapping("c", "decimal", pound, 2);
        mask.setDateMaskMapping("d", "date-time", "d MMM yyyy", "+0000");
        mask.setDateMaskMapping("D", "date", "d MMM yyyy hh:mm:ss");
    });
}