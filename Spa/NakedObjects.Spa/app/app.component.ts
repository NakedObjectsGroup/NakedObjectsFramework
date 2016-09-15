import { Component, OnInit } from '@angular/core';
import { FooterComponent } from "./footer.component";
import { RepresentationsService } from "./representations.service";
import { UrlManager } from "./urlmanager.service";
import { ClickHandlerService } from "./click-handler.service";
import { Context } from "./context.service";
import { RepLoader } from "./reploader.service";
import { ViewModelFactory } from "./view-model-factory.service";
import { Color } from "./color.service";
import { Error } from "./error.service";
import { FocusManager } from "./focus-manager.service";
import { Mask} from "./mask.service";
import { ColorServiceConfig } from "./color.service.config";
import { MaskServiceConfig } from "./mask.service.config";
import {DND_PROVIDERS, DND_DIRECTIVES} from 'ng2-dnd';

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.component.html'
})
export class AppComponent implements OnInit {

    backgroundColor: string;

    ngOnInit(): void {
        // todo
        this.backgroundColor = "object-color0";
    }
}