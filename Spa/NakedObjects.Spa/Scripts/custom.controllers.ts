namespace NakedObjects {
    import ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;

    app.controller("DialogController", ($scope: INakedObjectsScope, urlManager: IUrlManager) => {
        const rd = () => urlManager.getRouteData().pane()[$scope.object.onPaneId];

        // if no dialog open 

        if (!rd().dialogId) {
            urlManager.setDialog("adialogID", $scope.object.onPaneId);
        }
    });

    app.controller("PollingController", ($scope: INakedObjectsScope, $timeout: ng.ITimeoutService, urlManager: IUrlManager) => {

        const scope = $scope;




        const reload = () => $timeout(() => {
           

            // keep reloading while on page 

            const rd = () => urlManager.getRouteData().pane()[scope.object.onPaneId];

            if (ObjectIdWrapper.fromObjectId(rd().objectId).isSame(scope.object.domainObject.getOid())) {
                scope.object.doReload();
                reload();
            }         
        }, 10000);

        reload();
    });

    app.controller("CustomCollectionsController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {

        const scope = $scope;

        const collections = scope.object.collections;

        const c1 = collections[0];
        const c2 = collections[1];
        const c3 = collections[2];

        const dsc1 = c1.doSummary;
        const dlc1 = c1.doList;
        const dtc1 = c1.doTable;

        const dsc2 = c2.doSummary;
        const dlc2 = c2.doList;
        const dtc2 = c2.doTable;

        const dsc3 = c3.doSummary;
        const dlc3 = c3.doList;
        const dtc3 = c3.doTable;

        dsc1();
        dsc2();
        dsc3();

        c1.doList = () => {
        
            dsc2();
            dsc3();
            dlc1();
        }

        c2.doList = () => {
            dsc1();
       
            dsc3();
            dlc2();
        }

        c3.doList = () => {
            dsc1();
            dsc2();
        
            dlc3();
        }

        c1.doTable = () => {
        
            dsc2();
            dsc3();
            dtc1();
        }

        c2.doTable = () => {
            dsc1();
        
            dsc3();
            dtc2();
        }

        c3.doTable = () => {
            dsc1();
            dsc2();
       
            dtc3();
        }


    });
}