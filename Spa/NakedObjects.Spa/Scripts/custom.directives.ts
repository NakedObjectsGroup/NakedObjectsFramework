/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="typings/moment/moment.d.ts"/>

namespace NakedObjects {
    import Scope = angular.IScope;

    app.directive("customFullcalendar", (mask: IMask, $timeout: ng.ITimeoutService): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",

            // to make sure dynamic ids on element get picked up
            transclude: true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller

            link(scope: Scope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                const collection = (scope.$parent as INakedObjectsScope).collection;

                const content: any = element.append("<div class='content'></div>");

                let id: number = null;





                function setup() {

                    if (_.every(collection.items, i => i.tableRowViewModel)) {
                        clearInterval(id);

                        let lastDate: Date;

                        const events = _.map(collection.items, i => {
                                const date = i.tableRowViewModel.properties[2].value as Date;

                                if (!lastDate || date > lastDate) {
                                    lastDate = date;
                                }

                                return {
                                    title: i.title,
                                    start: date.toISOString(),
                                    allDay: true,
                                    color: (<any>i).color,
                                    vm: i,
                                    url: "empty"
                                };
                            });

                        content.fullCalendar({
                            events: events,
                            eventClick: (evt: any) => {
                                $timeout(() => evt.vm.doClick(false));
                                return false;
                            }
                        });

                        content.fullCalendar("gotoDate", lastDate.toISOString());                       
                    }
                }

                id = setInterval(setup, 100);
            }
        };
    });


    function showCalendar(collectionRep: Models.CollectionMember,
                            startDateId: string,
                            endDateId: string,
                            $timeout: ng.ITimeoutService,
                            urlManager: IUrlManager,
                            context: IContext,
                            error: IError,
                            color: IColor,
                            content : any) {

        context.getCollectionDetails(collectionRep, CollectionViewState.Table, false).
            then(details => {
                const items: Models.Link[] = details.value();

                let showDate: Date;

                if (items.length === 0) {
                    showDate = new Date(Date.now());
                }

                const events = _.map(items, (i: Models.Link) => {
                    const props = i.members();
                    const start = props[startDateId].value();
                    const end = props[endDateId].value();

                    const startDate = Models.toUtcDate(start);
                    const endDate = Models.toUtcDate(end);

                    if (!showDate || startDate > showDate) {
                        showDate = startDate;
                    }

                    return {
                        title: i.title(),
                        start: startDate.toISOString(),
                        end: endDate.toISOString(),
                        color: color.toColorNumberFromHref(i.href()),
                        vm: i,
                        url: "empty"
                    };
                });

                content.fullCalendar({
                    header: { center: 'month agendaWeek agendaDay' },
                    defaultView: 'agendaDay',
                    events: events,
                    eventClick: (evt: any) => {
                        $timeout(() => urlManager.setItem(evt.vm, 1));
                        return false;
                    }
                });
                content.fullCalendar("gotoDate", showDate.toISOString());

            }).
            catch((reject: Models.ErrorWrapper) => error.handleError(reject));
    }



    app.directive("customWorkordercalendar", (mask: IMask, $timeout: ng.ITimeoutService, urlManager : IUrlManager, context : IContext, error : IError, color : IColor, $parse ): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",

            // to make sure dynamic ids on element get picked up
            transclude: true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
    
            link(scope: Scope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                const fn = $parse((attrs as any).customWorkordercalendar);
                const config = fn();

                const object = (scope.$parent as INakedObjectsScope).object;
                const collection = _.find(object.collections, c => c.title === config.title);
                const content: any = element.append("<div class='content'></div>");
              
                const collectionRep = collection.collectionRep as Models.CollectionMember; 

                showCalendar(collectionRep, config.startDateId, config.endDateId, $timeout, urlManager, context, error, color, content);
            }
        };
    });


}