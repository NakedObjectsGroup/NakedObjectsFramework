import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RoutingModule } from './app-routing.module';
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
import { EditPropertyComponent } from './edit-property/edit-property.component';
import { ViewPropertyComponent } from './view-property/view-property.component';
import { EditParameterComponent } from './edit-parameter/edit-parameter.component';
import { RecentComponent } from './recent/recent.component';
import { ApplicationPropertiesComponent } from './application-properties/application-properties.component';
import { GeminiClickDirective } from './gemini-click.directive';
import { GeminiBooleanDirective } from './gemini-boolean.directive';
import { GeminiClearDirective } from './gemini-clear.directive';
import { FieldComponent } from './field/field.component';
import { ErrorService } from './error.service';
import { ContextService } from './context.service';
import { UrlManagerService } from './url-manager.service';
import { ClickHandlerService } from './click-handler.service';
import { RepLoaderService } from './rep-loader.service';
import { ViewModelFactoryService } from './view-model-factory.service';
import { ColorService } from './color.service';
import { MaskService } from './mask.service';
import { ColorConfigService } from './color-config.service';
import { MaskConfigService } from './mask-config.service';
import { CollectionComponent } from './collection/collection.component';
import { DndModule } from 'ng2-dnd';
import { ReactiveFormsModule } from '@angular/forms';
import { AttachmentComponent } from './attachment/attachment.component';
import { MultiLineDialogComponent } from './multi-line-dialog/multi-line-dialog.component';
import { ViewParameterComponent } from './view-parameter/view-parameter.component';
import { GeminiErrorHandler } from './error.handler';
import { MenusComponent } from './menus/menus.component';
import { MenuComponent } from './menu/menu.component';
import { ButtonsComponent } from './buttons/buttons.component';
import { ButtonComponent } from './button/button.component';
import { DynamicObjectComponent } from './dynamic-object/dynamic-object.component'
import { CustomComponentService } from './custom-component.service';
import { CustomComponentConfigService } from './custom-component-config.service';
import { DynamicListComponent } from './dynamic-list/dynamic-list.component';
import { ConfigService, configFactory} from './config.service';

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
        EditPropertyComponent,
        ViewPropertyComponent,
        EditParameterComponent,
        RecentComponent,
        ApplicationPropertiesComponent,
        GeminiClickDirective,
        GeminiBooleanDirective,
        GeminiClearDirective,
        CollectionComponent,
        AttachmentComponent,
        MultiLineDialogComponent,
        ViewParameterComponent,
        MenusComponent,
        MenuComponent,
        ButtonsComponent,
        ButtonComponent,
        DynamicObjectComponent,
        DynamicListComponent
    ],
    entryComponents: [
        ObjectComponent,
        ListComponent
    ],
    imports: [
        BrowserModule,
        DndModule.forRoot(),
        FormsModule,
        HttpModule,
        RoutingModule,
        ReactiveFormsModule
    ],
    providers: [
        UrlManagerService,
        ClickHandlerService,
        ContextService,
        RepLoaderService,
        ViewModelFactoryService,
        ColorService,
        ErrorService,
        MaskService,
        ColorConfigService,
        MaskConfigService,
        CustomComponentService,
        CustomComponentConfigService,
        { provide: ErrorHandler, useClass: GeminiErrorHandler },
        ConfigService,
        { provide: APP_INITIALIZER, useFactory: configFactory, deps: [ConfigService], multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
