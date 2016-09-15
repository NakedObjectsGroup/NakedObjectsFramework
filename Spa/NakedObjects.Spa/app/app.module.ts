import { NgModule, CUSTOM_ELEMENTS_SCHEMA }       from '@angular/core';
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
import * as Actionscomponent from './actions.component';
import * as Propertiescomponent from './properties.component';
import * as Dialogcomponent from './dialog.component';
import * as Parameterscomponent from './parameters.component';
import * as Propertycomponent from './property.component';
import * as Parametercomponent from './parameter.component';
import * as Autocompletecomponent from './autocomplete.component';
import {DND_PROVIDERS, DND_DIRECTIVES} from 'ng2-dnd';
import * as Collectionscomponent from './collections.component';
import * as Actioncomponent from './action.component';
import * as Collectioncomponent from './collection.component';
import * as Geminiconditionalchoicesdirective from './gemini-conditional-choices.directive';
import * as Geminicleardirective from './gemini-clear.directive';
import * as Geminifieldmandatorycheckdirective from './gemini-field-mandatory-check.directive';
import * as Geminifieldvalidatedirective from './gemini-field-validate.directive';
import * as Geminibooleandirective from './gemini-boolean.directive';
import * as Representationsservice from './representations.service';
import * as Clickhandlerservice from './click-handler.service';
import * as Viewmodelfactoryservice from './view-model-factory.service';
import * as Colorservice from './color.service';
import * as Errorservice from './error.service';
import * as Focusmanagerservice from './focus-manager.service';
import * as Maskservice from './mask.service';
import * as Colorserviceconfig from './color.service.config';
import * as Maskserviceconfig from './mask.service.config';
import * as Footercomponent from './footer.component';
import * as Geminiclickdirective from './gemini-click.directive';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        routing,
        HttpModule
    ],
    declarations: [
        AppComponent,
        Footercomponent.FooterComponent, 
        HomeComponent,
        ObjectComponent,
        ListComponent,
        ErrorComponent,
        Actionscomponent.ActionsComponent,
        Actioncomponent.ActionComponent,
        Propertiescomponent.PropertiesComponent,
        Collectionscomponent.CollectionsComponent,
        Collectioncomponent.CollectionComponent,
        Dialogcomponent.DialogComponent, 
        Parameterscomponent.ParametersComponent,
        Propertycomponent.PropertyComponent, 
        Parametercomponent.ParameterComponent, 
        Autocompletecomponent.AutoCompleteComponent,
        Geminiconditionalchoicesdirective.GeminiConditionalChoicesDirective,
        Geminiclickdirective.GeminiClickDirective,
        Geminibooleandirective.GeminiBooleanDirective,
        Geminicleardirective.GeminiClearDirective,
        Geminifieldmandatorycheckdirective.GeminiFieldMandatoryCheckDirective,
        Geminifieldvalidatedirective.GeminiFieldValidateDirective,
        DND_DIRECTIVES
    ],
    providers: [
        Representationsservice.RepresentationsService,
        UrlManager,
        Clickhandlerservice.ClickHandlerService,
        Context,
        RepLoader,
        Viewmodelfactoryservice.ViewModelFactory,
        Colorservice.Color,
        Errorservice.Error,
        Focusmanagerservice.FocusManager,
        Maskservice.Mask,
        Colorserviceconfig.ColorServiceConfig,
        Maskserviceconfig.MaskServiceConfig,
        DND_PROVIDERS
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}