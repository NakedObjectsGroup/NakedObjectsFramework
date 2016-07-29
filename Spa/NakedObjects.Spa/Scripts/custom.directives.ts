/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="typings/moment/moment.d.ts"/>

namespace NakedObjects {
    import Scope = angular.IScope;

    //Renders a calendar view of a named collection within an object using the fullCalendar.js library, which must
    //be referenced (along with its standard styling)
    //The directive must be called passing an object with these properties:
    //nameOfCollection -  the (formatted) name of the collection within the object that is to be rendered as a calendar
    //startPropertyId -  the Id for the property (i.e. column if it was a table) within the collection representing the start DateTime
    //endPropertyId -  the Id for the property (i.e. column if it was a table) within the collection representing the end DateTime
    //allDay - true if all events are all-day (in which case the endPropertyId is ignored)
    //defaultView  -  one of 'month agendaWeek agendaDay' (views defined in fullCalendar.js).
    app.directive("customCalendar", (mask: IMask, $timeout: ng.ITimeoutService, urlManager: IUrlManager, context: IContext, error: IError, color: IColor, $parse: any): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to attributes only
            restrict: "A",

            // to make sure dynamic ids on element get picked up
            transclude: true,

            link(scope: Scope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                const fn = $parse((attrs as any).customCalendar); // '.customCalendar' here must match the name of the directive, defined above

                const config: {nameOfCollection: string, startPropertyId: string, endPropertyId: string, allDay: boolean, defaultView: string} = fn();

                const object = (scope.$parent as INakedObjectsScope).object;
                const collection :ICollectionViewModel = _.find(object.collections, c => c.title === config.nameOfCollection);
                const content: any = element.append("<div class='content'></div>");

                const collectionRep = collection.collectionRep as Models.CollectionMember;

                showCalendar(collectionRep, config.startPropertyId, config.endPropertyId, config.allDay, config.defaultView, $timeout, urlManager, context, error, color, content);
            }
        };
    });

    function showCalendar(collectionRep: Models.CollectionMember,
        startDateId: string,
        endDateId: string,
        allDay: boolean,
        defaultView: string,
        $timeout: ng.ITimeoutService,
        urlManager: IUrlManager,
        context: IContext,
        error: IError,
        color: IColor,
        content: any) {

        context.getCollectionDetails(collectionRep, CollectionViewState.Table, false).
            then(details => {
                const items: Models.Link[] = details.value();

                let showDate: Date;

                if (items.length === 0) {
                    showDate = new Date(Date.now());
                }

                const events = _.map(items, (item: Models.Link) => {
                    const props = item.members();
                    const start = props[startDateId].value();
                    const end = props[endDateId].value();

                    const startDate = Models.toUtcDate(start);
                    const endDate = Models.toUtcDate(end);

                    if (!showDate || startDate > showDate) {
                        showDate = startDate;
                    }

                    return {
                        title: item.title(),
                        start: startDate.toISOString(),
                        end: endDate.toISOString(),
                        allDay: allDay,
                        vm: item,
                        url: "empty" //Because NOF manages the navigation -  see below
                    };
                });

                content.fullCalendar({
                    header: { center: 'month agendaWeek agendaDay' },
                    defaultView: defaultView,
                    events: events,
                    eventClick: (evt: any) => { //Standard NOF navigation. FullCalendar does not handle right-click :-(
                        $timeout(() => urlManager.setItem(evt.vm, 1));
                        return false;
                    }
                });
                content.fullCalendar("gotoDate", showDate.toISOString());

            }).
            catch((reject: Models.ErrorWrapper) => error.handleError(reject));
    }
}