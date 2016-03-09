/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular {

    app.run((mask: IMask) => {

        // map to convert from mask representation in RO extension to client represention.
   
        mask.setCurrencyMaskMapping("C", "decimal", "£", 2);
        mask.setCurrencyMaskMapping("c", "decimal", "£", 2);
        mask.setDateMaskMapping("d", "date-time", "d MMM yyyy", "+0000");
        mask.setDateMaskMapping("D", "date", "d MMM yyyy hh:mm:ss");
    });
}