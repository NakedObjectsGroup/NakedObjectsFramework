import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { ObjectComponent } from './object/object.component';
import { ListComponent } from './list/list.component';
import { ErrorComponent } from './error/error.component';
import { ActionsComponent } from './actions/actions.component';
import { ActionComponent } from './action/action.component';
import { PropertiesComponent } from './properties/properties.component';
import { CollectionsComponent } from './collections/collections.component';
import { DialogComponent } from './dialog/dialog.component';
import { ParametersComponent } from './parameters/parameters.component';
import { PropertyComponent } from './property/property.component';
import { ParameterComponent } from './parameter/parameter.component';
import { AutoCompleteComponent } from './auto-complete/auto-complete.component';
import { RecentComponent } from './recent/recent.component';
import { ApplicationPropertiesComponent } from './application-properties/application-properties.component';
import { GeminiConditionalChoicesDirective } from './gemini-conditional-choices.directive';
import { GeminiClickDirective } from './gemini-click.directive';
import { GeminiBooleanDirective } from './gemini-boolean.directive';
import { GeminiClearDirective } from './gemini-clear.directive';
import { GeminiValidateDirective } from './gemini-validate.directive';

@NgModule({
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
    DialogComponent,
    ParametersComponent,
    PropertyComponent,
    ParameterComponent,
    AutoCompleteComponent,
    RecentComponent,
    ApplicationPropertiesComponent,
    GeminiConditionalChoicesDirective,
    GeminiClickDirective,
    GeminiBooleanDirective,
    GeminiClearDirective,
    GeminiValidateDirective
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
