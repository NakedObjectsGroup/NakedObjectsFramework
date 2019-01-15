import { Component, ComponentFactoryResolver, OnDestroy, ViewChild, ViewContainerRef } from '@angular/core';
import { Type } from '@angular/core/src/type';
import { ActivatedRoute } from '@angular/router';
import * as Ro from '@nakedobjects/restful-objects';
import { ConfigService, ContextService, PaneRouteData, UrlManagerService, ViewType } from '@nakedobjects/services';
import { CustomComponentService } from '../custom-component.service';
import { PaneComponent } from '../pane/pane';

@Component({
    selector: 'nof-dynamic-object',
    templateUrl: 'dynamic-object.component.html',
    styleUrls: ['dynamic-object.component.css']
})
export class DynamicObjectComponent extends PaneComponent implements OnDestroy {

    @ViewChild('parent', { read: ViewContainerRef })
    parent: ViewContainerRef;

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        context: ContextService,
        private readonly componentFactoryResolver: ComponentFactoryResolver,
        private readonly customComponentService: CustomComponentService,
        private readonly configService: ConfigService) {
        super(activatedRoute, urlManager, context);
    }

    private lastOid: string;

    protected setup(routeData: PaneRouteData) {
        if (!routeData.objectId) {
            return;
        }
        const oid = Ro.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator);

        if (oid.domainType !== this.lastOid) {
            this.lastOid = oid.domainType;
            this.parent.clear();

            this.customComponentService.getCustomComponent(this.lastOid, ViewType.Object).then((c: Type<any>) => {
                const childComponent = this.componentFactoryResolver.resolveComponentFactory(c);
                this.parent.createComponent(childComponent);
            });
        }
    }

    ngOnDestroy(): void {
        super.ngOnDestroy();
        this.parent.clear();
    }
}
