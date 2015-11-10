/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini{

    export interface ICommandFactory {

        //Gets the command and (attempts to) invoke it
        //Command may be abbreviated
        executeCommand(command: string): void;

        //Command may be abbreviated
        getHelpTextForCommand(command: string): string;

        //Returns all command words that may be invoked in the current context
        allCommandsForCurrentContext(): string[];
    }

    app.service('commandFactory', function ($q: ng.IQService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $cacheFactory: ng.ICacheFactoryService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,    
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        navigation: INavigation,
        clickHandler: IClickHandler) {

        var commandFactory = <ICommandFactory>this;
        
  

    });

}