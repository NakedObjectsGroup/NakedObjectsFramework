import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Objectcomponent from './object.component';
import * as Abstractdroppablecomponent from './abstract-droppable.component';
import * as Geminicleardirective from './gemini-clear.directive';

@Component({
    selector: 'property',
    templateUrl: 'app/property.component.html',
    directives: [Geminicleardirective.GeminiClearDirective],
    styleUrls: ['app/property.component.css']
})
export class PropertyComponent extends Abstractdroppablecomponent.AbstractDroppableComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) {
        super();
    }

    prop: ViewModels.PropertyViewModel;

    @Input()
    edit: boolean;

    @Input()
    parent: ViewModels.DomainObjectViewModel;

    @Input()
    set property(value: ViewModels.PropertyViewModel) {
        this.prop = value;
        this.droppable = value as ViewModels.IFieldViewModel;
    }

    get property() {
        return this.prop;
    }

    classes(): string {
        return `${this.prop.color}${this.canDrop ? " candrop" : ""}`;
    }

}