import { Component, OnInit, ComponentFactoryResolver, ViewChild, ViewContainerRef } from '@angular/core';
import * as Customcomponentservice from '../custom-component.service';
import { Type } from '@angular/core/src/type';
import * as Routedata from '../route-data';
import * as Contextservice from '../context.service';

@Component({
    selector: 'nof-dynamic-error',
    template: require('./dynamic-error.component.html'),
    styles: [require('./dynamic-error.component.css')]
})
export class DynamicErrorComponent implements OnInit {

    @ViewChild('parent', { read: ViewContainerRef })
    parent: ViewContainerRef;

    constructor(
        private readonly context: Contextservice.ContextService,
        private readonly componentFactoryResolver: ComponentFactoryResolver,
        private readonly customComponentService: Customcomponentservice.CustomComponentService
    ) { }

    ngOnInit() {
        // todo this will fail if error expired 
        const errorWrapper = this.context.getError();
        this.customComponentService.getCustomErrorComponent(errorWrapper.category, errorWrapper.httpErrorCode | errorWrapper.clientErrorCode).then((c: Type<any>) => {
            const childComponent = this.componentFactoryResolver.resolveComponentFactory(c);
            this.parent.createComponent(childComponent);
        });
    }
}
