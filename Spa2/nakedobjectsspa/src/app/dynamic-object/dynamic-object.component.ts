import { Component, OnInit, ComponentFactoryResolver, ViewChild, ViewContainerRef } from '@angular/core';
import { ObjectComponent } from '../object/object.component';

@Component({
    selector: 'nof-dynamic-object',
    templateUrl: './dynamic-object.component.html',
    styleUrls: ['./dynamic-object.component.css']
})
export class DynamicObjectComponent implements OnInit {

    @ViewChild('parent', { read: ViewContainerRef })
    parent: ViewContainerRef;

    constructor(private readonly componentFactoryResolver: ComponentFactoryResolver) {
    }

    ngOnInit() {
        const childComponent = this.componentFactoryResolver.resolveComponentFactory(ObjectComponent);
        this.parent.createComponent(childComponent);
    }
}
