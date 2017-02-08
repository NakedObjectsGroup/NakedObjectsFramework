import { Component, OnInit, ComponentFactoryResolver, ViewChild, ViewContainerRef } from '@angular/core';
import * as Customcomponentservice from '../custom-component.service';
import { Type } from '@angular/core/src/type';
import * as Routedata from '../route-data';

@Component({
  selector: 'nof-dynamic-error',
  templateUrl: './dynamic-error.component.html',
  styleUrls: ['./dynamic-error.component.css']
})
export class DynamicErrorComponent implements OnInit {

    @ViewChild('parent', { read: ViewContainerRef })
    parent: ViewContainerRef;

    constructor(
        private readonly componentFactoryResolver: ComponentFactoryResolver,
        private readonly customComponentService: Customcomponentservice.CustomComponentService
    ) { }

  ngOnInit() {
      this.customComponentService.getCustomComponent("", Routedata.ViewType.Error).then((c: Type<any>) => {
          const childComponent = this.componentFactoryResolver.resolveComponentFactory(c);
          this.parent.createComponent(childComponent);
      });
  }
}
