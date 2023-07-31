import { Component, ComponentFactoryResolver, OnInit, Type, ViewChild, ViewContainerRef } from '@angular/core';
import { ContextService, LoggerService, UrlManagerService } from '@nakedobjects/services';
import { CustomComponentService } from '../custom-component.service';

@Component({
    selector: 'nof-dynamic-error',
    templateUrl: 'dynamic-error.component.html',
    styleUrls: ['dynamic-error.component.css']
})
export class DynamicErrorComponent implements OnInit {

    @ViewChild('parent', { read: ViewContainerRef, static : true })
    parent?: ViewContainerRef;

    constructor(
        private readonly context: ContextService,
        private readonly componentFactoryResolver: ComponentFactoryResolver,
        private readonly customComponentService: CustomComponentService,
        private readonly loggerService: LoggerService,
        private readonly urlManagerService: UrlManagerService
    ) { }

    ngOnInit() {

        const errorWrapper = this.context.getError();
        const code = errorWrapper.httpErrorCode || errorWrapper.clientErrorCode;
        if (errorWrapper && code && this.parent) {
            this.customComponentService.getCustomErrorComponent(errorWrapper.category, code).then(c => {
                const childComponent = this.componentFactoryResolver.resolveComponentFactory(c as Type<unknown>);
                this.parent!.createComponent(childComponent);
            });
        } else {
            this.loggerService.warn('No error found returning to home page');
            this.urlManagerService.setHomeSinglePane();
        }
    }
}
