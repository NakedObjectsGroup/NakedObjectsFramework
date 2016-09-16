import { NgModule }       from '@angular/core';
import { BrowserModule }  from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { HttpModule }     from '@angular/http';
import { routing }        from './app.routing';
import { AppComponent }  from './app.component';
import { HomeComponent } from './home.component';
import { ObjectComponent } from './object.component';
import { ListComponent } from './list.component';
import { Context } from './context.service';
import { UrlManager } from './urlmanager.service';
import { RepLoader } from './reploader.service';
import { ErrorComponent } from './error.component';
import { ActionsComponent} from './actions.component';
import { PropertiesComponent} from './properties.component';
import { DialogComponent} from './dialog.component';
import { ParametersComponent} from './parameters.component';
import { PropertyComponent} from './property.component';
import { ParameterComponent} from './parameter.component';
import { AutoCompleteComponent} from './autocomplete.component';
import { CollectionsComponent} from './collections.component';
import { ActionComponent }from './action.component';
import { CollectionComponent }from './collection.component';
import { GeminiConditionalChoicesDirective }from './gemini-conditional-choices.directive';
import { GeminiClearDirective }from './gemini-clear.directive';
import { GeminiValidateDirective }from './gemini-validate.directive';
import { GeminiBooleanDirective }from './gemini-boolean.directive';
import { RepresentationsService }from './representations.service';
import { ClickHandlerService }from './click-handler.service';
import { ViewModelFactory }from './view-model-factory.service';
import { Color }from './color.service';
import { Error }from './error.service';
import { FocusManager }from './focus-manager.service';
import { Mask }from './mask.service';
import { ColorServiceConfig }from './color.service.config';
import { MaskServiceConfig }from './mask.service.config';
import { FooterComponent }from './footer.component';
import { GeminiClickDirective }from './gemini-click.directive';
import {DND_PROVIDERS, DND_DIRECTIVES} from 'ng2-dnd';
@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        routing,
        HttpModule
    ],
    declarations: [
        AppComponent,
        FooterComponent, 
        HomeComponent,
        ObjectComponent,
        ListComponent,
        ErrorComponent,
        ActionsComponent,
        ActionComponent,
        PropertiesComponent,
        CollectionsComponent,
        CollectionComponent,
        DialogComponent, 
        ParametersComponent,
        PropertyComponent, 
        ParameterComponent, 
        AutoCompleteComponent,
        GeminiConditionalChoicesDirective,
        GeminiClickDirective,
        GeminiBooleanDirective,
        GeminiClearDirective,
        GeminiValidateDirective,
        DND_DIRECTIVES
    ],
    providers: [
        RepresentationsService,
        UrlManager,
        ClickHandlerService,
        Context,
        RepLoader,
        ViewModelFactory,
        Color,
        Error,
        FocusManager,
        Mask,
        ColorServiceConfig,
        MaskServiceConfig,
        DND_PROVIDERS
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}